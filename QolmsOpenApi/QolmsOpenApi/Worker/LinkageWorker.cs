using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System.Text.RegularExpressions;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 連携関連の処理
    /// </summary>
    public sealed class LinkageWorker
    {
       

        #region "Private Method"


        /// <summary>
        /// Executor を暗号化して返す
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        private static string GetEncryptExecutor(string executor)
        {
            string encExeCutor = executor;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                try
                {
                    encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
                }
                catch (Exception) { }
            }
            return encExeCutor;
        }

        //自動でUserIDを生成する場合のプレフィックスを取得する。現状TISだけ。
        private static string GetAutoUserIdPrefix(int linkageSystemNo)
        {
            switch (linkageSystemNo)
            {
                case QoLinkage.TIS_LINKAGE_SYSTEM_NO:
                    return "TS";
                default:
                    return "QQ";
            }
        }
        //新規登録と連携登録をする
        [Obsolete("廃止予定です。代わりにLinkageRegisterWorker.RegisterNewUserを使用してください。")]
        private static QoLinkageUserRegisterApiResults NewUserRegister(QoLinkageUserRegisterApiArgs args, int retryCnt)
        {
            QoLinkageUserRegisterApiResults result = new QoLinkageUserRegisterApiResults() { IsSuccess = bool.FalseString };
            var newLinkageSystemID = Guid.NewGuid();
            string userid = args.UserId;
            string prefix = GetAutoUserIdPrefix(args.LinkageSystemNo.TryToValueType(0));
            if (string.IsNullOrWhiteSpace(args.UserId) && !string.IsNullOrEmpty(prefix))
                userid = string.Format("{0}{1}", prefix, Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());

            //退会後既定時間内の新規登録の場合エラー返却
            if (int.TryParse(args.LinkageSystemId, out int linkageSystemId) && (args.FacilityKeyReference != null && args.FacilityKeyReference.ToDecrypedReference<Guid>() != Guid.Empty) && linkageSystemId < 99999000)
            {
                var readerArgs = new LinkageUpdatedReaderArgs() { FacilityKey = args.FacilityKeyReference.ToDecrypedReference<Guid>(), LinkageSystemId = linkageSystemId.ToString() };
                var readerResults = QsDbManager.Read(new LinkageUpdatedReader(), readerArgs);

                //規定時間内かチェック
                if (readerResults.IsSuccess && readerResults.UpdatedDate.AddMinutes((double)QoApiConfiguration.NewUserRegisterInterval) > DateTime.Now)
                {
                    result.Result = new QoApiResultItem() {Code = 2999.ToString("d4"), Detail = QoApiConfiguration.NewUserRegisterInterval.ToString() };
                    return result;
                }

            }

            var identityResult = QoIdentityClient.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, newLinkageSystemID.ToString("N"),
                userid, args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana,
                args.Sex, args.Birthday, args.Mail);
            if (identityResult.IsSuccess.TryToValueType(false) && identityResult.AccountKey.TryToValueType(Guid.Empty)!=Guid.Empty )
            {
                result.IsSuccess = bool.TrueString;
                result.UserId = userid;
                result.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(GetEncryptExecutor(args.Executor), identityResult.AccountKey.TryToValueType(Guid.Empty));
                result.LinkageIdReference = newLinkageSystemID.ToEncrypedReference();
            }
            else
            {
                //error
                if (identityResult.ErrorList.Contains("UserId"))                    // ユーザーID重複
                {
                    if (string.IsNullOrWhiteSpace(args.UserId) && retryCnt < 5)//自動ID附番の場合はRetry
                    {
                        retryCnt++;
                        return NewUserRegister(args, retryCnt);
                    }
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UserIdDuplicate);
                }
                else                    // 登録失敗
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, string.Join(",", identityResult.ErrorList));
            }
            return result;
         }

        //FacilityKey から、LinkageSystemマスタ使って LinkageSystemNoを取得する
        internal static int GetLinkageNo(Guid facilitykey)
        {
            //FacilityKey から、LinkageSystemマスタ使って LinkageSystemNoを取得する
            var readerArgs = new LinkageSystemMasterReaderArgs() { FacilityKey = facilitykey };
            var readerResults = QsDbManager.Read(new LinkageSystemMasterReader(), readerArgs);
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                return readerResults.Result.First().LINKAGESYSTEMNO;
            else
                QoAccessLog.WriteInfoLog(string.Format("GetLinkageNo Error :facilitykey={0}", facilitykey));
            return int.MinValue;
        }

        //診察券とLinkage登録
        private static string WriteLinkagePatientCard(
            Guid authorKey, Guid actorkey, int linkageSystemNo, Guid facilityKey, string patientId, int Sequence ,bool delteFlag)
        {
            var writerArgs = new LinkagePatientCardWriterArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorkey,
                CardCode = linkageSystemNo,
                Sequence = Sequence,
                FacilityKey = facilityKey,
                PatientCardSet = new QsJsonSerializer().Serialize(new QhPatientCardSetOfJson() { CardNo = patientId, FacilityKey = facilityKey.ToString() }),
                DeleteFlag = delteFlag ,
                StatusType = 2
            };
            var writerResults = QsDbManager.Write(new LinkagePatientCardWriter(), writerArgs);
            if (writerResults != null && !string.IsNullOrWhiteSpace(writerResults.ErrorMessage))
                return writerResults.ErrorMessage;
            else
                return string.Empty;
        }

        //患者属性が登録されているユーザ属性と一致しているかチェック
        private static bool CheckUser(QoLinkagePatientCardAddApiArgs args)
        {
            var readerArgs = new QhAccountIndexEntityReaderArgs() { Data = new List<QH_ACCOUNTINDEX_DAT>() { new QH_ACCOUNTINDEX_DAT() { ACCOUNTKEY = args.ActorKey.TryToValueType(Guid.Empty) } } };
            var readerResults = QsDbManager.Read(new QhAccountIndexEntityReader(), readerArgs);
            if(readerResults .IsSuccess && readerResults.Result !=null && readerResults.Result.Count == 1)
            {
                var entity = readerResults.Result.First();
                string familyName=string.Empty ;
                string givenName=string.Empty ;
                string familyNameKana=string.Empty ;
                string givenNameKana=string.Empty ;
                byte sex = entity.SEXTYPE;
                DateTime birthday = entity.BIRTHDAY;
                using(var crypt=new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    try
                    {
                        familyName = crypt.DecryptString(entity.FAMILYNAME);
                        givenName = crypt.DecryptString(entity.GIVENNAME);
                        familyNameKana = crypt.DecryptString(entity.FAMILYKANANAME);
                        givenNameKana = crypt.DecryptString(entity.GIVENKANANAME);
                    }
                    catch (Exception ex)
                    {
                        QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty ));
                    }
                }
                //if(args.FamilyName !=familyName || args.GivenName !=givenName)
                //    QoAccessLog.WriteErrorLog(string.Format("患者名が一致しません:{0}{1}!={2}{3}",args.FamilyName,args.GivenName,familyName,givenName),args.Executor.TryToValueType(Guid.Empty));
                //else if(args.FamilyKanaName !=familyNameKana || args.GivenKanaName !=givenNameKana)
                //    QoAccessLog.WriteErrorLog(string.Format("患者カナ名が一致しません:{0}{1}!={2}{3}", args.FamilyKanaName, args.GivenKanaName, familyNameKana, givenNameKana), args.Executor.TryToValueType(Guid.Empty));
                if( args.Birthday.TryToValueType(DateTime.MinValue ).Date !=birthday.Date )
                    QoAccessLog.WriteErrorLog(string.Format("患者生年月日が一致しません:{0}!={1}", args.Birthday.TryToValueType(DateTime.MinValue), birthday.Date), args.Executor.TryToValueType(Guid.Empty));
                else if ((byte)args.SexType.TryToValueType(QsApiSexTypeEnum.None) != sex)
                    QoAccessLog.WriteErrorLog(string.Format("患者性別が一致しません:{0}!={1}", (byte)args.SexType.TryToValueType(QsApiSexTypeEnum.None), sex), args.Executor.TryToValueType(Guid.Empty));
                else
                    return true;
            }
            return false;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// 診察券登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。代わりにLinkagePatientCardAddWorkerを使用してください。")]
        public static QoLinkagePatientCardAddApiResults PatientCardAdd(QoLinkagePatientCardAddApiArgs args)
        {
            var result = new QoLinkagePatientCardAddApiResults() { IsSuccess = bool.FalseString };
            if (args.AuthorKey.TryToValueType(Guid.Empty) == Guid.Empty || args.ActorKey.TryToValueType(Guid.Empty) == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Keyが不明です");
                return result;
            }
            Guid facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,"FacilityKeyが不明です");
                return result;
            }
            int linkageSytemNo = GetLinkageNo(facilityKey);
            if (linkageSytemNo < 0)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,"不明なFacilityです");
                return result;
            }
            if (CheckUser(args)==false)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ユーザ属性が一致しません");
                return result;
            }
            //登録実行
            var errorMessage = WriteLinkagePatientCard(args.AuthorKey.TryToValueType(Guid.Empty),
                args.ActorKey.TryToValueType(Guid.Empty), linkageSytemNo, facilityKey,args.LinkUserId,int.MinValue ,false);
            if (string.IsNullOrWhiteSpace(errorMessage))
                result.IsSuccess = bool.TrueString;
            else
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError,errorMessage);

            return result;
        }

        /// <summary>
        /// 連携ありアカウント本登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        [Obsolete("廃止予定です。代わりにLinkageRegisterWorker.RegisterNewUserを使用してください。")]
        public static QoLinkageUserRegisterApiResults UserRegister(QoLinkageUserRegisterApiArgs args)
        {
            if(args.LinkageSystemNo.TryToValueType(int.MinValue)<0)
                throw new ArgumentNullException("LinkageSystemNo", "LinkageSystemNo情報が不正です。");
            if (string.IsNullOrEmpty(args.FamilyName) || string.IsNullOrEmpty(args.GivenName) || string.IsNullOrEmpty(args.GivenNameKana) || string.IsNullOrWhiteSpace(args.FamilyNameKana)
                || string.IsNullOrEmpty(args.Password) || string.IsNullOrEmpty(args.Sex) || string.IsNullOrEmpty(args.Birthday) || string.IsNullOrWhiteSpace(args.Password ))
                throw new ArgumentException("args", "属性情報が不足しています。");
            QoLinkageUserRegisterApiResults result = new QoLinkageUserRegisterApiResults() { IsSuccess = bool.FalseString };
            List<string> errorList = new List<string>();

            //if (args.AccountKeyReference.TryToValueType(Guid.Empty) == Guid.Empty)
            //{
                //新規登録
                result = NewUserRegister(args,0);
            //}
            //else
            //{
            //    //連携のみ登録
            //    result = UserLinkageRegister(args);
            //}
            
            return result;
        }

        /// <summary>
        /// 診察券削除を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。代わりにLinkagePatientCardDeleteWorkerを使用してください。")]
        public static QoLinkagePatientCardDeleteApiResults PatientCardDelete(QoLinkagePatientCardDeleteApiArgs args)
        {
            var result = new QoLinkagePatientCardDeleteApiResults() { IsSuccess = bool.FalseString };
            Guid facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty || args.ActorKey.TryToValueType(Guid.Empty) == Guid.Empty || args.ActorKey.TryToValueType(Guid.Empty) == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FacilityKeyが不明です");
                return result;
            }
            int linkageSytemNo = GetLinkageNo(facilityKey);
            if (linkageSytemNo < 0)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "不明なFacilityです");
                return result;
            }
            
            //削除実行
            var errorMessage = WriteLinkagePatientCard(args.AuthorKey.TryToValueType(Guid.Empty),
                args.ActorKey.TryToValueType(Guid.Empty), linkageSytemNo, facilityKey, string.Empty,1,true);
            if (string.IsNullOrWhiteSpace(errorMessage))
                result.IsSuccess = bool.TrueString;
            else
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, errorMessage);

            return result;
        }

        /// <summary>
        /// LinkageSystemId の取得を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoLinkageLinkageSystemIdReadApiResults LinkageSystemIdRead(QoLinkageLinkageSystemIdReadApiArgs args)
        {
            var result = new QoLinkageLinkageSystemIdReadApiResults() { IsSuccess = bool.FalseString };

            // パラメータチェック
            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AccountKeyが不明です");
                return result;
            }

            // 取得
            var readerArgs = new LinkageReaderArgs() { 
                AccountKey = accountKey, 
                LinkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue), 
                StatusType = args.StatusType.TryToValueType(byte.MinValue)
            };
            var linkageSystemId = QsDbManager.Read(new LinkageReader(), readerArgs).LinkageSystemId;

            if (linkageSystemId != null)
            {
                result.IsSuccess = bool.TrueString;
                result.LinkageSystemId = linkageSystemId;
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, $"LinkageSystemId の取得に失敗しました。AccountKey = {readerArgs.AccountKey} LinkageSystemNo = {readerArgs.LinkageSystemNo} StatusType = {readerArgs.StatusType} ");
            }

            return result;
        }

        #endregion
    }
}