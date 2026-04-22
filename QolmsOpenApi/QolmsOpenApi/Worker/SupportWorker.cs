using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using System.Collections.Generic;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{

    /// <summary>
    /// サポートコントローラ要求を処理します
    /// </summary>
    /// <remarks></remarks>
    public sealed class SupportWorker
    {
        private static readonly string SupportFolderPath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "Support");
        private static readonly string ZipFolderPath = System.IO.Path.Combine(SupportFolderPath, "AppLogZipFile");
        private static readonly string LogDataFolderPath = System.IO.Path.Combine(SupportFolderPath, "AppLogFile");

        // 引数をチェックして引数レベルで不正な場合のエラーメッセージを用意する
        private static ArgumentException CheckArgsAndMakeErrorMessage(QoSupportRegisterApiArgs args)
        {
            if (args.Executor.TryToValueType(Guid.Empty) == Guid.Empty)
                return new ArgumentException("実行者が不正です");
            if (string.IsNullOrEmpty(args.UserTel) == false && ulong.TryParse (args.UserTel,out ulong _) == false)
                return new ArgumentException("電話番号が不正です");
            // If String.IsNullOrEmpty(args.ZipData) Then Return New ArgumentException("データがありません")

            return null;
        }

        // お問い合わせテーブルに登録してお問い合わせ番号を返す
        private static long InquiryRegister(QsDbSystemTypeEnum systemType, QsDbInquiryTypeEnum inquiryType, Guid inquiryUserAccountKey, string inquiryUserName, string inquiryUserNameKana, string inquiryUserTel, string inquiryUserMail, string inquiryUserPostalCode, string inquiryContents)
        {
            long result = long.MinValue;
            DbInquiryRegisterWriter writer = new DbInquiryRegisterWriter();
            DbInquiryRegisterWriterArgs writerArgs = new DbInquiryRegisterWriterArgs() { Author = inquiryUserAccountKey, InquiryAccountKey = inquiryUserAccountKey, InquiryContents = inquiryContents, InquiryType = inquiryType, InquiryUserMail = inquiryUserMail, InquiryUserName = inquiryUserName, InquiryUserNameKana = inquiryUserNameKana, InquiryUserPostalCode = inquiryUserPostalCode, InquiryUserTel = inquiryUserTel, StatusNo = 0, SystemType = systemType };
            DbInquiryRegisterWriterResults writerResult = QsDbManager.Write(writer, writerArgs);
            if (writerResult.IsSuccess)
                result = writerResult.InquiryNo;
            return result;
        }

        // 'データ取り込み
        private static bool ImportData(string zipFilePath, string inquiryNo, string executeSystemType, string accountKey)
        {
            bool result = true;
            try
            {
                byte[] data = File.ReadAllBytes(zipFilePath);
                string errorMessage = string.Empty;
                Guid fileKey = WriteBlob(inquiryNo, executeSystemType, accountKey, data, ref errorMessage);
                if ((fileKey != Guid.Empty))
                {
                    // BLOB保存成功
                    QhInquiryFileEntityWriter writer = new QhInquiryFileEntityWriter();
                    QH_INQUIRYFILE_DAT entity = new QH_INQUIRYFILE_DAT() { CONTENTENCODING = "", CONTENTTYPE = "application/zip", CREATEDDATE = DateTime.Now, DELETEFLAG = false, FILEKEY = fileKey, INQUIRYNO = inquiryNo.TryToValueType(long.MinValue), ORIGINALNAME = Path.GetFileName(zipFilePath), UPDATEDDATE = DateTime.Now };
                    QhInquiryFileEntityWriterArgs writerArgs = new QhInquiryFileEntityWriterArgs() { Data = new List<QH_INQUIRYFILE_DAT>() { entity } };
                    QhInquiryFileEntityWriterResults writerResults = QsDbManager.Write(writer, writerArgs);
                    if (writerResults.IsSuccess)
                        // 処理終了後ファイルは削除する
                        File.Delete(zipFilePath);
                }
                else
                    QoAccessLog.WriteErrorLog(errorMessage, accountKey.TryToValueType(Guid.Empty));
            }

            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, accountKey.TryToValueType(Guid.Empty));
                result = false;
            }


            return result;
        }

        /// <summary>
        /// LOG ファイルを、BLOBに登録する。
        /// </summary>
        private static Guid WriteBlob(string inquiryNo, string executeSystemType, string accountKey, byte[] data, ref string refErrorMessage)
        {
            Guid result = Guid.Empty;

            try
            {
                AppLogFileBlobEntityWriter writer = new AppLogFileBlobEntityWriter();
                AppLogFileBlobEntityWriterArgs writerArgs = new AppLogFileBlobEntityWriterArgs()
                {
                    Entity = new QhInquiryFileBlobEntity()
                    {
                        Name = Guid.Empty,
                        ContentType = "application/zip",
                        Data = data,
                        InquiryNo = inquiryNo,
                        ExecuteSystemType = executeSystemType,
                        AccountKey = accountKey
                    }
                };

                AppLogFileBlobEntityWriterResults writerResults = QsAzureStorageManager.Write(writer, writerArgs);
                {
                    if (writerResults.IsSuccess)
                        result = writerResults.Result;
                }
            }
            catch (Exception ex)
            {
                result = Guid.Empty;
                refErrorMessage = ex.Message;
            }

            return result;
        }

        // ''' <summary>
        // ''' 指定したファイルキーのBLOBを削除する。
        // ''' </summary>
        // ''' <param name="fileKey">ファイルキー</param>
        // ''' <returns>True:成功 False:失敗</returns>
        // ''' <remarks></remarks>
        // Private Shared Function DeleteBlob(fileKey As Guid, ByRef refErrorMessage As String) As Boolean

        // Dim result As Boolean = False

        // Try
        // 'BLOBを削除する
        // Dim deleter As New CheckupCdaFileBlobEntityDeleter()
        // Dim deleterArgs As New CheckupCdaFileBlobEntityDeleterArgs() With {
        // .Entity = New QhCheckupCdaFileBlobEntity() With {
        // .Name = fileKey
        // }
        // }

        // Dim deleterResults As CheckupCdaFileBlobEntityDeleterResults = QsAzureStorageManager.Write(deleter, deleterArgs)
        // result = deleterResults.IsSuccess

        // Catch ex As Exception
        // result = False
        // refErrorMessage = ex.Message
        // End Try

        // Return result

        // End Function

        /// <summary>
        /// ユーザへの お問い合わせ受理メール を送信します。
        /// </summary>
        /// <param name="systemType">システム種別</param>
        /// <param name="mailAddress">送信先のメールアドレス</param>
        /// <param name="inquiryNo">お問い合わせ番号</param>
        /// <param name="contents">お問い合わせ内容</param>
        /// <returns></returns>
        private static void SendSupportAcceptanceMail(QsApiSystemTypeEnum systemType, string mailAddress, string inquiryNo, string contents)
        {
            string param = AppWorker.GetUrlParam(systemType);
            string settingsName = string.Format("MailSettingsNamePersonalMessage_{0}", param);
            string bodyPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailBodySupportAcceptance_{0}.txt", param));
            string footerPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailFooter_{0}.txt", param));

            Task.Run(() => 
                new SupportAcceptanceNoticeClient(
                    new SupportAcceptanceNoticeClientArgs(
                        settingsName, mailAddress, inquiryNo, contents, bodyPath, footerPath
                    )
                ).SendAsync()
            );
        }

        /// <summary>
        /// サポートへの お問い合わせメール を送信します。
        /// </summary>
        /// <param name="systemType">システム種別</param>
        /// <param name="inquiryNo">お問い合わせ番号</param>
        /// <returns></returns>
        private static void SendSupportMail(QsApiSystemTypeEnum systemType, string inquiryNo)
        {
            string param = AppWorker.GetUrlParam(systemType);
            string settingsName = string.Format("MailSettingsNamePersonalMessage_{0}", param);
            string mailToSettingsName = string.Format("MailToSupport_{0}", param);
            string bodyPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailBodySupport_{0}.txt", param));
            string footerPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailFooter_{0}.txt", param));
            string subjectName = string.Format("MailSubjectSupport_{0}", param);

            Task.Run(() =>
                new SupportNoticeClient(
                    new SupportNoticeClientArgs(
                        settingsName, subjectName, mailToSettingsName, inquiryNo, bodyPath, footerPath
                    )
                ).SendAsync()
            );
        }

        /// <summary>
        /// お問い合わせ登録とログファイルをアップロードします。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public static QoSupportRegisterApiResults Register(QoSupportRegisterApiArgs args)
        {

            // ゲストさんの場合はアカウントキーがGuid.Empty。
            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            QoSupportRegisterApiResults result = new QoSupportRegisterApiResults() { IsSuccess = bool.FalseString };

            try
            {
                // 引数チェック
                ArgumentException argsEx = CheckArgsAndMakeErrorMessage(args);
                if (argsEx != null)
                {
                    result.Result = QoApiResult.Build(argsEx);
                    QoAccessLog.WriteErrorLog(argsEx,args.Executor.TryToValueType(Guid.Empty));
                    return result;
                }

                // 施設キー参照が設定されていて、医療機関コードが未設定の場合は施設マスタより取得する
                if(!string.IsNullOrEmpty(args.FacilityKeyReference) && string.IsNullOrEmpty(args.MedicalFacilityCode))
                {
                    var facilityKey = args.FacilityKeyReference.ToDecrypedReference<Guid>();
                    var facilityRepo = new FacilityRepository();
                    var facilityEntity = facilityRepo.ReadFacility(facilityKey);
                    if(facilityEntity != null)
                    {
                        args.MedicalFacilityCode = facilityEntity.MEDICALFACILITYCODE;
                    }
                }

                // お問い合わせテーブル登録
                QhInquiryContentsSetOfJson contentsJson = new QhInquiryContentsSetOfJson() {
                    AppVersion = args.AppVersion,
                    DeviceModel = args.DeviceModel, 
                    OsVersion = args.OsVersion, 
                    PlatformOs = args.PlatformOs, 
                    PrescriptionKeys = args.PrescriptionKeys,
                    MedicalFacilityCode = args.MedicalFacilityCode,
                    MedicalFacilityName = args.MedicalFacilityName,
                    MessageText = args.Contents 
                };
                QsJsonSerializer serializer = new QsJsonSerializer();
                string contents = serializer.Serialize(contentsJson);
                long inquiryNo = InquiryRegister(args.ExecuteSystemType.TryToValueType(QsDbSystemTypeEnum.None), QsDbInquiryTypeEnum.Question, accountKey, args.UserName, args.UserNameKana, args.UserTel, args.UserMail, string.Empty, contents);

                // ZipFile読み込み
                // データがあれば取り込み：Zip圧縮されたものをBase64エンコードされて渡されるのでデコードしてファイルを復元、物理ファイルを保存するところまでを同期処理して返す
                if (inquiryNo > long.MinValue)
                {
                    result.InquiryNo = inquiryNo.ToString();
                    if (string.IsNullOrEmpty(args.ZipData) == false)
                    {
                        byte[] data = Convert.FromBase64String(args.ZipData);

                        if (System.IO.Directory.Exists(ZipFolderPath) == false)
                            System.IO.Directory.CreateDirectory(ZipFolderPath);

                        // Zipファイル名はオリジナルではなくお問い合わせ番号を使う
                        string fileName = string.Format("{0}.zip", result.InquiryNo);

                        string filePath = System.IO.Path.Combine(ZipFolderPath, fileName);
                        // Zipファイルを出力
                        System.IO.File.WriteAllBytes(filePath, data);

                        // 以後の処理は終了を待たない
                        Task.Run(() =>
                        {
                            ImportData(filePath, result.InquiryNo, args.ExecuteSystemType, accountKey.ToApiGuidString());
                        });
                    }

                    // メールアドレスの登録がある場合
                    if (!string.IsNullOrWhiteSpace(args.UserMail))
                    {
                        // ユーザへの お問い合わせ受理メール を送信（終了を待たない）
                        SupportWorker.SendSupportAcceptanceMail(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None), args.UserMail, inquiryNo.ToString(), args.Contents);
                    }

                    // サポート窓口への お問い合わせメール を送信（終了を待たない）
                    SupportWorker.SendSupportMail(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None), inquiryNo.ToString());
                    result.IsSuccess = bool.TrueString;
                }
                else
                    result.IsSuccess = bool.FalseString;
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
            }

            return result;
        }
    }
 

}