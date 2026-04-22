using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Jahisフォーマットデータを作成するビルダー
    /// </summary>
    public class JahisMessageBuilder
    {

        private JM_Header _header = new JM_Header
        {
            Header_1 = $"JAHISTC{(int)JMVersionTypeEnum.Latest:00}",
            Header_2 = "2"
        };

        private JM_No001 _patientData = new JM_No001();

        private List<JM_Prescription> _prescriptions = new List<JM_Prescription>();
        private List<JM_No003> _otcNo003List = new List<JM_No003>();
        private List<JM_No004> _noteMemoNo004List = new List<JM_No004>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="jahisVersion">Jahisバージョン</param>
        /// <param name="outputKbn">出力区分 1:医療機関・薬局から患者等に 2: 患者等から医療機関・薬局に</param>
        public JahisMessageBuilder(JMVersionTypeEnum jahisVersion, int outputKbn = 2)
        {
            _header = new JM_Header
            {
                Header_1 = $"JAHISTC{(int)jahisVersion:00}",
                Header_2 = $"{outputKbn}"
            };               
        }      


        /// <summary>
        /// 患者情報レコードを追加する
        /// </summary>
        /// <param name="name">氏名</param>
        /// <param name="kana">カナ</param>
        /// <param name="sexType">性別 1:男性 2：女性</param>
        /// <param name="birthDate">生年月日</param>
        /// <returns></returns>
        public JahisMessageBuilder SetPatientData(string name, string kana, byte sexType, DateTime birthDate)
        {
            _patientData = new JM_No001
            {
                No001_1 = "1",
                No001_2 = name,
                No001_3 = sexType == 1 ? "1" : "2",
                No001_4 = birthDate.ToString("yyyyMMdd"),
                No001_5 = string.Empty, // 郵便番号
                No001_6 = string.Empty, // 住所
                No001_7 = string.Empty, // 電話番号
                No001_8 = string.Empty, // 患者連絡先
                No001_9 = string.Empty, // 血液型
                No001_10 = string.Empty, // 体重
                No001_11 = kana,                
            };

            return this;
        }


        /// <summary>
        /// 市販薬情報を追加する
        /// </summary>
        /// <param name="medicineName"></param>
        /// <returns></returns>
        public JahisMessageBuilder AddOtcMedicine(string medicineName)
        {
            _otcNo003List.Add(new JM_No003
            {
                No003_1 = "3",
                No003_2 = medicineName,
                No003_3 = string.Empty,
                No003_4 = string.Empty,
                No003_5 = "2",                
            });
            return this;
        }

        /// <summary>
        /// 手帳メモの設定
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public JahisMessageBuilder SetNoteMemo(params string[] memo)
        {
            _noteMemoNo004List = memo.Select(x => new JM_No004
            {
                No004_1 = "4",
                No004_2 = x,
                No004_3 = DateTime.Today.ToString("yyyyMMdd"),
                No004_4 = "2"
            }).ToList();

            return this;
        }

        /// <summary>
        /// 処方情報を追加
        /// </summary>
        /// <param name="preBuilder"></param>
        /// <returns></returns>
        public JahisMessageBuilder AddPrescription(Action<JahisPrescriptionBuilder> preBuilder)
        {
            var builder = new JahisPrescriptionBuilder();
            preBuilder(builder);

            _prescriptions.Add(builder.Build());

            return this;
        }

        /// <summary>
        /// Jahisフォーマットデータを生成する
        /// </summary>
        /// <returns></returns>
        public JM_Message Build()
        {
            return new JM_Message(_header, _patientData, new List<JM_No002>(),_otcNo003List, _noteMemoNo004List, _prescriptions, new List<JM_No701>());
        }

        /// <summary>
        /// Buildしてシリアライズして暗号化した文字列を返す
        /// </summary>
        /// <returns></returns>
        public string BuildCrypted()
        {
            var message = Build();
            var json = new QsJsonSerializer().Serialize(message);
            return json.TryEncrypt();
        }
    }

    /// <summary>
    /// 処方情報を生成するビルダー
    /// </summary>
    public class JahisPrescriptionBuilder
    {
        private JM_No005 _dateRecNo005;
        private JM_No011 _yakuhinRecNo011;
        private JM_No015 _dispensingRecNo015;
        private JM_No051 _hospitalRecNo051;
        private List<JM_No421> _zanyakuNo421List = new List<JM_No421>();
        private List<JM_No501> _bikouNo501List = new List<JM_No501>();
        private List<JM_No601> _memoNo601List = new List<JM_No601>();

        private List<JM_RpSet> _rpSetList = new List<JM_RpSet>();
        

        /// <summary>
        /// 調剤等年月日を設定
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetDate(DateTime date)
        {
            _dateRecNo005 = new JM_No005
            {
                No005_1 = "5",
                No005_2 = date.ToString("yyyyMMdd"),
                No005_3 = "2"
            };

            return this;
        }

        /// <summary>
        /// 薬局・薬剤師を設定
        /// </summary>
        /// <param name="pharmacyName"></param>
        /// <param name="pharmacistName"></param>
        /// <param name="pharmacyNo"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetPharmacy(string pharmacyName, string pharmacistName, int pharmacyNo = 0)
        {
            var pharmacyNoStr = pharmacyNo <= 0 ? string.Empty : pharmacyNo.ToString();

            _yakuhinRecNo011 = new JM_No011
            {
                No011_1 = "11",
                No011_2 = pharmacyName,
                No011_3 = string.Empty,
                No011_4 = string.Empty,
                No011_5 = pharmacyNoStr,
                No011_6 = string.Empty,
                No011_7 = string.Empty,
                No011_8 = string.Empty,
                No011_9 = "2"
            };

            _dispensingRecNo015 = new JM_No015
            {
                No015_1 = "15",
                No015_2 = pharmacistName,
                No015_3 = string.Empty,
                No015_4 = "2"
            };

            return this;
        }

        /// <summary>
        /// 病院を設定
        /// </summary>
        /// <param name="hospitalName"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetHospital(string hospitalName, string hospitalId = "")
        {
            _hospitalRecNo051 = new JM_No051
            {
                No051_1 = "51",
                No051_2 = hospitalName,
                No051_3 = string.Empty,
                No051_4 = string.Empty,
                No051_5 = hospitalId,
                No051_6 = "2"
            };
            
            return this;
        }

        /// <summary>
        /// 患者等記入情報を設定
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetPatientMemo(params string[] memo)
        {
            _memoNo601List = memo.Select(x => new JM_No601
            {
                No601_1 = "601",
                No601_2 = x,
                No601_3 = DateTime.Today.ToString("yyyyMMdd")
            }).ToList();
            return this;
        }

        /// <summary>
        /// 残薬内容を設定
        /// </summary>
        /// <param name=""></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetZanyaku(params string[] memo)
        {
            _zanyakuNo421List = memo.Select(x => new JM_No421
            {
                No421_1 = "421",
                No421_2 = x,
                No421_3 = "2"
            }).ToList();

            return this;
        }

        /// <summary>
        /// 備考情報を設定
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder SetBikou(params string[] memo)
        {
            _bikouNo501List = memo.Select(x => new JM_No501
            {
                No501_1 = "501",
                No501_2 = x,
                No501_3 = "2"
            }).ToList();

            return this;
        }

        /// <summary>
        /// 調剤情報を追加
        /// </summary>
        /// <param name="rpsetConfig"></param>
        /// <returns></returns>
        public JahisPrescriptionBuilder AddRpSet(Action<JahisRpSetBuilder> rpsetConfig)
        {
            var builder = new JahisRpSetBuilder();
            rpsetConfig(builder);
            _rpSetList.Add(builder.Build());
            return this;
        }

        

        /// <summary>
        /// 処方情報を生成する
        /// </summary>
        /// <returns></returns>
        public JM_Prescription Build()
        {
            var pre = new JM_Prescription(_dateRecNo005,_yakuhinRecNo011,_dispensingRecNo015,_hospitalRecNo051,_rpSetList,null,null,_bikouNo501List,_memoNo601List);

            pre.No421_List = _zanyakuNo421List;

            return pre;
        }
    }

    /// <summary>
    /// 調剤情報を生成するビルダー
    /// </summary>
    public class JahisRpSetBuilder
    {
        private string _department;
        private string _doctor;
        private List<JM_Rp> _rpList = new List<JM_Rp>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="doctor"></param>
        /// <param name="department"></param>
        public JahisRpSetBuilder(string doctor = null, string department = null)
        {
            if (doctor != null)
            {
                SetDoctor(doctor);
            }
            if (department != null)
            {
                SetDepartment(department);
            }
        }

        /// <summary>
        /// 医師を設定
        /// </summary>
        /// <param name="docotor"></param>
        /// <returns></returns>
        public JahisRpSetBuilder SetDoctor(string docotor)
        {
            _doctor = docotor;
            return this;
        }

        /// <summary>
        /// 診療科を設定
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public JahisRpSetBuilder SetDepartment(string department)
        {
            _department = department;
            return this;
        }

        /// <summary>
        /// RP情報を追加
        /// </summary>
        /// <param name="rpConfig"></param>
        /// <returns></returns>
        public JahisRpSetBuilder AddRp(Action<JahisRpBuilder> rpConfig)
        {
            var builder = new JahisRpBuilder(_rpList.Count+1);
            rpConfig(builder);

            _rpList.Add(builder.Build());

            return this;
        }

        /// <summary>
        /// 調剤情報を生成する
        /// </summary>
        /// <returns></returns>
        public JM_RpSet Build()
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            return new JM_RpSet
            {
                No055 = new JM_No055
                {
                    No055_1 = "55",
                    No055_2 = _doctor,
                    No055_3 = _department,
                    No055_4 = "2",
                },
                Rp_List = _rpList
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        }
    }


    /// <summary>
    /// Rpデータを作成するビルダー
    /// </summary>
    public class JahisRpBuilder
    {
        private string _usageName = string.Empty;
        private List<string> _usageSuppliement = new List<string>();
        private string _dosageFormCode = string.Empty;
        private int _quantity;
        private string _quantityUnit = string.Empty;
        private List<JM_Medicine> _medicineList = new List<JM_Medicine>();
        private int _rpNo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rpNo">RP番号</param>
        public JahisRpBuilder(int rpNo)
        {
            _rpNo = rpNo;
        }

        /// <summary>
        /// 用法・用法補足を設定
        /// </summary>
        /// <param name="usageName">用法名</param>
        /// <param name="usageSuppliment">用法補足</param>
        /// <returns></returns>
        public JahisRpBuilder SetUsage(string usageName, params string[] usageSuppliment)
        {
            _usageName = usageName;
            _usageSuppliement = usageSuppliment.ToList();
            return this;
        }

        /// <summary>
        /// 数量・単位を設定
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public JahisRpBuilder SetQuantity(int quantity, string unit)
        {
            _quantity = quantity;
            _quantityUnit = unit;
            return this;
        }

        /// <summary>
        /// 剤型コードを設定
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public JahisRpBuilder SetDosageFormCode(string code)
        {
            _dosageFormCode = code;
            return this;
        }

        /// <summary>
        /// 薬品レコードを追加
        /// </summary>
        /// <param name="name">薬品名</param>
        /// <param name="quantity">容量</param>
        /// <param name="quantityUnit">単位名</param>
        /// <param name="yjCode">YJコード</param>
        /// <returns></returns>
        public JahisRpBuilder AddMedicine(string name, string quantity, string quantityUnit, string yjCode)
        {

#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var medicine = new JM_Medicine
            {
                No201 = new JM_No201
                {
                    No201_1 = "201",
                    No201_2 = $"{_rpNo}",
                    No201_3 = name,
                    No201_4 = quantity,
                    No201_5 = quantityUnit,
                    No201_6 = string.IsNullOrEmpty(yjCode) ? "" : "4",
                    No201_7 = yjCode,
                    No201_8 = "2" // レコード作成者 1:医療関係者 2:患者
                }
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            _medicineList.Add(medicine);

            return this;
        }

        /// <summary>
        /// RPデータを生成する
        /// </summary>
        /// <returns></returns>
        public JM_Rp Build()
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            return new JM_Rp
            {                
                No301 = new JM_No301
                {
                    No301_1 = "301",
                    No301_2 = _rpNo.ToString(),
                    No301_3 = _usageName,
                    No301_4 = _quantity.ToString(),
                    No301_5 = _quantityUnit,
                    No301_6 = _dosageFormCode,
                    No301_7 = "1",
                    No301_8 = "",
                    No301_9 = "2" // レコード作成者 1:医療関係者 2:患者
                },
                No311_List = _usageSuppliement.Select(x => new JM_No311
                {
                    No311_1 = "311",
                    No311_2 = _rpNo.ToString(),
                    No311_3 = x,
                    No311_4 = "2"
                }).ToList(),
                No391_List = new List<JM_No391>(),
                Medicine_List = _medicineList
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        }

    }
}