using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 健診取り込み関連の機能を提供します。
    /// </summary>
    public class CheckupFileWorker
    {


        /// <summary>
        /// 介護情報ファイル名 の定義名を表します。
        /// </summary>
        public const string CHECKUPCAREFILENAME_KIROKU = "記録.csv";
        /// <summary>
        /// 介護情報ファイル名 の定義名を表します。
        /// </summary>
        public const string CHECKUPCAREFILENAME_RIYOUSHA = "利用者情報.csv";
        /// <summary>
        /// 介護情報ファイル名 の定義名を表します。
        /// </summary>
        public const string CHECKUPCAREFILENAME_SCHEDULE = "月間スケジュール.csv";


        #region "Private Method"
        private static int ReadSequence(CheckupFileSequenceReaderArgs readerArgs)
        {
            QoAccessLog.WriteErrorLog("ReadSequence", Guid.Empty);
            var reader = new CheckupFileSequenceReader();
            var readerResults = QsDbManager.Read(reader, readerArgs);
            if (readerResults.IsSuccess)
            {
                return readerResults.Sequence + 1;
            }
            return int.MinValue;
        }

        private static QhCheckupFrailtyFileBlobEntity ConvertFrailtyBlobEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, string data, int sequence, string serviceCode, string organizationId)
        {
            var result = new QhCheckupFrailtyFileBlobEntity();
            try
            {
                result.Name = Guid.Empty;
                result.ContentType = "text/plain";
                result.ContentEncoding = "UTF-8";
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    result.Data = Encoding.UTF8.GetBytes(crypt.EncryptString(data));
                }
                result.LinkageSystemNo = linkageSystemNo.ToString();
                result.LinkageSystemId = linkageSystemId;
                result.ServiceCode = serviceCode;
                result.EffectiveDate = effectivedate.ToString("yyyyMMdd");
                result.OrganizationId = organizationId;
                result.Sequence = sequence.ToString();
                result.OriginalName = originalName;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertFrailtyBlobEntityが失敗しました。");
            }

            return result;
        }

        private static QH_CHECKUPFRAILTYFILE_DAT ConvertFrailtyDbEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, Guid fileKey, int sequence, string serviceCode, string organizationId)
        {
            var result = new QH_CHECKUPFRAILTYFILE_DAT();

            try
            {
                result.FILEKEY = fileKey;
                result.SEQUENCE = sequence;
                result.LINKAGESYSTEMNO = linkageSystemNo;
                result.LINKAGESYSTEMID = linkageSystemId;
                result.SERVICECODE = serviceCode;
                result.EFFECTIVEDATE = effectivedate;
                result.ORGANIZATIONID = organizationId;
                result.STATUSNO = (byte)QsDbCheckupCdaStatusNoEnum.SavedBlob;
                result.ORIGINALNAME = originalName;
                result.CONTENTTYPE = "text/csv";
                result.CONTENTENCODING = "UTF-8";
                result.HASHVALUE = string.Empty;
                result.DELETEFLAG = false;
                result.CREATEDDATE = DateTime.Now;
                result.UPDATEDDATE = DateTime.Now;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertFrailtyDbEntityが失敗しました。");
            }

            return result;
        }

        private static QhCheckupCareFileBlobEntity ConvertCareBlobEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, string data, int sequence, string serviceCode, string organizationId)
        {
            var result = new QhCheckupCareFileBlobEntity();
            try
            {
                result.Name = Guid.Empty;
                result.ContentType = "text/plain";
                result.ContentEncoding = "UTF-8";
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    result.Data = Encoding.UTF8.GetBytes(crypt.EncryptString(data));
                }
                result.LinkageSystemNo = linkageSystemNo.ToString();
                result.LinkageSystemId = linkageSystemId;
                result.ServiceCode = serviceCode;
                result.EffectiveDate = effectivedate.ToString("yyyyMMdd");
                result.OrganizationId = organizationId;
                result.Sequence = sequence.ToString();
                result.OriginalName = originalName;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertCareBlobEntityが失敗しました。");
            }

            return result;
        }

        private static QH_CHECKUPCAREFILE_DAT ConvertCareDbEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, Guid fileKey, int sequence)
        {
            var result = new QH_CHECKUPCAREFILE_DAT();

            try
            {
                result.FILEKEY = fileKey;
                result.SEQUENCE = sequence;
                result.LINKAGESYSTEMNO = linkageSystemNo;
                result.LINKAGESYSTEMID = linkageSystemId;
                result.SERVICECODE = string.Empty;
                result.EFFECTIVEDATE = effectivedate;
                result.ORGANIZATIONID = string.Empty;
                result.STATUSNO = (byte)QsDbCheckupCdaStatusNoEnum.SavedBlob;
                result.ORIGINALNAME = originalName;
                result.CONTENTTYPE = "text/csv";
                result.CONTENTENCODING = "UTF-8";
                result.HASHVALUE = string.Empty;
                result.DELETEFLAG = false;
                result.CREATEDDATE = DateTime.Now;
                result.UPDATEDDATE = DateTime.Now;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertCareDbEntityが失敗しました。");
            }

            return result;
        }

        private static QhCheckupAfterSchoolFileBlobEntity ConvertAfterSchoolBlobEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, string data, int sequence, string serviceCode, string organizationId)
        {
            var result = new QhCheckupAfterSchoolFileBlobEntity();
            try
            {
                result.Name = Guid.Empty;
                result.ContentType = "text/plain";
                result.ContentEncoding = "UTF-8";
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    result.Data = Encoding.UTF8.GetBytes(crypt.EncryptString(data));
                }
                result.LinkageSystemNo = linkageSystemNo.ToString();
                result.LinkageSystemId = linkageSystemId;
                result.ServiceCode = serviceCode;
                result.EffectiveDate = effectivedate.ToString("yyyyMMdd");
                result.OrganizationId = organizationId;
                result.Sequence = sequence.ToString();
                result.OriginalName = originalName;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertAfterSchoolBlobEntityが失敗しました。");
            }

            return result;
        }

        private static QH_CHECKUPAFTERSCHOOLFILE_DAT ConvertAfterSchoolDbEntity(int linkageSystemNo, string originalName, string linkageSystemId, DateTime effectivedate, Guid fileKey, int sequence, string serviceCode, string organizationId)
        {
            var result = new QH_CHECKUPAFTERSCHOOLFILE_DAT();

            try
            {
                result.FILEKEY = fileKey;
                result.SEQUENCE = sequence;
                result.LINKAGESYSTEMNO = linkageSystemNo;
                result.LINKAGESYSTEMID = linkageSystemId;
                result.SERVICECODE = serviceCode;
                result.EFFECTIVEDATE = effectivedate;
                result.ORGANIZATIONID = organizationId;
                result.STATUSNO = (byte)QsDbCheckupCdaStatusNoEnum.SavedBlob;
                result.ORIGINALNAME = originalName;
                result.CONTENTTYPE = "text/csv";
                result.CONTENTENCODING = "UTF-8";
                result.HASHVALUE = string.Empty;
                result.DELETEFLAG = false;
                result.CREATEDDATE = DateTime.Now;
                result.UPDATEDDATE = DateTime.Now;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                throw new Exception("ConvertAfterSchoolDbEntityが失敗しました。");
            }

            return result;
        }

        private static bool ReadFrailtyCSV(ZipArchiveEntry entry, int linkageSystemNo, QsApiSystemTypeEnum systemType)
        {

            var fileName = entry.Name;
            QoAccessLog.WriteErrorLog(fileName, Guid.Empty);
            //ファイル解析
            //解凍後ファイルをStreamとして読み込む
            using (Stream stream = entry.Open())
            {
                //読み込んだStreamのコピー先MemoryStreamを作成する
                using (var entriems = new MemoryStream())
                {
                    //コピー
                    stream.CopyTo(entriems);
                    entriems.Position = 0;

                    // テキストエンコーディングにUTF-8を用いてentriemsの読み込みを行うStreamReaderを作成
                    using (var reader = new StreamReader(entriems, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        // ReadLineメソッドで1行の文字列データを読み込む
                        line = reader.ReadLine();
                        var header = line.Split(',');
                        
                        //想定されたheader行かチェック
                        if (!(header.Length == 330 && header[0] == "id"))
                        {
                            throw new Exception("想定されたヘッダー行が含まれていません。");
                        }
                        //データ行読込
                        while (reader.Peek() >= 0)
                        {
                            // ReadLineメソッドで1行の文字列データを読み込む
                            line = reader.ReadLine();
                            var data = line.Split(',');
                            var linkageSystemId = $"{data[2]}";
                            var effectiveDate = DateTime.Parse($"{data[3]}");
                            var id = $"{data[0]}";
                            var promoterCd = $"{data[1]}";

                            var args = new CheckupFileSequenceReaderArgs()
                            {
                                LinkageSystemNo = linkageSystemNo,
                                LinkageSystemId = linkageSystemId,
                                ServiceCode = string.Empty,
                                OrganizationId = promoterCd,
                                EffectiveDate = effectiveDate,
                                SystemType = systemType
                            };
                            var sequence = ReadSequence(args);
                            if (sequence == int.MinValue)
                            {
                                throw new Exception("sequenceが正しく取得できませんでした。");
                            }
                            QoAccessLog.WriteErrorLog(sequence.ToString(), Guid.Empty);
                            //Blobに登録
                            var storageArgs = new CheckupFrailtyFileBlobEntityWriterArgs<QhCheckupFrailtyFileBlobEntity>
                            {
                                Entity = ConvertFrailtyBlobEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, line, sequence, id, promoterCd)
                            };


                            var storageWriter = new CheckupFrailtyFileBlobEntityWriter<QhCheckupFrailtyFileBlobEntity>();
                            var storageResults = storageWriter.Execute(storageArgs);

                            if (!storageResults.IsSuccess)
                            {
                                throw new Exception("フレイル健診データのBlobStorageへの登録に失敗しました。");
                            }


                            //DBに登録
                            var writer = new CheckupFrailtyFileWriter();
                            var writerArgs = new CheckupFrailtyFileWriterArgs()
                            {
                                Entity = ConvertFrailtyDbEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, storageResults.Result, sequence, string.Empty, promoterCd)
                            };
                            var writerResults = QsDbManager.Write(writer, writerArgs);

                            if (!writerResults.IsSuccess)
                            {
                                throw new Exception("フレイル健診データのDBへの登録に失敗しました。");
                            }

                        }
                    }

                }
            }
            return true;
        }

        private static bool ReadCareKIROKUCSV(ZipArchiveEntry entry, int linkageSystemNo, QsApiSystemTypeEnum systemType)
        {
            //想定されたファイル名かチェック
            var fileName = entry.Name;

            //ファイル解析
            //解凍後ファイルをStreamとして読み込む
            using (Stream stream = entry.Open())
            {
                //読み込んだStreamのコピー先MemoryStreamを作成する
                using (var entriems = new MemoryStream())
                {
                    //コピー
                    stream.CopyTo(entriems);
                    entriems.Position = 0;

                    // テキストエンコーディングにUTF-8を用いてentriemsの読み込みを行うStreamReaderを作成
                    using (var reader = new StreamReader(entriems, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        // ReadLineメソッドで1行の文字列データを読み込む
                        line = reader.ReadLine();
                        
                        var header = line.Split(',');
                        QoAccessLog.WriteErrorLog(header[0], Guid.Empty);
                        //想定されたheader行かチェック
                        if (!(header.Length > 0 && header[0] == "\"利用者コード\""))
                        {
                            throw new Exception("想定されたヘッダー行が含まれていません。");
                        }

                        line = reader.ReadToEnd();
                        var slice = line.Split(new string[] { "\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"\r\n" }, StringSplitOptions.None);
                        //データ行読込
                        foreach (var block in slice)
                        {
                            var data = block.Split(',');
                            var linkageSystemId = $"{data[0].Replace("\"", "")}";
                            var serviceCode = $"{999}";
                            var organizationId = $"{linkageSystemNo}";
                            var effectiveDate = DateTime.Parse($"{data[2].Replace("\"", "")}");

                            var args = new CheckupFileSequenceReaderArgs()
                            {
                                LinkageSystemNo = linkageSystemNo,
                                LinkageSystemId = linkageSystemId,
                                ServiceCode = serviceCode,
                                OrganizationId = organizationId,
                                EffectiveDate = effectiveDate,
                                SystemType = systemType
                            };
                            var sequence = ReadSequence(args);
                            if (sequence == int.MinValue)
                            {
                                throw new Exception("sequenceが正しく取得できませんでした。");
                            }

                            //Blobに登録
                            var storageArgs = new CheckupCareFileBlobEntityWriterArgs<QhCheckupCareFileBlobEntity>
                            {
                                Entity = ConvertCareBlobEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, block.Replace("\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"", ""), sequence, serviceCode, organizationId)
                            };


                            var storageWriter = new CheckupCareFileBlobEntityWriter<QhCheckupCareFileBlobEntity>();
                            var storageResults = storageWriter.Execute(storageArgs);

                            if (!storageResults.IsSuccess)
                            {
                                throw new Exception("介護情報データのBlobStorageへの登録に失敗しました。");
                            }

                            //DBに登録
                            var writer = new CheckupCareFileWriter();
                            var writerArgs = new CheckupCareFileWriterArgs()
                            {
                                Entity = ConvertCareDbEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, storageResults.Result, sequence)
                            };
                            var writerResults = QsDbManager.Write(writer, writerArgs);

                            if (!writerResults.IsSuccess)
                            {
                                throw new Exception("介護情報データのDBへの登録に失敗しました。");
                            }

                        }
                    }

                }
            }
            return true;
        }

        private static bool ReadCareRIYOUSHACSV(ZipArchiveEntry entry, int linkageSystemNo, QsApiSystemTypeEnum systemType)
        {
            //想定されたファイル名かチェック
            var fileName = entry.Name;

            //ファイル解析
            //解凍後ファイルをStreamとして読み込む
            using (Stream stream = entry.Open())
            {
                //読み込んだStreamのコピー先MemoryStreamを作成する
                using (var entriems = new MemoryStream())
                {
                    //コピー
                    stream.CopyTo(entriems);
                    entriems.Position = 0;

                    // テキストエンコーディングにUTF-8を用いてentriemsの読み込みを行うStreamReaderを作成
                    using (var reader = new StreamReader(entriems, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        // ReadLineメソッドで1行の文字列データを読み込む
                        line = reader.ReadLine();

                        var header = line.Split(',');
                        QoAccessLog.WriteErrorLog(header[0], Guid.Empty);
                        //想定されたheader行かチェック
                        if (!(header.Length > 0 && header[0] == "\"利用者コード\""))
                        {
                            throw new Exception("想定されたヘッダー行が含まれていません。");
                        }

                        line = reader.ReadToEnd();
                        line = line.Replace("\"\r\n\"", "\"\r\n!@\"");
                        var slice = line.Split(new string[] { "\r\n!@" }, StringSplitOptions.None);
                        //データ行読込
                        foreach (var block in slice)
                        {
                            var data = block.Split(',');
                            var linkageSystemId = $"{data[0].Replace("\"","")}";
                            var serviceCode = $"{999}";
                            var organizationId = $"{linkageSystemNo}";
                            var effectiveDate = DateTime.Now.Date;

                            var args = new CheckupFileSequenceReaderArgs()
                            {
                                LinkageSystemNo = linkageSystemNo,
                                LinkageSystemId = linkageSystemId,
                                ServiceCode = serviceCode,
                                OrganizationId = organizationId,
                                EffectiveDate = effectiveDate,
                                SystemType = systemType
                            };
                            var sequence = ReadSequence(args);
                            if (sequence == int.MinValue)
                            {
                                throw new Exception("sequenceが正しく取得できませんでした。");
                            }

                            //Blobに登録
                            var storageArgs = new CheckupCareFileBlobEntityWriterArgs<QhCheckupCareFileBlobEntity>
                            {
                                Entity = ConvertCareBlobEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, block, sequence, serviceCode, organizationId)
                            };


                            var storageWriter = new CheckupCareFileBlobEntityWriter<QhCheckupCareFileBlobEntity>();
                            var storageResults = storageWriter.Execute(storageArgs);

                            if (!storageResults.IsSuccess)
                            {
                                throw new Exception("介護情報データのBlobStorageへの登録に失敗しました。");
                            }


                            //DBに登録
                            var writer = new CheckupCareFileWriter();
                            var writerArgs = new CheckupCareFileWriterArgs()
                            {
                                Entity = ConvertCareDbEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, storageResults.Result, sequence)
                            };
                            var writerResults = QsDbManager.Write(writer, writerArgs);

                            if (!writerResults.IsSuccess)
                            {
                                throw new Exception("介護情報データのDBへの登録に失敗しました。");
                            }

                        }
                    }

                }
            }
            return true;
        }

        private static bool ReadAfterSchoolCSV(ZipArchiveEntry entry, int linkageSystemNo, QsApiSystemTypeEnum systemType, DateTime effectiveDate, string linkageSystemId)
        {

            var fileName = entry.Name;
            //想定されたファイル名かチェック
            //if (fileName != "kiroku.csv" || fileName != "riyousha.csv")
            //{
            //    throw new Exception("想定されたファイル名ではありません。");
            //}

            //ファイル解析
            //解凍後ファイルをStreamとして読み込む
            using (Stream stream = entry.Open())
            {
                //読み込んだStreamのコピー先MemoryStreamを作成する
                using (var entriems = new MemoryStream())
                {
                    //コピー
                    stream.CopyTo(entriems);
                    entriems.Position = 0;

                    // テキストエンコーディングにUTF-8を用いてentriemsの読み込みを行うStreamReaderを作成
                    using (var reader = new StreamReader(entriems, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        // ReadLineメソッドで1行の文字列データを読み込む
                        line = reader.ReadLine();
                        var header = line.Split(',');
                        //想定されたheader行かチェック
                        if (!(header.Length > 0 && header[0] == "学校名"))
                        {
                            throw new Exception("想定されたヘッダー行が含まれていません。");
                        }

                        //データ行読込
                        // ReadToEndメソッドで文字列データを読み込む
                        line = reader.ReadToEnd();
                        //var linkageSystemId = $"{data[1]}";
                        var serviceCode = $"{999}";
                        var organizationId = $"{linkageSystemNo}";


                        var args = new CheckupFileSequenceReaderArgs()
                        {
                            LinkageSystemNo = linkageSystemNo,
                            LinkageSystemId = linkageSystemId,
                            ServiceCode = serviceCode,
                            OrganizationId = organizationId,
                            EffectiveDate = effectiveDate,
                            SystemType = systemType
                        };
                        var sequence = ReadSequence(args);
                        if (sequence == int.MinValue)
                        {
                            throw new Exception("sequenceが正しく取得できませんでした。");
                        }

                        //Blobに登録
                        var storageArgs = new CheckupAfterSchoolFileBlobEntityWriterArgs<QhCheckupAfterSchoolFileBlobEntity>
                        {
                            Entity = ConvertAfterSchoolBlobEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, line, sequence, serviceCode, organizationId)
                        };


                        var storageWriter = new CheckupAfterSchoolFileBlobEntityWriter<QhCheckupAfterSchoolFileBlobEntity>();
                        var storageResults = storageWriter.Execute(storageArgs);

                        if (!storageResults.IsSuccess)
                        {
                            throw new Exception("学童健診データのBlobStorageへの登録に失敗しました。");
                        }


                        //DBに登録
                        var writer = new CheckupAfterSchoolFileWriter();
                        var writerArgs = new CheckupAfterSchoolFileWriterArgs()
                        {
                            Entity = ConvertAfterSchoolDbEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, storageResults.Result, sequence, serviceCode, organizationId)
                        };
                        var writerResults = QsDbManager.Write(writer, writerArgs);

                        if (!writerResults.IsSuccess)
                        {
                            throw new Exception("学童健診データのDBへの登録に失敗しました。");
                        }
                    }
                }
            }
            return true;
        }

        private static bool ReadAfterSchoolOES(ZipArchiveEntry entry, int linkageSystemNo, QsApiSystemTypeEnum systemType, DateTime effectiveDate, string linkageSystemId)
        {

            var fileName = entry.Name;
            //想定されたファイル名かチェック
            //if (fileName != "kiroku.csv" || fileName != "riyousha.csv")
            //{
            //    throw new Exception("想定されたファイル名ではありません。");
            //}

            //ファイル解析
            //解凍後ファイルをStreamとして読み込む
            using (Stream stream = entry.Open())
            {
                //読み込んだStreamのコピー先MemoryStreamを作成する
                using (var entriems = new MemoryStream())
                {
                    //コピー
                    stream.CopyTo(entriems);
                    entriems.Position = 0;

                    // テキストエンコーディングにUTF-8を用いてentriemsの読み込みを行うStreamReaderを作成
                    using (var reader = new StreamReader(entriems, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        // ReadToEndメソッドで文字列データを読み込む
                        line = reader.ReadToEnd();
                        //var linkageSystemId = $"{data[1]}";
                        var serviceCode = $"{998}";
                        var organizationId = $"{linkageSystemNo}";


                        var args = new CheckupFileSequenceReaderArgs()
                        {
                            LinkageSystemNo = linkageSystemNo,
                            LinkageSystemId = linkageSystemId,
                            ServiceCode = serviceCode,
                            OrganizationId = organizationId,
                            EffectiveDate = effectiveDate,
                            SystemType = systemType
                        };
                        var sequence = ReadSequence(args);
                        if (sequence == int.MinValue)
                        {
                            throw new Exception("sequenceが正しく取得できませんでした。");
                        }

                        //Blobに登録
                        var storageArgs = new CheckupAfterSchoolFileBlobEntityWriterArgs<QhCheckupAfterSchoolFileBlobEntity>
                        {
                            Entity = ConvertAfterSchoolBlobEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, line, sequence, serviceCode, organizationId)
                        };


                        var storageWriter = new CheckupAfterSchoolFileBlobEntityWriter<QhCheckupAfterSchoolFileBlobEntity>();
                        var storageResults = storageWriter.Execute(storageArgs);

                        if (!storageResults.IsSuccess)
                        {
                            throw new Exception("学童健診データのBlobStorageへの登録に失敗しました。");
                        }


                        //DBに登録
                        var writer = new CheckupAfterSchoolFileWriter();
                        var writerArgs = new CheckupAfterSchoolFileWriterArgs()
                        {
                            Entity = ConvertAfterSchoolDbEntity(linkageSystemNo, fileName, linkageSystemId, effectiveDate, storageResults.Result, sequence, serviceCode, organizationId)
                        };
                        var writerResults = QsDbManager.Write(writer, writerArgs);

                        if (!writerResults.IsSuccess)
                        {
                            throw new Exception("学童健診データのDBへの登録に失敗しました。");
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// フレイル健診データ取り込み処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCheckupFileFrailtyUploadApiResults FrailtyUpload(QoCheckupFileFrailtyUploadApiArgs args)
        {
            var result = new QoCheckupFileFrailtyUploadApiResults() {  };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }

            var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            QoAccessLog.WriteErrorLog(systemType.ToString(), Guid.Empty);
            if (systemType.ToString() != QsApiSystemTypeEnum.Frailty.ToString())
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            try
            {
                QoAccessLog.WriteErrorLog("読込開始", Guid.Empty);
                //Zipファイル読込
                var buf = Convert.FromBase64String(args.ZipData);
                using (var ms = new MemoryStream(buf))
                {
                    using (var archive = new ZipArchive(ms))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            ReadFrailtyCSV(entry, linkageSystemNo, systemType);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return result;
        }

        /// <summary>
        /// 介護情報データ取り込み処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCheckupFileCareUploadApiResults CareUpload(QoCheckupFileCareUploadApiArgs args)
        {
            var result = new QoCheckupFileCareUploadApiResults() { };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            if (systemType.ToString() != QsApiSystemTypeEnum.Care.ToString())
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            try
            {
                //Zipファイル読込
                var buf = Convert.FromBase64String(args.ZipData);
                using (var ms = new MemoryStream(buf))
                {
                    using (var archive = new ZipArchive(ms))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            
                            switch (entry.Name)
                            {
                                case CHECKUPCAREFILENAME_RIYOUSHA:
                                    ReadCareRIYOUSHACSV(entry, linkageSystemNo, systemType);
                                    break;
                                case CHECKUPCAREFILENAME_KIROKU:
                                    ReadCareKIROKUCSV(entry, linkageSystemNo, systemType);
                                    break;
                                case CHECKUPCAREFILENAME_SCHEDULE:
                                    break;
                                default:
                                    throw new Exception("想定されたファイル名ではありません。");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return result;
        }

        /// <summary>
        /// 学童健診データ取り込み処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCheckupFileAfterSchoolUploadApiResults AfterSchoolUpload(QoCheckupFileAfterSchoolUploadApiArgs args)
        {
            var result = new QoCheckupFileAfterSchoolUploadApiResults() { };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            if (systemType.ToString() != QsApiSystemTypeEnum.AfterSchool.ToString())
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            if (!DateTime.TryParseExact(args.RecordDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime effectiveDate))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            try
            {
                //Zipファイル読込
                var buf = Convert.FromBase64String(args.ZipData);
                using (var ms = new MemoryStream(buf))
                {
                    using (var archive = new ZipArchive(ms))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            switch (Path.GetExtension(entry.Name.ToLower()))
                            {
                                case ".csv":
                                    ReadAfterSchoolCSV(entry, linkageSystemNo, systemType, effectiveDate, args.LinkageSystemId);
                                    break;
                                case ".oes":
                                    ReadAfterSchoolOES(entry, linkageSystemNo, systemType, effectiveDate, args.LinkageSystemId);
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError);
                return result;
            }
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return result;
        }

        #endregion
    }

}