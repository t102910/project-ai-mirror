using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace MGF.QOLMS.QolmsJotoWebView
{

    internal sealed class NoteExaminationWorker
    {

        IExaminationRepository _examinationRepository;

        #region "Constant"

        private static readonly Dictionary<string, QjHealthAgeValueTypeEnum> HARTLIFE_HEALTHAGE_CALCULATION_VALUES
            = new List<KeyValuePair<string, QjHealthAgeValueTypeEnum>>()
            {
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350106", QjHealthAgeValueTypeEnum.BMI),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350141", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350137", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350139", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350142", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350138", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("350140", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000190", QjHealthAgeValueTypeEnum.Ch019),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000210", QjHealthAgeValueTypeEnum.Ch021),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000450", QjHealthAgeValueTypeEnum.Ch023),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000080", QjHealthAgeValueTypeEnum.Ch025),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000090", QjHealthAgeValueTypeEnum.Ch027),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("000120", QjHealthAgeValueTypeEnum.Ch029),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("701600", QjHealthAgeValueTypeEnum.Ch035),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("001000", QjHealthAgeValueTypeEnum.Ch035FBG),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("003080", QjHealthAgeValueTypeEnum.Ch037),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("003070", QjHealthAgeValueTypeEnum.Ch039)
            }.ToDictionary(i => i.Key, j => j.Value);

        private static readonly Dictionary<string, QjHealthAgeValueTypeEnum> JLAC10_HEALTHAGE_CALCULATION_VALUES
            = new List<KeyValuePair<string, QjHealthAgeValueTypeEnum>>()
            {
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9N011000000000001", QjHealthAgeValueTypeEnum.BMI),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A751000000000001", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A752000000000001", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A755000000000001", QjHealthAgeValueTypeEnum.Ch014),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A761000000000001", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A762000000000001", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("9A765000000000001", QjHealthAgeValueTypeEnum.Ch016),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F015000002327101", QjHealthAgeValueTypeEnum.Ch019),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F015000002327201", QjHealthAgeValueTypeEnum.Ch019),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F015000002399901", QjHealthAgeValueTypeEnum.Ch019),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F070000002327101", QjHealthAgeValueTypeEnum.Ch021),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F070000002327201", QjHealthAgeValueTypeEnum.Ch021),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F070000002399901", QjHealthAgeValueTypeEnum.Ch021),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F077000002327101", QjHealthAgeValueTypeEnum.Ch023),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F077000002327201", QjHealthAgeValueTypeEnum.Ch023),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F077000002391901", QjHealthAgeValueTypeEnum.Ch023),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3F077000002399901", QjHealthAgeValueTypeEnum.Ch023),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B035000002327201", QjHealthAgeValueTypeEnum.Ch025),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B035000002399901", QjHealthAgeValueTypeEnum.Ch025),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B045000002327201", QjHealthAgeValueTypeEnum.Ch027),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B045000002399901", QjHealthAgeValueTypeEnum.Ch027),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B090000002327101", QjHealthAgeValueTypeEnum.Ch029),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3B090000002399901", QjHealthAgeValueTypeEnum.Ch029),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D046000001906202", QjHealthAgeValueTypeEnum.Ch035),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D046000001920402", QjHealthAgeValueTypeEnum.Ch035),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D046000001927102", QjHealthAgeValueTypeEnum.Ch035),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D046000001999902", QjHealthAgeValueTypeEnum.Ch035),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D010000001926101", QjHealthAgeValueTypeEnum.Ch035FBG),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D010000001927201", QjHealthAgeValueTypeEnum.Ch035FBG),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D010000001999901", QjHealthAgeValueTypeEnum.Ch035FBG),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("3D010000002227101", QjHealthAgeValueTypeEnum.Ch035FBG),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("1A020000000190111", QjHealthAgeValueTypeEnum.Ch037),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("1A020000000191111", QjHealthAgeValueTypeEnum.Ch037),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("1A010000000190111", QjHealthAgeValueTypeEnum.Ch039),
            new KeyValuePair<string, QjHealthAgeValueTypeEnum>("1A010000000191111", QjHealthAgeValueTypeEnum.Ch039)
            }.ToDictionary(i => i.Key, j => j.Value);

        private static readonly Lazy<string> _requestUrl = new Lazy<string>(() => GetConfigSettings("ExaminationRequestUri"));
        private static readonly Lazy<string> _jwtExecuter = new Lazy<string>(() => GetConfigSettings("ExaminationCryptedJWTExecuter"));
        private static readonly Lazy<string> _siteUri = new Lazy<string>(() => GetConfigSettings("QolmsYappliSiteUri"));

        #endregion

        #region "Constructor"

        public NoteExaminationWorker(IExaminationRepository examinationRepository)
        {
            _examinationRepository = examinationRepository;
        }

        #endregion

        #region "Private Method"

        public static string GetConfigSettings(string settingsName)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(settingsName))
            {
                try
                {
                    result = ConfigurationManager.AppSettings[settingsName];
                }
                catch
                {
                    // ignore
                }
            }

            return result;
        }


        private static List<ExaminationMatrix> CreateExaminationMatrix(
            QolmsJotoModel mainModel,
            List<ExaminationSetItem> items,
            List<ExaminationGroupItem> groups)
        {
            var resultN = new List<ExaminationMatrix>();
            var result = new ExaminationMatrix();
            var cols = new List<ExaminationSetItem>(items);

            if (cols != null && cols.Any() && groups != null && groups.Any())
            {
                cols = cols.OrderBy(i => i.RecordDate).ThenBy(i => i.Sequence).ToList();

                foreach (var col in cols)
                {
                    var colKey = string.Empty;
                    var associatedFileN = new List<AssociatedFileItem>();
                    var examinationJudgementN = new Dictionary<string, ExaminationJudgementItem>();
                    var healthAge = decimal.MinValue;

                    if (ExaminationAxis.IsColKey(col.CacheKey,ref colKey))
                    {
                        var seq = col.Sequence == int.MinValue ? 0 : col.Sequence;

                        colKey = string.Format("{0:yyyyMMdd}", col.RecordDate) + "_" +
                                 seq.ToString() + "_" +
                                 col.LinkageSystemNo.ToString() + "_" +
                                 col.OrganizationKey.ToString() + "_" +
                                 col.CategoryId.ToString();

                        if (col.OverallAssessmentPdfN != null && col.OverallAssessmentPdfN.Any())
                        {
                            col.OverallAssessmentPdfN.ToList().ForEach(i =>
                            {
                                if (i.FacilityKey != Guid.Empty)
                                {
                                    var json = new AssociatedFileStorageReferenceJsonParameter()
                                    {
                                        Accountkey = mainModel.AuthorAccount.AccountKey.ToString(),
                                        LoginAt = mainModel.AuthorAccount.LoginAt.ToString(),
                                        RecordDate = i.RecordDate.ToString(),
                                        FacilityKey = i.FacilityKey.ToString(),
                                        LinkageSystemNo = i.LinkageSystemNo.ToString(),
                                        LinkageSystemId = i.LinkageSystemId,
                                        DataKey = i.DataKey.ToString()
                                    };

                                    associatedFileN.Add(
                                        new AssociatedFileItem()
                                        {
                                            DataType = QjExaminationDataTypeEnum.OverallAssessmentPdf,
                                            DataKey = i.DataKey,
                                            FacilityKey = i.FacilityKey,
                                            RecordDate = i.RecordDate,
                                            LinkageSystemNo = i.LinkageSystemNo,
                                            LinkageSystemId = i.LinkageSystemId,
                                            FileStorageReferenceJson = json.ToJsonString()
                                        }
                                    );
                                }
                            });
                        }

                        if (col.DicomUrlAccessKeyN != null && col.DicomUrlAccessKeyN.Any())
                        {
                            col.DicomUrlAccessKeyN.Where(i => i != string.Empty).ToList().ForEach(i =>
                            {
                                var facilityKey = col.OrganizationKey.TryToValueType(Guid.Empty);

                                if (facilityKey != Guid.Empty)
                                {
                                    associatedFileN.Add(
                                        new AssociatedFileItem()
                                        {
                                            DataType = QjExaminationDataTypeEnum.DicomData,
                                            AdditionalKey = i,
                                            FacilityKey = facilityKey,
                                            RecordDate = col.RecordDate
                                        }
                                    );
                                }
                            });
                        }

                        if (col.ExaminationJudgementN != null && col.ExaminationJudgementN.Any())
                        {
                            examinationJudgementN = col.ExaminationJudgementN.ToDictionary(i => i.Name);
                        }

                        if (col.HealthAge > 0)
                        {
                            healthAge = col.HealthAge;
                        }

                        result.AddCol(colKey, col.OrganizationName, col.RecordDate.ToString("yyyy/M/d"),
                                      associatedFileN, examinationJudgementN, healthAge, string.Empty);
                    }
                }

                foreach (var grp in groups)
                {
                    foreach (var item in grp.ExaminationN)
                    {
                        foreach (var tset in items)
                        {
                            foreach (var titem in tset.ExaminationN)
                            {
                                if (titem.Code == item.Code)
                                {
                                    item.High = titem.High;
                                    item.Low = titem.Low;
                                    item.Unit = titem.Unit;
                                    item.ReferenceDisplayName = titem.ReferenceDisplayName;
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var grp in groups)
                {
                    result.AddGroupRow(grp);

                    foreach (var row in grp.ExaminationN)
                    {
                        result.AddRow(row);
                    }
                }

                foreach (var col in cols)
                {
                    var colKey = string.Empty;

                    if (ExaminationAxis.IsColKey(col.CacheKey,ref colKey))
                    {
                        var dicHealthAgeValues = new Dictionary<string, string>();
                        var facilityKey = Guid.Parse(col.OrganizationKey);

                        foreach (var row in col.ExaminationN)
                        {
                            var seq = col.Sequence == int.MinValue ? 0 : col.Sequence;

                            colKey = string.Format("{0:yyyyMMdd}", col.RecordDate) + "_" +
                                     seq.ToString() + "_" +
                                     col.LinkageSystemNo.ToString() + "_" +
                                     col.OrganizationKey.ToString() + "_" +
                                     col.CategoryId.ToString();

                            if (facilityKey == Guid.Parse("4C2FC0EA-705A-42D2-B32A-85E9A4EECCDE"))
                            {
                                if (HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(row.Code) &&
                                    !dicHealthAgeValues.ContainsKey(row.Code))
                                {
                                    dicHealthAgeValues.Add(row.Code, row.Value);
                                }
                            }
                            else
                            {
                                if (JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(row.Code) &&
                                    !dicHealthAgeValues.ContainsKey(row.Code))
                                {
                                    dicHealthAgeValues.Add(row.Code, row.Value);
                                }
                            }

                            result.SetItem(row, colKey, row.Code);
                        }
                    }
                }

                result.UpdateMatrix();
                resultN.Add(result);
            }

            return resultN;
        }

        private static void WorningLogValueCheck(List<decimal> values, Guid accountkey, DateTime recordDate)
        {
            if (values.Count > 1)
            {
                foreach (var val1 in values)
                {
                    if (values.Where(i => i != val1).Count() > 0)
                    {
                        AccessLogWorker.WriteAccessLog(
                            null,
                            string.Empty,
                            AccessLogWorker.AccessTypeEnum.None,
                            string.Format("Warning ExaminationHealthAge AccountKey={0} RecordDate={1}", accountkey, recordDate)
                        );
                        break;
                    }
                }
            }
        }

        #endregion

        #region "Private Method - CreateViewModel"

        public NoteExaminationViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var cache = new NoteExaminationViewModel()
            {
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MinValue
            };

            mainModel.GetModelPropertyCache(cache, m => m.StartDate);
            mainModel.GetModelPropertyCache(cache, m => m.EndDate);

            var apiResult = _examinationRepository.ExecuteNoteExaminationReadApi(mainModel);

            var examinationList = apiResult.ExaminationSetN.ConvertAll(i => i.ToExaminationSetItem());
            var groupList = apiResult.ExaminationGroupN.ConvertAll(i => i.ToExaminationGroupItem());

            var dic = new Dictionary<DateTime, Dictionary<string, string>>();

            foreach (var item in examinationList)
            {
                var dic2 = new Dictionary<string, string>();

                foreach (var item2 in item.ExaminationN)
                {
                    if (Guid.Parse(item.OrganizationKey) == Guid.Parse("4C2FC0EA-705A-42D2-B32A-85E9A4EECCDE"))
                    {
                        if (HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(item2.Code) &&
                            !dic2.ContainsKey(item2.Code))
                        {
                            dic2.Add(item2.Code, item2.Value);
                        }
                    }
                    else
                    {
                        if (JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(item2.Code) &&
                            !dic2.ContainsKey(item2.Code))
                        {
                            dic2.Add(item2.Code, item2.Value);
                        }
                    }
                }

                if (!dic.ContainsKey(item.RecordDate))
                {
                    dic.Add(item.RecordDate, dic2);
                }
            }

            var json = new NoteExaminationHelthAgeJsonParamater()
            {
                Accountkey = mainModel.AuthorAccount.AccountKey,
                LoginAt = mainModel.AuthorAccount.LoginAt,
                healthAgeCalcN = dic
            };

            return new NoteExaminationViewModel(
                mainModel,
                cache.StartDate,
                cache.EndDate,
                CreateExaminationMatrix(mainModel, examinationList, groupList),
                false,
                examinationList,
                groupList,
                json.ToJsonString()
            );
        }

        public byte[] GetPdfFile(
            QolmsJotoModel mainModel,
            Guid fileKey,
            int linkageSystemNo,
            string linkageSystemId,
            DateTime recordDate,
            Guid facilityKey,
            ref string refOriginalName,
            ref string refContentType)
        {
            refOriginalName = string.Empty;
            refContentType = string.Empty;

            byte[] result = null;

            if (fileKey != Guid.Empty)
            {
                result = _examinationRepository.ExecuteNoteExaminationPdfReadApi(
                    mainModel,
                    fileKey,
                    linkageSystemNo,
                    linkageSystemId,
                    recordDate,
                    facilityKey,
                    ref refOriginalName,
                    ref refContentType
                );
            }

            return result;
        }

        public List<HealthAgeEditInputModel> CreateHealthAgeEditInputModel(
            QolmsJotoModel mainModel,
            NoteExaminationHelthAgeJsonParamater inputModel)
        {
            var result = new List<HealthAgeEditInputModel>();

            foreach (var item in inputModel.healthAgeCalcN)
            {
                var dic = new Dictionary<QjHealthAgeValueTypeEnum, decimal>();

                foreach (var examination in item.Value)
                {
                    if (HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(examination.Key))
                    {
                        ProcessHartlifeHealthAgeValue(
                            examination.Key,
                            examination.Value,
                            item.Value,
                            dic,
                            mainModel.AuthorAccount.AccountKey,
                            item.Key
                        );
                    }
                    else if (JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(examination.Key))
                    {
                        ProcessJlac10HealthAgeValue(
                            examination.Key,
                            examination.Value,
                            item.Value,
                            dic,
                            mainModel.AuthorAccount.AccountKey,
                            item.Key
                        );
                    }
                }

                if (dic.Count == 13)
                {
                    //Todo:UseCaseの方がよさそう
                    var helthAgeWorker = new HealthAgeEditWorker(new HealthAgeRepository(), new JmdcHealthAgeApiRepository());

                    result.Add(
                        helthAgeWorker.CreateInputModelByExamination(
                            mainModel,
                            item.Key,
                            dic,
                            QjPageNoTypeEnum.NoteExamination
                        )
                    );
                }
            }

            return result;
        }

        private static void ProcessHartlifeHealthAgeValue(
            string examinationKey,
            string examinationValue,
            Dictionary<string, string> itemValues,
            Dictionary<QjHealthAgeValueTypeEnum, decimal> dic,
            Guid accountKey,
            DateTime recordDate)
        {
            var valueType = HARTLIFE_HEALTHAGE_CALCULATION_VALUES[examinationKey];
            decimal value = decimal.MinValue;

            if (valueType == QjHealthAgeValueTypeEnum.Ch037 || valueType == QjHealthAgeValueTypeEnum.Ch039)
            {
                value = ParseHealthAgeStringValue(examinationValue);
            }
            else
            {
                if (!decimal.TryParse(examinationValue, out value))
                {
                    return;
                }
            }

            if (valueType == QjHealthAgeValueTypeEnum.Ch014)
            {
                // 最高血圧
                if (examinationKey == "350141" ||
                    (examinationKey == "350137" && !itemValues.ContainsKey("350141")) ||
                    (examinationKey == "350139" && !itemValues.ContainsKey("350141") && !itemValues.ContainsKey("350137")))
                {
                    dic.Add(valueType, value);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch016)
            {
                // 最低血圧
                if (examinationKey == "350142" ||
                    (examinationKey == "350138" && !itemValues.ContainsKey("350142")) ||
                    (examinationKey == "350140" && !itemValues.ContainsKey("350142") && !itemValues.ContainsKey("350138")))
                {
                    dic.Add(valueType, value);
                }
            }
            else
            {
                dic.Add(valueType, value);
            }
        }

        private static void ProcessJlac10HealthAgeValue(
            string examinationKey,
            string examinationValue,
            Dictionary<string, string> itemValues,
            Dictionary<QjHealthAgeValueTypeEnum, decimal> dic,
            Guid accountKey,
            DateTime recordDate)
        {
            var valueType = JLAC10_HEALTHAGE_CALCULATION_VALUES[examinationKey];
            decimal value = decimal.MinValue;

            if (valueType == QjHealthAgeValueTypeEnum.Ch037 || valueType == QjHealthAgeValueTypeEnum.Ch039)
            {
                value = ParseHealthAgeStringValue(examinationValue);
            }
            else
            {
                if (!decimal.TryParse(examinationValue, out value))
                {
                    return;
                }
            }

            if (valueType == QjHealthAgeValueTypeEnum.Ch014)
            {
                // 最高血圧
                if ((examinationKey == "9A751000000000001" && value > 0M) ||
                    ((examinationKey == "9A752000000000001" && value > 0M) &&
                     (!itemValues.ContainsKey("9A751000000000001") ||
                      (itemValues.ContainsKey("9A751000000000001") && itemValues["9A751000000000001"].TryToValueType(decimal.MinValue) <= 0M))) ||
                    ((examinationKey == "9A755000000000001" && value > 0M) &&
                     (!itemValues.ContainsKey("9A751000000000001") ||
                      (itemValues.ContainsKey("9A751000000000001") && itemValues["9A751000000000001"].TryToValueType(decimal.MinValue) <= 0M)) &&
                     (!itemValues.ContainsKey("9A752000000000001") ||
                      (itemValues.ContainsKey("9A752000000000001") && itemValues["9A751000000000001"].TryToValueType(decimal.MinValue) <= 0M))))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "9A755000000000001" || i.Key == "9A751000000000001" || i.Key == "9A755000000000001")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch016)
            {
                // 最低血圧
                if ((examinationKey == "9A761000000000001" && value > 0M) ||
                    ((examinationKey == "9A762000000000001" && value > 0M) &&
                     (!itemValues.ContainsKey("9A761000000000001") ||
                      (itemValues.ContainsKey("9A761000000000001") && itemValues["9A761000000000001"].TryToValueType(decimal.MinValue) <= 0M))) ||
                    ((examinationKey == "9A765000000000001" && value > 0M) &&
                     (!itemValues.ContainsKey("9A761000000000001") ||
                      (itemValues.ContainsKey("9A761000000000001") && itemValues["9A761000000000001"].TryToValueType(decimal.MinValue) <= 0M)) &&
                     (!itemValues.ContainsKey("9A762000000000001") ||
                      (itemValues.ContainsKey("9A762000000000001") && itemValues["9A762000000000001"].TryToValueType(decimal.MinValue) <= 0M))))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "9A765000000000001" || i.Key == "9A761000000000001" || i.Key == "9A762000000000001")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch019)
            {
                if (examinationKey == "3F015000002327101" ||
                    (examinationKey == "3F015000002327201" && !itemValues.ContainsKey("3F015000002327101")) ||
                    (examinationKey == "3F015000002399901" && !itemValues.ContainsKey("3F015000002327201") && !itemValues.ContainsKey("3F015000002327101")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3F015000002327101" || i.Key == "3F015000002327201" || i.Key == "3F015000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch021)
            {
                if (examinationKey == "3F070000002327101" ||
                    (examinationKey == "3F070000002327201" && !itemValues.ContainsKey("3F070000002327101")) ||
                    (examinationKey == "3F070000002399901" && !itemValues.ContainsKey("3F070000002327201") && !itemValues.ContainsKey("3F070000002327101")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3F070000002327101" || i.Key == "3F070000002327201" || i.Key == "3F070000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch023)
            {
                if (examinationKey == "3F077000002327101" ||
                    (examinationKey == "3F077000002327201" && !itemValues.ContainsKey("3F077000002327101")) ||
                    (examinationKey == "3F077000002391901" && !itemValues.ContainsKey("3F077000002327201") && !itemValues.ContainsKey("3F077000002327101")) ||
                    (examinationKey == "3F077000002399901" && !itemValues.ContainsKey("3F077000002391901") && !itemValues.ContainsKey("3F077000002327201") && !itemValues.ContainsKey("3F077000002327101")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3F077000002327101" || i.Key == "3F077000002327201" || i.Key == "3F077000002391901" || i.Key == "3F077000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch025)
            {
                if (examinationKey == "3B035000002327201" ||
                    (examinationKey == "3B035000002399901" && !itemValues.ContainsKey("3B035000002327201")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3B035000002327201" || i.Key == "3B035000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch027)
            {
                if (examinationKey == "3B045000002327201" ||
                    (examinationKey == "3B045000002399901" && !itemValues.ContainsKey("3B045000002327201")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3B045000002327201" || i.Key == "3B045000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch029)
            {
                if (examinationKey == "3B090000002327101" ||
                    (examinationKey == "3B090000002399901" && !itemValues.ContainsKey("3B090000002327101")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3B045000002327201" || i.Key == "3B045000002399901")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch035)
            {
                if (examinationKey == "3D046000001906202" ||
                   (examinationKey == "3D046000001920402" && !itemValues.ContainsKey("3D046000001906202")) ||
                   (examinationKey == "3D046000001927102" && !itemValues.ContainsKey("3D046000001920402") && !itemValues.ContainsKey("3D046000001906202")) ||
                   (examinationKey == "3D046000001999902" && !itemValues.ContainsKey("3D046000001927102") && !itemValues.ContainsKey("3D046000001920402") && !itemValues.ContainsKey("3D046000001906202")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3D046000001906202" || i.Key == "3D046000001920402" || i.Key == "3D046000001927102" || i.Key == "3D046000001999902")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch035FBG)
            {
                if (examinationKey == "3D010000001926101" ||
                   (examinationKey == "3D010000001927201" && !itemValues.ContainsKey("3D010000001926101")) ||
                   (examinationKey == "3D010000001999901" && !itemValues.ContainsKey("3D010000001927201") && !itemValues.ContainsKey("3D010000001926101")) ||
                   (examinationKey == "3D010000002227101" && !itemValues.ContainsKey("3D010000001999901") && !itemValues.ContainsKey("3D010000001927201") && !itemValues.ContainsKey("3D010000001926101")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "3D010000001926101" || i.Key == "3D010000001927201" || i.Key == "3D010000001999901" || i.Key == "3D010000002227101")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch037)
            {
                if (examinationKey == "1A020000000190111" ||
                   (examinationKey == "1A020000000191111" && !itemValues.ContainsKey("1A020000000190111")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "1A020000000190111" || i.Key == "1A020000000191111")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else if (valueType == QjHealthAgeValueTypeEnum.Ch039)
            {
                if (examinationKey == "1A010000000190111" ||
                   (examinationKey == "1A010000000191111" && !itemValues.ContainsKey("1A010000000190111")))
                {
                    dic.Add(valueType, value);

                    var values = itemValues
                        .Where(i => i.Key == "1A010000000190111" || i.Key == "1A010000000191111")
                        .Select(i => i.Value.TryToValueType(decimal.MinValue))
                        .ToList();

                    WorningLogValueCheck(values, accountKey, recordDate);
                }
            }
            else
            {
                dic.Add(valueType, value);
            }
        }

        private static decimal ParseHealthAgeStringValue(string value)
        {
            var str = ToZenkaku(value).Replace("（", string.Empty).Replace("）", string.Empty);

            if (str == "－")
                return 1M;
            else if (str == "±")
                return 2M;
            else if (str == "＋" || str == "１＋")
                return 3M;
            else if (str == "＋＋" || str == "２＋")
                return 4M;
            else if (str == "＋＋＋" || str == "３＋")
                return 5M;
            else
                return decimal.MinValue;
        }

        public static List<ExaminationMatrix> CreateExaminationMatrixFromItems(
            QolmsJotoModel mainModel,
            List<ExaminationSetItem> items,
            List<ExaminationGroupItem> groups)
        {
            return CreateExaminationMatrix(mainModel, items, groups);
        }

        //public static string GetExaminationPage(QolmsJotoModel mainModel)
        //{
        //    var uri = RequestUrl;
        //    if (uri.EndsWith("/"))
        //    {
        //        uri = uri.TrimEnd('/');
        //    }

        //    var returnUrl = SiteUri;

        //    if (returnUrl.EndsWith("/"))
        //    {
        //        returnUrl = returnUrl.TrimEnd('/');
        //    }

        //    returnUrl += RETURN_URL_PASS;

        //    if (fromPageNoType == QjPageNoTypeEnum.PortalCompanyConnectionHome)
        //    {
        //        returnUrl += "?fromPageNo=34";
        //    }
        //    else if (fromPageNoType == QjPageNoTypeEnum.PortalHome)
        //    {
        //        returnUrl += "?fromPageNo=1";
        //    }

        //    var url = string.Format(
        //        "{0}?pageno=2&jwt={1}&redirecturl={2}",
        //        uri,
        //        Uri.EscapeDataString(GetExamintiontJwt(mainModel)),
        //        Uri.EscapeDataString(returnUrl)
        //    );

        //    return url;
        //}

        private static string ToZenkaku(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (c >= 'A' && c <= 'Z')
                    sb.Append((char)(c + 0xFEE0));
                else if (c >= 'a' && c <= 'z')
                    sb.Append((char)(c + 0xFEE0));
                else if (c >= '0' && c <= '9')
                    sb.Append((char)(c + 0xFEE0));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion
    }
}
