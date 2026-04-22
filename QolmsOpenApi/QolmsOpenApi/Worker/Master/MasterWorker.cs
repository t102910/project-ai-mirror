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

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// マスター関連の機能を提供します。
    /// </summary>
    public class MasterWorker
    {
        #region "Private Method"        

        
        //医療機関番号、もしくは都道府県コード、キーワードをもとに医療施設を検索。
        private static List<QH_FACILITY_MST> GetMedicalFacilitySearchList(string prefectureCode, string keyWord, int pageIndex, int pageSize, string medicalFacilityCode)
        {
            var reader = new MedicalFacilitySearchReader();
            var readerArgs = new MedicalFacilitySearchReaderArgs() { PrefectureCode = prefectureCode, KeyWord = keyWord, PageIndex = pageIndex, PageSize = pageSize, MedicalFacilityCode = medicalFacilityCode};
            try
            {
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                {
                    return readerResults.Result;
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[GetMedicalFacilitySearchList]医療施設情報の取得に失敗しました。PrefectureCode:{prefectureCode}, KeyWord:{keyWord}, KeyWord:{keyWord}, PageIndex:{pageIndex}, PageSize:{pageSize}, MedicalFacilityCode:{medicalFacilityCode}"), Guid.Empty);
                    return new List<QH_FACILITY_MST>();
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
        /// 医療機関施設リストを取得して結果を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoMasterMedicalFacilitySearchApiResults MedicalFacilitySearchRead(QoMasterMedicalFacilitySearchApiArgs args)
        {
            var result = new QoMasterMedicalFacilitySearchApiResults() { IsSuccess = bool.FalseString };

            //引数チェック
            //都道府県コード
            if (string.IsNullOrEmpty(args.PrefectureCode)) //必須
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }


            //DB検索
            var medicalFacilitylist = GetMedicalFacilitySearchList(args.PrefectureCode, args.KeyWord, args.PageIndex.TryToValueType(0), args.PageSize.TryToValueType(1), args.MedicalFacilityCode);
            if (medicalFacilitylist != null)
            {
                result.FacilityList = new List<QoApiMedicalFacilitySearchItem>();

                foreach (var entity in medicalFacilitylist)
                {
                    result.FacilityList.Add(new QoApiMedicalFacilitySearchItem()
                    {
                        FacilityId = entity.FACILITYID,
                        FacilityName = entity.FACILITYNAME,
                        FacilityKanaName = entity.FACILITYKANANAME,
                        PostalCode = entity.POSTALCODE,
                        Address1 = entity.ADDRESS1,
                        Address2 = entity.ADDRESS2,
                        PrefectureCode = entity.PREFNO.ToString(),
                        CityCode = entity.CITYNO.ToString(),
                        Tel = entity.TEL,
                        Fax = entity.FAX,
                        OfficialName = entity.OFFICIALNAME,
                        MedicalFacilityCode = entity.MEDICALFACILITYCODE
                    });
                    QoAccessLog.WriteErrorLog("FacilityList OK", Guid.Empty);
                }

                //最終ページインデックス ***DISPORDERに取得件数格納***
                int lastPageIndex = int.MinValue;
                try
                {
                    //総Index数計算
                    lastPageIndex = Math.DivRem(medicalFacilitylist.First().DISPORDER, args.PageSize.TryToValueType(0), out int rem);
                    lastPageIndex = rem != 0 ? lastPageIndex : lastPageIndex - 1 ;
                }
                catch
                {
                    lastPageIndex = 1;
                }
                result.LastPageIndex = lastPageIndex.ToString();

                //取得ページインデックス 最終ページインデックスのほうが大きい場合、最終ページインデックスを設定
                result.PageIndex = args.PageIndex.TryToValueType(0) > lastPageIndex ? lastPageIndex.ToString() : args.PageIndex.TryToValueType(0).ToString();

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