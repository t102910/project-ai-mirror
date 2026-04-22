using System;
using System.Collections.Generic;
using System.Linq;

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Joto ネイティブアプリのホーム画面表示情報 を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class PhrForHomeReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, PhrForHomeReaderArgs, PhrForHomeReaderResults>
    {
        #region "Private Property"
        #endregion

        #region "Public Property"
        #endregion

        #region "Constructor"
        /// <summary>
        /// <see cref="PhrForHomeReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public PhrForHomeReader() : base()
        {
        }
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public PhrForHomeReaderResults ExecuteByDistributed(PhrForHomeReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            PhrForHomeReaderResults result = new PhrForHomeReaderResults() { IsSuccess = false };

            Guid accountKey = args.AccountKey;
            DateTime targetDate = args.TargetDate;

            if (accountKey == Guid.Empty || targetDate == DateTime.MinValue)
            {
                return result;
            }

            result.TargetDate = DateTime.Now.ToApiDateString();

            // バイタル情報を取得
            DbHealthRecordReaderCore healthRecordReader = new DbHealthRecordReaderCore(accountKey);
            // order by vitaltype asc, recorddate desc
            List<QH_HEALTHRECORD_DAT> healthRecords = healthRecordReader.ReadHealthRecordList(targetDate);

            if (healthRecords != null && healthRecords.Count > 0)
            {
                var amSbpList = new List<decimal>();
                var amDbpList = new List<decimal>();
                var pmSbpList = new List<decimal>();
                var pmDbpList = new List<decimal>();

                foreach (var record in healthRecords)
                {
                    switch ((QsDbVitalTypeEnum)record.VITALTYPE)
                    {
                        case QsDbVitalTypeEnum.Steps:
                            // 最新のものを取得
                            if (result.Steps == decimal.MinValue)
                            {
                                result.Steps = record.VALUE1;
                            }
                            break;
                        case QsDbVitalTypeEnum.BodyWeight:
                            // 最新のものを取得
                            if (result.BodyWeight == decimal.MinValue)
                            {
                                result.BodyWeight = record.VALUE1;
                            }
                            break;
                        case QsDbVitalTypeEnum.BodyMassIndex:
                            // 最新のものを取得
                            if (result.BMI == decimal.MinValue)
                            {
                                result.BMI = record.VALUE1;
                            }
                            break;
                        case QsDbVitalTypeEnum.BloodPressure:
                            // 後で平均を出すために保持
                            if (record.RECORDDATE.Hour < 12)
                            {
                                amSbpList.Add(record.VALUE1);
                                amDbpList.Add(record.VALUE2);
                            }
                            else
                            {
                                pmSbpList.Add(record.VALUE1);
                                pmDbpList.Add(record.VALUE2);
                            }
                            break;
                        case QsDbVitalTypeEnum.SleepingTime:
                            // 最古のものを取得
                            // ★ Apple/Google 準拠: RECORDDATE = BedTime（就寝時刻）
                            //    WakeupTime = BedTime + VALUE1（睡眠分数）
                            result.BedTime   = record.RECORDDATE.ToApiDateString();
                            result.SleepTime = record.VALUE1;
                            if (record.VALUE1 != decimal.MinValue)
                            {
                                result.WakeupTime = record.RECORDDATE
                                    .AddMinutes((double)record.VALUE1)
                                    .ToApiDateString();
                            }
                            break;
                        default:
                            break;
                    }
                }

                // 血圧：午前・午後それぞれの平均値をセット
                if (amSbpList.Count > 0)
                {
                    result.SBP1 = Math.Round(amSbpList.Average(), 0, MidpointRounding.AwayFromZero);
                    result.DBP1 = Math.Round(amDbpList.Average(), 0, MidpointRounding.AwayFromZero);
                }
                if (pmSbpList.Count > 0)
                {
                    result.SBP2 = Math.Round(pmSbpList.Average(), 0, MidpointRounding.AwayFromZero);
                    result.DBP2 = Math.Round(pmDbpList.Average(), 0, MidpointRounding.AwayFromZero);
                }
            }

            // 血糖値情報を取得（前回と最新の2件）
            List<QH_HEALTHRECORD_DAT> bloodSugarRecords = healthRecordReader.ReadPreviousAndLatestBloodSugarList(targetDate);

            if (bloodSugarRecords != null && bloodSugarRecords.Count > 0)
            {
                if (bloodSugarRecords.Count >= 1)
                {
                    result.BloodSugarLevel1 = bloodSugarRecords[0].VALUE1;
                    result.BloodSugarLevelTiming1 = bloodSugarRecords[0].CONDITIONTYPE.ToString();
                    result.BloodSugarLevelLogTime1 = bloodSugarRecords[0].CREATEDDATE.ToApiDateString();
                }

                if (bloodSugarRecords.Count >= 2)
                {
                    result.BloodSugarLevel2 = bloodSugarRecords[1].VALUE1;
                    result.BloodSugarLevelTiming2 = bloodSugarRecords[1].CONDITIONTYPE.ToString();
                    result.BloodSugarLevelLogTime2 = bloodSugarRecords[1].CREATEDDATE.ToApiDateString();
                }
            }

            // BMIが未取得で体重が取得できている場合、身長からBMIを計算
            if (result.BMI == decimal.MinValue && result.BodyWeight != decimal.MinValue)
            {
                QH_HEALTHRECORD_DAT heightRecord = healthRecordReader.ReadLatestHeight();
                if (heightRecord != null && heightRecord.VALUE1 > 0)
                {
                    decimal height = heightRecord.VALUE1 / 100m; // cm から m に変換
                    decimal bmi = result.BodyWeight / (height * height);
                    result.BMI = Math.Round(bmi, 1);
                }
            }

            // 気分を取得
            const int jotoHdrLinkageSystemNo = QoLinkage.JOTO_LINKAGE_SYSTEM_NO;
            DbEventReaderCore eventReader = new DbEventReaderCore(accountKey);
            List<QH_EVENT_DAT> mentalEventRecords = eventReader.ReadMentalEventList(targetDate, jotoHdrLinkageSystemNo);

            if (mentalEventRecords != null && mentalEventRecords.Count > 0)
            {
                QH_EVENT_DAT latestMentalEvent = mentalEventRecords[0];
                if (latestMentalEvent != null && !string.IsNullOrWhiteSpace(latestMentalEvent.EVENTSET))
                {
                    try
                    {
                        QsJsonSerializer serializer = new QsJsonSerializer();
                        QhQolmsDiaryEventSetOfJson eventSet = serializer.Deserialize<QhQolmsDiaryEventSetOfJson>(latestMentalEvent.EVENTSET);
                        if (eventSet != null)
                        {
                            result.Feelings = eventSet.FeelingType;
                        }
                    }
                    catch
                    {
                        result.Feelings = string.Empty;
                    }
                }
            }

            // 運動イベント情報を取得
            DbExerciseEvent2ReaderCore exerciseReader = new DbExerciseEvent2ReaderCore(accountKey);
            List<QH_EXERCISEEVENT2_DAT> exerciseRecords = exerciseReader.ReadExerciseEventList(targetDate);

            if (exerciseRecords != null && exerciseRecords.Count > 0)
            {
                int totalCalorie = exerciseRecords.Sum(e => (int)e.CALORIE);
                result.Exercise = totalCalorie;
            }

            // 食事イベント情報を取得
            DbMealEvent2ReaderCore mealReader = new DbMealEvent2ReaderCore(accountKey);
            List<QH_MEALEVENT2_DAT> mealRecords = mealReader.ReadMealEventList(targetDate);

            if (mealRecords != null && mealRecords.Count > 0)
            {
                int totalMealCalorie = mealRecords.Sum(m => (int)m.CALORIE);
                result.CaloryInMeal = totalMealCalorie;

                var mealTypes = mealRecords
                    .Select(m => m.MEALTYPE)
                    .Distinct()
                    .OrderBy(t => t)
                    .Select(t => t.ToString());
                result.MealType = string.Join(",", mealTypes);
            }

            result.IsSuccess = true;
            return result;
        }
        #endregion
    }
}
