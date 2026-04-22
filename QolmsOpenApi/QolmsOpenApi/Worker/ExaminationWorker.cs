using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsExaminationEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 検査結果関連の機能を提供します。
    /// </summary>
    public class ExaminationWorker
    {
        
        #region "Private Method"
                
        //アカウントキー、検査日、施設キーをもとに検査結果を検索。
        private static List<QH_EXAMINATION_DAT> GetExaminationDetailList(Guid actorKey, DateTime recordDate, Guid facilityKeyReference)
        {
            var reader = new ExaminationDetailReader();
            try
            {
                var readerArgs = new ExaminationDetailReaderArgs() { AccountKey = actorKey, RecordDate = recordDate, FacilityKey = facilityKeyReference};
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count >= 0)
                {
                    return readerResults.Result;
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[GetExaminationDetailList]検査結果情報の取得に失敗しました。AccountKey:{actorKey}, RecordDate:{recordDate}, FacilityKey:{facilityKeyReference}"), Guid.Empty);
                    return new List<QH_EXAMINATION_DAT>();
                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return null;
        }

        //施設キーをもとに検査結果グループ情報を検索。
        private static Dictionary<string, Tuple<int, string, string, string>> GetExaminationGroupList(Guid facilityKeyReference)
        {
            var reader = new ExaminationGroupNameReader();
            try
            {
                var readerArgs = new ExaminationGroupNameReaderArgs() { FacilityKey = facilityKeyReference };
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.GroupNameList != null && readerResults.GroupNameList.Count >= 0)
                {
                    return readerResults.GroupNameList;
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[GetExaminationGroupList]検査結果グループ情報の取得に失敗しました。FacilityKey:{facilityKeyReference}"), Guid.Empty);
                    return new Dictionary<string, Tuple<int, string, string, string>>();
                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return null;
        }

        #endregion
        #region "Public Method"


        /// <summary>
        /// 検査結果リストを取得して結果を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoExaminationDetailReadApiResults ExaminationDetailRead(QoExaminationDetailReadApiArgs args)
        {
            var result = new QoExaminationDetailReadApiResults() { IsSuccess = bool.FalseString , DetailList = new List<QoApiExaminationDetailItem>()};

            //引数チェック
            //検査結果取得対象アカウントキー
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty) //必須
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            //検査日
            if (!DateTime.TryParse(args.RecordDate,out DateTime recordDate)) //必須
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            //施設キー
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference<Guid>();
            if (facilityKey == Guid.Empty) //必須
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }


            //検査結果検索
            List<QH_EXAMINATION_DAT> examinationDetailList = GetExaminationDetailList(accountKey, recordDate, facilityKey);
            
            //検査項目グループ情報取得
            Dictionary<string, Tuple<int, string, string, string>> examinationGroupList = GetExaminationGroupList(facilityKey);

            //正常にDB検索できた場合処理続行。
            if (examinationDetailList != null && examinationDetailList.Count > 0 && examinationGroupList != null && examinationGroupList.Count > 0)
            {

                //取得した検査結果数分処理を行う。
                foreach (var entity in examinationDetailList)
                {

                    //CONVERTEDDATASETをJsonクラスへ変換。
                    Examination examinationDat = new QsJsonSerializer().Deserialize<Examination>(entity.CONVERTEDDATASET);

                    if (examinationDat == null)
                    {
                        //Jsonクラスが得られなかった場合。
                        continue;
                    }

                    //格納クラス
                    var detailItem = new QoApiExaminationDetailItem();
                    var GroupItem = new QoApiExaminationGroupItem();

                    //GroupNo保持
                    int GroupNo = int.MinValue;

                    //検査項目グループ順に処理を行う。
                    foreach (var Group in examinationGroupList)
                    {

                        //GroupNoが切り替わった場合
                        if (GroupNo != Group.Value.Item1)
                        {
                            //GroupItemに検査結果が格納されている場合、返却クラスに追加
                            if (GroupItem.ItemList != null)
                            {
                                if (GroupItem.ItemList.Count > 0)
                                {
                                    //格納クラス初期化
                                    if (detailItem.GroupList == null)
                                    {
                                        detailItem.GroupList = new List<QoApiExaminationGroupItem>();
                                    }
                                    detailItem.GroupList.Add(GroupItem);
                                }
                            }
                            //グループ情報の初期化、及び設定
                            GroupNo = Group.Value.Item1;
                            GroupItem = new QoApiExaminationGroupItem();
                            GroupItem.GroupName = Group.Value.Item2;
                        }

                        //検査結果項目検索
                        foreach (ExaminationItem examinationItem in examinationDat.ExaminationItems)
                        {
                            //ItemCodeチェック　※JRAC10コード優先、JRAC10コードとローカルコードが両方設定されていた場合、または設定されていない場合スキップ
                            if ((!string.IsNullOrEmpty(examinationItem.Code) && !string.IsNullOrEmpty(examinationItem.LocalCode))
                                || (string.IsNullOrEmpty(examinationItem.Code) && string.IsNullOrEmpty(examinationItem.LocalCode)))
                            {
                                continue;
                            }

                            //コード判定
                            string itemCode = string.IsNullOrEmpty(examinationItem.Code) ? examinationItem.LocalCode : examinationItem.Code;

                            //対象が存在する場合
                            if (Group.Key == itemCode)
                            {
                                //格納クラス初期化
                                if (GroupItem.ItemList == null)
                                {
                                    GroupItem.ItemList = new List<QoApiExaminationItem>();
                                }
                                //検査値格納
                                GroupItem.ItemList.Add(new QoApiExaminationItem()
                                {
                                    Name = Group.Value.Item3,
                                    Unit = examinationItem.Unit,
                                    Value = examinationItem.Value,
                                    High = examinationItem.High,
                                    Low = examinationItem.Low,
                                    IsLower = examinationItem.Interpretation == "L" ? true.ToString() : false.ToString(),
                                    IsHigher = examinationItem.Interpretation == "H" ? true.ToString() : false.ToString(),
                                    Comment = Group.Value.Item4
                                });
                                //次の検査項目へjump
                                break;
                            }

                        }
                    }

                    //検査結果格納
                    if (detailItem.GroupList != null && detailItem.GroupList.Count > 0)
                    {
                        result.DetailList.Add(detailItem);
                    }

                }

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            //正常に処理が行われたが、検査結果が0件の場合
            else if (examinationDetailList != null && examinationDetailList.Count == 0)
            {
                //空リスト作成
                result.DetailList = new List<QoApiExaminationDetailItem>();
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            else
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
            }

            return result;
        }
        
        #endregion
    }
}