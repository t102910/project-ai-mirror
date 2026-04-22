using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsCryptV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 医療ナビの初期ロードデータ取得処理
    /// </summary>
    public class AppMedicalNaviInitWorker
    {
        LinkagePatientCardListWorker _cardListWorker;
        MasterMedicalFacilityListWorker _facilityListWorker;
        UserListReadWorker _userListReadWorker;
        IFamilyRepository _familyRepo;
        IAccountRepository _accountRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facilityRepository"></param>
        /// <param name="patientCardRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="accountRepository"></param>
        public AppMedicalNaviInitWorker(
            IFacilityRepository facilityRepository, 
            IPatientCardRepository patientCardRepository,
            IFamilyRepository familyRepository, 
            IAccountRepository accountRepository)
        {
            _familyRepo = familyRepository;
            _accountRepo = accountRepository;
            _cardListWorker = new LinkagePatientCardListWorker(patientCardRepository);
            _facilityListWorker = new MasterMedicalFacilityListWorker(facilityRepository);
            _userListReadWorker = new UserListReadWorker(_familyRepo);
        }

        /// <summary>
        /// 医療ナビ初期データ取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>

        public QoAppMedicalNaviInitApiResults GetInitData(QoAppMedicalNaviInitApiArgs args)
        {
            var results = new QoAppMedicalNaviInitApiResults
            {
                IsSuccess = bool.FalseString
            };

            // メインアカウントキー取得
            var publicAccountKey = Guid.Parse(args.ActorKey);

            // Emailの取得
            if(!TryReadEmail(publicAccountKey, results, out var email))
            {
                return results;
            }

            // 本人含む家族リストを取得
            if(!TryReadFamilyList(args, results, out var userList))
            {
                return results;
            }

            

            var accountSetList = new List<QoApiAccountPatientCardSetItem>();
            foreach(var user in userList)
            {
                // 診察券の取得
                if (!TryReadPatientCards(args, user.AccountKeyReference.ToDecrypedReference<Guid>(),results,out var cardList))
                {
                    return results;
                }

                // TODO: 互換性対応(本番API/アプリ両方稼働しきったら削除する)
                var account = new QoApiAccountFamilyViewItem
                {
                    AccessKey = user.AccessKey,
                    AccountKeyReference = user.AccountKeyReference,
                    Birthday = user.Birthday,
                    FamilyName = user.FamilyName,
                    FamilyNameKana = user.FamilyNameKana,
                    GivenName = user.GivenName,
                    GivenNameKana = user.GivenNameKana,
                    PersonPhotoReference = user.PersonPhotoReference,
                    Sex = user.Sex.ToString(),
                };

                // アカウントセット情報に追加               
                accountSetList.Add(new QoApiAccountPatientCardSetItem
                {
                    Account = account,
                    User = user,
                    PatientCardItemN = cardList
                });
            }

            // 診察券として利用している施設情報の取得
            if(!TryReadFacilityList(args,accountSetList,results,out var facilityResults))
            {
                return results;
            }

            results.AccountEmail = email;
            results.AccountPatientCardN = accountSetList;
            results.FacilityN = facilityResults.FacilityN;
            results.ReadDate = facilityResults.ReadDate;

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryReadEmail(Guid accountKey, QoApiResultsBase results, out string email)
        {
            try
            {
                email = _accountRepo.GetAccountEmail(accountKey);
                return true;
            }
            catch(Exception ex)
            {
                email = string.Empty;
                results.Result = QoApiResult.Build(ex, "Emailの取得に失敗しました。");
                return false;
            }
        }


        // DBから家族情報を取得
        bool TryReadFamilyList(QoAppMedicalNaviInitApiArgs args,  QoApiResultsBase results, out List<QoApiUserItem> userList)
        {            
            try
            {
                var userArgs = new QoUserListReadApiArgs
                {
                    ActorKey = args.ActorKey,
                    ExecuteSystemType = args.ExecuteSystemType,
                    ExecutorName = args.ExecutorName,
                    Executor = args.Executor,
                };

                var userResults = _userListReadWorker.Read(userArgs);
                if(userResults.IsSuccess != bool.TrueString)
                {
                    throw new Exception(userResults.Result.Detail);
                }
                userList = userResults.UserList;
                
                return true;
            }
            catch(Exception ex)
            {
                userList = null;
                results.Result = QoApiResult.Build(ex, $"ユーザーの取得に失敗しました。{ex.Message}");
                return false;
            }
        }

        bool TryReadPatientCards(QoAppMedicalNaviInitApiArgs args, Guid accountKey, QoApiResultsBase results,out List<QoApiPatientCardItem> cardList)
        {
            try
            {
                var cardArgs = new QoLinkagePatientCardListReadApiArgs
                {
                    ActorKey = accountKey.ToApiGuidString(),
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName
                };

                var cardResults = _cardListWorker.Read(cardArgs);
                if(cardResults.IsSuccess != bool.TrueString)
                {
                    throw new Exception($"{nameof(LinkagePatientCardListWorker)}内部でエラーが発生しました。{results.Result.Code}:{results.Result.Detail}");
                }

                cardList = cardResults.PatientCardItemN;
                return true;
            }
            catch(Exception ex)
            {
                cardList = null;
                results.Result = QoApiResult.Build(ex, "診察券の取得に失敗しました。");
                return false;
            }
        }

        bool TryReadFacilityList(QoAppMedicalNaviInitApiArgs args,List<QoApiAccountPatientCardSetItem> accountSetList, QoApiResultsBase results, out QoMasterMedicalFacilityListReadApiResults facilityResults)
        {
            try
            {
                // 重複を排除した施設キーリストを生成
                var facilityKeys = accountSetList.SelectMany(x => x.PatientCardItemN).Select(x => x.FacilityKeyReference).Distinct().ToList();

                // MEMO: 差分をとる必要が出てくればUpdatedDateを指定すると良い
                var facilityArgs = new QoMasterMedicalFacilityListReadApiArgs
                {
                    ActorKey = args.ActorKey,
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,
                    FacilityKeyReferenceList = facilityKeys // 施設キー指定による抽出
                };

                // 施設情報取得
                facilityResults = _facilityListWorker.Read(facilityArgs);
                if (facilityResults.IsSuccess != bool.TrueString)
                {
                    throw new Exception($"{nameof(MasterMedicalFacilityListWorker)}内部でエラーが発生しました。{facilityResults.Result.Code}:{facilityResults.Result.Detail}");
                }
                
                return true;
            }
            catch(Exception ex)
            {
                facilityResults = null;
                results.Result = QoApiResult.Build(ex, "医療機関情報の取得に失敗しました。");
                return false;
            }
        }

    }
}