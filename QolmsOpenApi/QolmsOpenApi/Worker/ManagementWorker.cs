using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Configuration;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Providers;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// マネジメント用のビジネスロジックを実装するクラスです。
    /// </summary>
    public class ManagementWorker 
    {
        #region "インターフェース"

        IManagementRepository _managementRepo;
        IDateTimeProvider _datetimeProv;

        #endregion

        #region "const"

        /// <summary>
        /// アップロードを許可する 文字コード を表します。
        /// </summary>
        private readonly List<Encoding> ALLOWED_ENCODINGS = new List<Encoding>() { Encoding.GetEncoding("shift_jis"), Encoding.GetEncoding("utf-8") };

        /// <summary>
        /// ファイル投稿 Zip内のファイル数制限 を表します。
        /// </summary>
        private readonly Lazy<int> _filePostingLimit = new Lazy<int>(() => QoApiConfiguration.FilePostingLimit.TryToValueType(int.MinValue));

        /// <summary>
        /// アップロードを許可しないファイル拡張子
        /// </summary>
        private readonly string[] NOTALLOWED_FILEEXTENSION = new string[] { ".exe", ".dll", ".com", ".ocx", ".bat", ".js", ".cgi" };

        /// <summary>
        /// アップロードを許可しないマジックナンバー
        /// </summary>
        private readonly byte[] NOTALLOWED_MAGICNUMBER = new byte[] { 0x4D, 0x5A };

        /// <summary>
        /// データ投稿 ファイルアップロード時のMIMEタイプを表します。
        /// </summary>
        private const string UPLOADFILE_CONTENTTYPE = "application/octet-stream";

        /// <summary>
        /// データ投稿のステータス 処理中を表します。
        /// </summary>
        private const byte UPLOADFILE_PROCESSING = 1;

        /// <summary>
        /// ファイル名の最長文字数を表します。
        /// </summary>
        private const byte FILENAME_MAX_LENGTH = 50;

        #endregion

        #region "constructor"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="managementRepository"></param>
        /// <param name="dateTimeProvider"></param>
        public ManagementWorker(IManagementRepository managementRepository, IDateTimeProvider dateTimeProvider)
        {
            _managementRepo = managementRepository;
            _datetimeProv = dateTimeProvider;
        }

        #endregion

        #region "Private Method"

        /// <summary>
        /// Config設定を取得します。
        /// </summary>
        /// <param name="settingsName">設定名</param>
        /// <returns></returns>
        private static string GetConfigSettings(string settingsName)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(settingsName))
            {
                try
                {
                    result = ConfigurationManager.AppSettings[settingsName];
                }
                catch
                {
                }
            }

            return result;
        }

        /// <summary>
        /// エンコードを指定して zipファイルの内容をチェックします。
        /// </summary>
        /// <param name="file">ファイルのバイナリ</param>
        /// <param name="enc">エンコード</param>
        /// <returns></returns>
        private bool IsCheckIllegalZipFileSpecifyEncoding(byte[] file ,Encoding enc)
        {
            try
            {
                using (var ms = new MemoryStream(file))
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Read, false, enc))
                {
                    if (archive.Entries.Count > _filePostingLimit.Value)
                    {
                        return false;
                    }

                    foreach (var entry in archive.Entries)
                    {
                        //ファイル名が空 かつ ファイルサイズが0 の場合は フォルダ扱いとして チェック処理をスキップ
                        if (string.IsNullOrWhiteSpace(entry.Name) && entry.Length == 0)
                        {
                            continue;
                        }

                        var buff = new byte[entry.Length - 1];
                        if (entry.Length > 0)
                        {
                            entry.Open().Read(buff, 0, (int)entry.Length - 1);//longからintへの変換 処理順要確認
                        }

                        if (string.IsNullOrWhiteSpace(entry.Name))
                        {
                            return false;
                        }

                        //拡張子 チェック
                        var extension = Path.GetExtension(entry.Name);
                        if (string.IsNullOrWhiteSpace(extension) || Array.IndexOf(NOTALLOWED_FILEEXTENSION, extension) > 0)//アップロードを許可しない拡張子
                        {
                            return false;
                        }

                        //マジックナンバー チェック
                        if (buff != null && buff[0] == NOTALLOWED_MAGICNUMBER[0] && buff[1] == NOTALLOWED_MAGICNUMBER[1])
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// バイト配列に読み込んだ zipファイルの内容を、不正なファイルか チェックします。
        /// </summary>
        /// <param name="file">zipファイル内容 のバイナリ</param>
        /// <returns>
        /// true : 不正なファイルなし , 
        /// false : 不正なファイルあり
        /// </returns>
        private bool IsCheckIllegalZipFile(byte[] file)
        {
            foreach (var enc in ALLOWED_ENCODINGS)
            {
                if (IsCheckIllegalZipFileSpecifyEncoding(file, enc))
                {
                    return true;
                } 
            }

            return false;
        }

        /// <summary>
        /// ファイル名が不正かどうか チェックします。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>
        /// true : 正しい , 
        /// false : 不正
        /// </returns>
        private bool IsCheckIllegalFileName(string fileName)
        {
            return fileName.Length > 0 && fileName.Length <= FILENAME_MAX_LENGTH ;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// データ投稿 ファイル を 登録します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        public QoManagementFileUploadWriteApiResults FileUpload(QoManagementFileUploadWriteApiArgs args)
        {
            var result = new QoManagementFileUploadWriteApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = new QoApiResultItem()
            };

            Guid fileKey = Guid.Empty;
            var file = Convert.FromBase64String(args.FileData);

            if (IsCheckIllegalFileName(args.OriginalName) && IsCheckIllegalZipFile(file))
            {
                //アップロード開始
                fileKey = _managementRepo.UproadPostingFileBlob(UPLOADFILE_CONTENTTYPE, file, string.Empty, args.OriginalName, args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty));

                if (fileKey == Guid.Empty)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
                    // Blobへ登録失敗 QoAccessLog
                    QoAccessLog.WriteAccessLog(default, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, _datetimeProv.Now, QoAccessLog.AccessTypeEnum.Error, string.Empty, "データ投稿 Blob 登録時エラーです。");

                    return result;
                }

                if (_managementRepo.InsertPostingFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue),fileKey,args.OriginalName,UPLOADFILE_CONTENTTYPE , UPLOADFILE_PROCESSING, string.Empty,false, _datetimeProv.Now,args.AuthorKey.TryToValueType(Guid.Empty)))
                {
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                    string comment = string.Format("「ファイル名：{0}」", args.OriginalName);
                    QoAccessLog.WriteAccessLog(default, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, _datetimeProv.Now, QoAccessLog.AccessTypeEnum.Api, string.Empty, $"データ投稿 新規登録:{comment}");
                }
                else
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "SQL実行エラー 登録に失敗しました。");

                    // DBへ登録失敗
                    QoAccessLog.WriteAccessLog(default, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, _datetimeProv.Now, QoAccessLog.AccessTypeEnum.Error, string.Empty, "データ投稿 登録時エラー：SQL実行エラーです。");

                    // Blobへ登録したファイル情報を削除
                    // 削除に失敗した場合は諦める
                    _managementRepo.DeletePostingFileBlob(fileKey);
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ファイル形式が不正です。");
            }

            return result;
        }

        #endregion
    }
}