using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Enums
{
    /// <summary>
    /// QolmsOpenApiの利用可能な機能権限を表現します。
    /// アクセスキーに組み込まれており、アクセスキーに対応した権限がAPIにあるかを照合する
    /// 権限を絞り込むにシステムごとにアクセスキーを生成する必要がある。
    /// QoAccessKeyGenerateApiは無条件でAllになっているのでそれを使う限りは
    /// 権限の絞り込みはできないので注意が必要。
    /// </summary>
    [Flags()]
    public enum QoApiFunctionTypeEnum : int
    {
        None            = 0,
        AccessKey       = 1 << 0,
        Master          = 1 << 1,
        HealthRecord    = 1 << 2,
        NoteMedicine    = 1 << 3,
        Point           = 1 << 4,
        Facility        = 1 << 5,
        Prescription    = 1 << 6,
        Calendar        = 1 << 7,
        Assessment      = 1 << 8,
        ExerciseEvent   = 1 << 9,
        JotoHdr         = 1 << 10,
        Linkage         = 1 << 11,
        Notification    = 1 << 12,
        Support         = 1 << 13,
        Tis             = 1 << 14,
        Contact         = 1 << 15,
        Examination     = 1 << 16,
        Kagamino        = 1 << 17,
        MedicalNavi     = 1 << 18,
        Waiting         = 1 << 19,
        Line            = 1 << 20,
        /// <summary>
        /// 心拍見守りアプリ
        /// </summary>
        HeartMonitor    = 1 << 21,

        CheckupFile = 1 << 22,
        Push = 1 << 23,
        /// <summary>
        /// 標準病名マスタ
        /// </summary>
        Disease = 1 << 24,

        /// <summary>
        /// JOTOネイティブアプリ向け
        /// </summary>
        JotoApp = 1 << 25,
        
        /// <summary>
        /// ゲスト用        
        /// </summary>
        Guest = 1073741824,  // 1 << 30 なので使用可能フラグはここまで

        // 以下はビットフィールドではありません。定義を追加したら以下も変更してください。

        // ''' <summary>
        // ''' システム権限です。
        // ''' </summary>
        // System = AccessKey Or HealthRecord Or NoteMedicine Or Point

        /// <summary>
        /// ゲストユーザ権限です。
        /// </summary>
        GuestUser = QoApiFunctionTypeEnum.Guest | QoApiFunctionTypeEnum.Master,
        /// <summary>
        /// CCC薬局ユーザー権限です。
        /// </summary>
        CccPharmacyUser = QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Facility | QoApiFunctionTypeEnum.Prescription,

        /// <summary>
        /// CCCお薬手帳アプリユーザー権限です。
        /// </summary>
        CccAppUser = QoApiFunctionTypeEnum.AccessKey | QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.NoteMedicine | QoApiFunctionTypeEnum.Prescription | QoApiFunctionTypeEnum.Calendar | QoApiFunctionTypeEnum.Assessment,

        /// <summary>
        /// JOTOホームドクターアプリユーザー権限です。
        /// </summary>
        /// <remarks></remarks>
        JotoHdrAppUser = QoApiFunctionTypeEnum.ExerciseEvent | QoApiFunctionTypeEnum.JotoHdr | QoApiFunctionTypeEnum.HeartMonitor | QoApiFunctionTypeEnum.Push,

        /// <summary>
        /// 全権限です。
        /// </summary>
        All = QoApiFunctionTypeEnum.AccessKey | QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.HealthRecord | QoApiFunctionTypeEnum.NoteMedicine | 
            QoApiFunctionTypeEnum.Point | QoApiFunctionTypeEnum.Facility | QoApiFunctionTypeEnum.Prescription | QoApiFunctionTypeEnum.Calendar | 
            QoApiFunctionTypeEnum.Assessment | QoApiFunctionTypeEnum.Linkage | QoApiFunctionTypeEnum.Notification | QoApiFunctionTypeEnum.Support |
            QoApiFunctionTypeEnum.Tis | QoApiFunctionTypeEnum.Contact | QoApiFunctionTypeEnum.Examination | QoApiFunctionTypeEnum.Kagamino |
            QoApiFunctionTypeEnum.MedicalNavi | QoApiFunctionTypeEnum.Waiting | QoApiFunctionTypeEnum.HeartMonitor | QoApiFunctionTypeEnum.Line |
            QoApiFunctionTypeEnum.CheckupFile | QoApiFunctionTypeEnum.Push | QoApiFunctionTypeEnum.Disease | QoApiFunctionTypeEnum.JotoApp
    }

}
