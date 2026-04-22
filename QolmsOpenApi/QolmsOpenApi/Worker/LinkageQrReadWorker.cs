using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using SPCryptoEngine;
using MEI.TwoinQRLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 連携ユーザー / 診察券 登録用QRデータの処理を行います。
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkageQrReadWorker
    {
        static readonly char[] Separator = new[] { '#' };

        IFacilityRepository _facilityRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facilityRepository"></param>
        public LinkageQrReadWorker(IFacilityRepository facilityRepository)
        {
            _facilityRepo = facilityRepository;
        }
        
        /// <summary>
        /// Qrデータを読み込み解析する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkageQrCodeReadApiResults QrRead(QoLinkageQrCodeReadApiArgs args)
        {
            var result = new QoLinkageQrCodeReadApiResults() 
            {
                IsSuccess = bool.FalseString 
            };

            // QR必須チェック
            if(!args.QRData.CheckArgsRequired(nameof(args.QRData), result))
            {
                return result;
            }

            // バージョン変換チェック
            if(!args.Version.CheckArgsConvert(nameof(args.Version),0, result, out var version))
            {
                return result;
            }               

            // QR復号化
            if(!TryDecrypt(args.QRData,args.ExecuteSystemType,result, out var qr))
            {
                // 失敗したらエラー
                return result;
            }
            QoAccessLog.WriteInfoLog(qr);
            // 復号化した結果が空ならエラー
            if (string.IsNullOrEmpty(qr))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "復号化した文字列が空です。");
                return result;
            }

            var fields = qr.Split(Separator);

            // 要素数が不足していればエラー
            if(version <= 1 && fields.Length < 8)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "Version01のQR情報が欠落しています。");
                return result;
            }
            if(version >= 2 && fields.Length < 10)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "Version02以降のQR情報が欠落しています。");
                return result;
            }

            // 生年月日 日付変換
            if(!DateTime.TryParseExact(fields[0].Trim(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime birthday))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "生年月日が不正です。");
                return result;
            }

            // 作成日 日付変換
            if(!DateTime.TryParseExact(fields[1].Trim(), "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime createdDate))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "作成日が不正です。");
                return result;
            }

            result.Birthday = birthday.ToApiDateString();
            result.CreatedDate = createdDate.ToApiDateString();
            result.LinkUserId = fields[2].Trim();

            

            // 性別変換チェック
            if (!TryGetQolmsSexType(fields[3].Trim(), out var qolmsSexType))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "性別が不正です。");
                return result;
            }

            result.SexType = qolmsSexType;
            result.MedicalFacilityCode = fields[4].Trim();
            
            // 医療機関コードから施設情報を取得できなければエラー
            if (!TryGetFacilityKeyReference(result.MedicalFacilityCode, result, out var facilityKeyRef, out var facilityName))
            {
                return result;
            }

            result.FacilityKeyReference = facilityKeyRef;
            result.FacilityName = facilityName;

            // 漢字姓名を分解
            var namesp = fields[5].Split(new[] { '　' }, StringSplitOptions.RemoveEmptyEntries);
            if (namesp.Length > 1)
            {
                result.FamilyName = namesp[0].Trim();
                result.GivenName = namesp[1].Trim();
            }
            else    
            {
                //区切られてなかったら全部FamilyNameに入れる
                result.FamilyName = fields[5].Trim();
            }
            // カナ姓名 それぞれ半角なら全角に変換
            result.FamilyKanaName = Microsoft.VisualBasic.Strings.StrConv(fields[6].Trim(), Microsoft.VisualBasic.VbStrConv.Wide, 0x411);
            result.GivenKanaName = Microsoft.VisualBasic.Strings.StrConv(fields[7].Trim(), Microsoft.VisualBasic.VbStrConv.Wide, 0x411);

            // SystemType事のQR有効期限設定を取得
            var qrExpirationConfig = args.ExecuteSystemType.GetQrExpirationHour();
            // Version1は受付票用固定
            var expirationHour = qrExpirationConfig.ReceptionExpirationHour;

            if(version >= 2)
            {
                // Version2以降に出力識別コードがある
                var outputTypeCode = fields[8].Trim().TryToValueType(int.MinValue);
                if(outputTypeCode == int.MinValue || outputTypeCode >= 2)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "識別コードが不正です。");
                    return result;
                }

                // 有効期限 識別コード0:受付票なら受付票用の期限、1:予約票なら予約票の期限をセット
                expirationHour = outputTypeCode == 0 ? 
                    qrExpirationConfig.ReceptionExpirationHour : 
                    qrExpirationConfig.ReservationExpirationHour;

                // Version2以降は最後のフィールドに電話番号が「｜」区切りで入っている
                var phoneNumbers = fields[9].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var number in phoneNumbers)
                {
                    result.PhoneNumbers.Add(number);
                }
            }

            if (!result.LinkUserId.StartsWith("99999"))
            {
                if (!CheckExpiration(createdDate, expirationHour))
                {
                    // 有効期限エラー
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountRegisterExpired, "QRコードの有効期限が切れています。");
                    return result;
                }
            }

            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            result.IsSuccess = bool.TrueString;            

            return result;
        }

        // 医療機関コードから施設キー参照を取得
        bool TryGetFacilityKeyReference(string medicalFacilityCode, QoApiResultsBase result, out string facilityKeyRef, out string facilityName)
        {
            facilityKeyRef = string.Empty;
            facilityName = string.Empty;
            try
            {
                var entity = _facilityRepo.ReadFacilityFromMedicalFacilityCode(medicalFacilityCode);

                facilityKeyRef = entity.FACILITYKEY.ToEncrypedReference();
                facilityName = entity.FACILITYNAME;
                return true;
            }
            catch(Exception)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "医療機関コードから施設情報を取得できませんでした。");
                return false;
            }
        }

        //SSI 復号
        bool TryDecrypt(string qrData, string systemType, QoApiResultsBase result, out string decoded)
        {
            decoded = string.Empty;
            try
            {
                var appType = systemType.ToApplicationType();
                switch (appType)
                {
                    case QsDbApplicationTypeEnum.QolmsTisApp:
                    case QsDbApplicationTypeEnum.QolmsNaviApp:
                        decoded = DecryptForSSI(qrData);
                        break;
                    case QsDbApplicationTypeEnum.MeiNaviApp:
                        decoded = DecryptForMEI(qrData);
                        break;
                    default:
                        decoded = DecryptForSSI(qrData);
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"暗号化されたQrコードの復号に失敗しました。");
                return false;                
            }
        }

        private string DecryptForSSI(string body)
        {
            var generator = new KeyGenerator();
            return generator.GetDecryptoKey(body);
        }

        private string DecryptForMEI(string body)
        {
            return TwoinQREncrypt.Decrypt(body);
        }

        //SSIの性別をQOLMSの性別に変換
        private static bool TryGetQolmsSexType(string ssiSexType, out string qolmsType)
        {
            qolmsType = "0";

            switch (ssiSexType.Trim())
            {
                case "1":
                    qolmsType = "1";
                    return true;
                case "3":
                    qolmsType = "2";
                    return true;

                default:
                    return false;
            }
        }


        // 有効期限をチェックします。
        private static bool CheckExpiration(DateTime createdDate, int expirationHour)
        {            
            if (createdDate.AddHours(expirationHour) >= DateTime.Now)
            {
                return true;
            }

            return false;
        }
    }
}