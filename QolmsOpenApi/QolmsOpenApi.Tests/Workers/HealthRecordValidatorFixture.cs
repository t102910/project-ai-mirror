using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthRecordValidatorFixture
    {
        IHealthRecordValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new HealthRecordValidator();
        }

        [TestMethod]
        public void RecordDateが日付でない場合はNG()
        {
            
            var vitals = new List<QhApiVitalValueItem>
            {
                new QhApiVitalValueItem
                {
                    RecordDate = "hogefuga"
                }
            };

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("RecordDateが不正です").IsTrue();
        }

        [TestMethod]
        public void Value1がdecimal値でない場合はNG()
        {            
            var vitals = new List<QhApiVitalValueItem>
            {
                new QhApiVitalValueItem
                {
                    RecordDate = DateTime.Today.ToApiDateString(),
                    Value1 = "12a34"
                }
            };

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Value1が不正です").IsTrue();
        }

        [TestMethod]
        public void 歩数に時間データがあればNG()
        {
            var vitals = GetValidVitalvitals();
            vitals[0].RecordDate = new DateTime(2022, 10, 10, 10, 10, 0).ToApiDateString();

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("歩数は時間単位のデータを受け付けません").IsTrue();
        }

        [TestMethod]
        public void 歩数に少数があればNG()
        {
            var vitals = GetValidVitalvitals();
            vitals[0].Value1 = "300.52";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("歩数に少数は設定できません").IsTrue();
        }

        [TestMethod]
        public void 歩数が範囲外であればNG()
        {
            var vitals = GetValidVitalvitals();

            // 最小値未満
            vitals[0].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("歩数に最小値より小さい値").IsTrue();

            // 最大値を超える
            vitals[0].Value1 = (HealthRecordValidator.SIX_DIGIT_MAX_VALUE + 1M).ToString();

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("歩数に最小値より小さい値").IsTrue();
        }

        [TestMethod]
        public void 睡眠が範囲外であればNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[1].Value1 = "0.5";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが睡眠として不正です").IsTrue();

            // 最大値を超える
            vitals[1].Value1 = $"{60 * 24 + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが睡眠として不正です").IsTrue();
        }

        [TestMethod]
        public void 血圧が不正であればNG()
        {
            var vitals = GetValidVitalvitals();
            // 最高血圧 最小値未満
            vitals[2].Value1 = "0.5";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();

            // 最高血圧 最大値を超える
            vitals[2].Value1 = $"{HealthRecordValidator.MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();

            // 最低血圧 最小値未満
            vitals[2].Value2 = "0.5";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();

            // 最高血圧 正常値
            vitals[2].Value1 = "120";

            // 最低血圧 最大値を超える
            vitals[2].Value2 = $"{HealthRecordValidator.MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();

            // 最低血圧 最大値を超える
            vitals[2].Value2 = $"{HealthRecordValidator.MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();

            // 最高血圧を超える最低血圧
            vitals[2].Value2 = "240";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血圧値として不正です").IsTrue();
        }

        [TestMethod]
        public void 体内年齢が範囲外の場合はNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[3].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[3].Value1 = $"{HealthRecordValidator.THREE_DIGIT_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 少数あり
            vitals[3].Value1 = "50.52";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 脈拍が範囲外の場合はNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[4].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[4].Value1 = $"{HealthRecordValidator.THREE_DIGIT_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 少数あり
            vitals[4].Value1 = "50.52";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 血糖値でConditionが不正だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 正常値
            vitals[5].Value1 = "50";
            // 存在しないConditionType
            vitals[5].ConditionType = "10";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血糖値として不正です").IsTrue();
        }

        [TestMethod]
        public void 血糖値で範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[5].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血糖値として不正です").IsTrue();

            // 最大値を超える
            vitals[5].Value1 = $"{HealthRecordValidator.MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが血糖値として不正です").IsTrue();
        }

        [TestMethod]
        public void HbA1cが範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[6].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[6].Value1 = $"{HealthRecordValidator.PERCENTAGE_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 体脂肪率が範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[7].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[7].Value1 = $"{HealthRecordValidator.PERCENTAGE_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 水分率が範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[7].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[7].Value1 = $"{HealthRecordValidator.PERCENTAGE_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 酸素飽和度が範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[8].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[8].Value1 = $"{HealthRecordValidator.PERCENTAGE_MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void 身長が範囲外だったらNG()
        {
            var vitals = GetValidVitalvitals();
            // 最小値未満
            vitals[9].Value1 = "0";

            (var isValid, var error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            vitals[9].Value1 = $"{HealthRecordValidator.MAX_VALUE + 1}";

            (isValid, error) = _validator.Validate(vitals);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();
        }

        [TestMethod]
        public void Alert_心拍の検証()
        {
            var vitalType = QsDbVitalTypeEnum.Pulse;
            // 最小値未満
            var value1 = 0m;

            (var isValid, var error) = _validator.ValidateForAlert(vitalType, value1,0);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            value1 = HealthRecordValidator.THREE_DIGIT_MAX_VALUE + 1m;

            (isValid, error) = _validator.ValidateForAlert(vitalType, value1,0);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 少数あり
            value1 = 50.52m;

            (isValid, error) = _validator.ValidateForAlert(vitalType, value1, 0);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 正常値
            value1 = 100m;

            (isValid, error) = _validator.ValidateForAlert(vitalType, value1, 0);

            isValid.IsTrue();
        }

        [TestMethod]
        public void Alert_酸素飽和度の検証()
        {
            var vitalType = QsDbVitalTypeEnum.BloodOxygen;
            
            // 最小値未満
            var value1 = 0m;

            (var isValid, var error) = _validator.ValidateForAlert(vitalType,value1,0m);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 最大値を超える
            value1 = HealthRecordValidator.PERCENTAGE_MAX_VALUE + 1m;

            (isValid, error) = _validator.ValidateForAlert(vitalType, value1, 0m);

            isValid.IsFalse();
            error.Contains("Valueが不正です").IsTrue();

            // 正常値
            value1 = 99m;

            (isValid, _) = _validator.ValidateForAlert(vitalType, value1, 0m);

            isValid.IsTrue();
        }

        List<QhApiVitalValueItem> GetValidVitalvitals()
        {
            return new List<QhApiVitalValueItem>
            {
                new QhApiVitalValueItem
                {
                    RecordDate = DateTime.Today.ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Steps}",
                    Value1 = "3020"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.SleepingTime}",
                    Value1 = "480"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodPressure}",
                    Value1 = "120",
                    Value2 = "70"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyAge}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Pulse}",
                    Value1 = "80"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodSugar}",
                    Value1 = "40",
                    ConditionType = $"{(int)QsDbVitalConditionTypeEnum.Fasting}"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Glycohemoglobin}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyFatPercentage}",
                    Value1 = "16"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.TotalBodyWater}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodOxygen}",
                    Value1 = "90"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyHeight}",
                    Value1 = "62.5"
                },
            };
        }
    }
}
