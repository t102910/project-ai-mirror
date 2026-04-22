using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.JAHISMedicineEntityV1;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;

namespace QolmsOpenApi.Tests.Workers
{
    /// <summary>
    /// Jahisフォーマットビルダー(テスト用)
    /// こちらは暫定版のためMGF.QOLMS.QolmsOpenApi.Worker.JahisMessageBuilderの方の使用を推奨します。
    /// </summary>
    public class JahisMessageTestBuilder
    {
        private List<JM_RpSet> _rpSetList = new List<JM_RpSet>();
        private List<JM_No003> _otcList = new List<JM_No003>();
        private List<string> _memoList = new List<string>();
        private List<string> _noteMemoList = new List<string>();
        private string _pharmacy;
        private string _pharmacist;
        private string _hospital;
        private string _date;

        public JahisMessageTestBuilder(DateTime? recordDate = null)
        {
            if (recordDate.HasValue)
            {
                SetDate(recordDate.Value);
            }
        }

        public JahisMessageTestBuilder SetDate(DateTime date)
        {
            _date = date.ToString("yyyyMMdd");
            return this;
        }

        public JahisMessageTestBuilder SetPatientMemo(params string[] memo)
        {
            _memoList = memo.ToList();
            return this;
        }

        public JahisMessageTestBuilder SetNoteMemo(params string[] memo)
        {
            _noteMemoList = memo.ToList();
            return this;
        }

        public JahisMessageTestBuilder SetPharmacy(string pharmacyName, string pharmacistName)
        {
            _pharmacy = pharmacyName;
            _pharmacist = pharmacistName;
            return this;
        }

        public JahisMessageTestBuilder SetHospital(string hospitalName)
        {
            _hospital = hospitalName;
            return this;
        }

        public JahisMessageTestBuilder AddRpSet(Action<JahisRpSetBuilder> rpsetConfig)
        {
            var builder = new JahisRpSetBuilder();
            rpsetConfig(builder);
            _rpSetList.Add(builder.Build());
            return this;
        }

        public JahisMessageTestBuilder AddOtcMedicine(string medicineName)
        {
            _otcList.Add(new JM_No003
            {
                No003_2 = medicineName
            });
            return this;
        }

        public JM_Message Build()
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            return new JM_Message
            {
                No003_List = _otcList,
                No004_List = _noteMemoList.ConvertAll(x => new JM_No004
                {
                    No004_2 = x
                }),                
                Prescription_List = new List<JM_Prescription>
                {                    
                    new JM_Prescription
                    {
                        No601_List = _memoList.ConvertAll(x => new JM_No601
                        {
                            No601_2 = x
                        }),
                        No005 = new JM_No005
                        {
                            No005_2 = _date
                        },
                        No011 = new JM_No011
                        {
                            No011_2 = _pharmacy,
                        },
                        No015 = new JM_No015
                        {
                            No015_2 = _pharmacist
                        },
                        No051 = new JM_No051
                        {
                            No051_2 = _hospital,
                        },
                        RpSet_List = _rpSetList,
                    }
                }
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
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

    public class JahisRpSetBuilder
    {
        private string _department;
        private string _doctor;
        private List<JM_Rp> _rpList = new List<JM_Rp>();

        public JahisRpSetBuilder(string doctor = null, string department = null)
        {
            if(doctor != null)
            {
                SetDoctor(doctor);
            }
            if(department != null)
            {
                SetDepartment(department);
            }
        }

        public JahisRpSetBuilder SetDoctor(string docotor)
        {
            _doctor = docotor;            
            return this;
        }

        public JahisRpSetBuilder SetDepartment(string department)
        {
            _department = department;
            return this;
        }

        public JahisRpSetBuilder AddRp(Action<JahisRpBuilder> rpConfig) 
        {
            var builder = new JahisRpBuilder();
            rpConfig(builder);

            _rpList.Add(builder.Build());

            return this;
        }

        public JM_RpSet Build()
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            return new JM_RpSet
            {
                No055 = new JM_No055
                {
                    No055_2 = _doctor,
                    No055_3 = _department,
                },
                Rp_List = _rpList
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        }
    }

    public class JahisRpBuilder
    {
        private string _usageName = string.Empty;
        private List<string> _usageSuppliement = new List<string>();
        private string _dosageFormCode = string.Empty;
        private int _quantity;
        private string _quantityUnit = string.Empty;
        private List<JM_Medicine> _medicineList = new List<JM_Medicine>();
        
        public JahisRpBuilder SetUsage(string usageName, params string[] usageSuppliment)
        {
            _usageName = usageName;
            _usageSuppliement = usageSuppliment.ToList();
            return this;
        }

        public JahisRpBuilder SetQuantity(int quantity, string unit)
        {
            _quantity = quantity;
            _quantityUnit = unit;
            return this;
        }

        public JahisRpBuilder SetDosageFormCode(string code)
        {
            _dosageFormCode = code;
            return this;
        }

        public JahisRpBuilder AddMedicine(string name, string quantity, string quantityUnit, string yjCode)
        {

#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var medicine = new JM_Medicine
            {
                No201 = new JM_No201
                {
                    No201_3 = name,
                    No201_4 = quantity,
                    No201_5 = quantityUnit,
                    No201_6 = string.IsNullOrEmpty(yjCode) ? "" : "4",
                    No201_7 = yjCode
                }
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            _medicineList.Add(medicine);

            return this;
        }

        public JM_Rp Build()
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            return new JM_Rp
            {
                No301 = new JM_No301
                {
                    No301_3 = _usageName,
                    No301_6 = _dosageFormCode,
                    No301_4 = _quantity.ToString(),
                    No301_5 = _quantityUnit
                },
                No311_List = _usageSuppliement.Select(x => new JM_No311
                {
                    No311_3 = x
                }).ToList(),
                Medicine_List = _medicineList
            };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        }       

    }    

}
