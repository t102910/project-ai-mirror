using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Enums
{
    public enum QoApiResultCodeTypeEnum : int
    {
        None = 0,
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        InternalServerError = 500,

        /// <summary>
        /// 汎用エラーです（コード定義に困ったらとりあえずこれ）。Enum定義追加を検討してください。
        /// </summary>
        GeneralError = 1001,
        /// <summary>
        /// 引数エラー
        /// </summary>
        ArgumentError = 1002,
        ///<summary>
        /// DBエラー
        ///</summary> 
        DatabaseError = 1003,
        /// <summary>
        /// ストレージエラー
        ///</summary> 
        StorageError = 1004,
        /// <summary>
        /// 実行エラー
        ///</summary> 
        OperationError = 1005,
        /// <summary>
        /// データ消失エラー
        ///</summary> 
        NotFoundError = 1006,


        /// <summary>
        /// 仮登録既定回数超過
        ///</summary> 
        SignUpMailAddressCountOver = 2001,
        /// <summary>
        /// 仮登録
        ///</summary> 
        SignUpGuidError = 2002,
        /// <summary>
        /// ユーザーID重複エラー
        ///</summary> 
        UserIdDuplicate = 2003,
        /// <summary>
        /// 本登録有効期限切れ
        ///</summary> 
        AccountRegisterExpired = 2004,
        /// <summary>
        /// メールアドレス使用済み（重複可否設定あり）
        ///</summary> 
        UsedMailAddress = 2005,

        /// <summary>
        /// ID問い合わせでユーザーIDが見つからなかった
        /// </summary>
        UserIdForgetNotFound = 2997,

        /// <summary>
        /// ID問い合わせでユーザーIDが2件以上見つかった
        /// </summary>
        UserIdForgetMultipleResults = 2998,

        /// <summary>
        /// 退会後規定時間以内に再登録
        /// </summary>
        RegisterIntervalError = 2999,

        /// <summary>
        /// 診察券番号重複エラー（同一施設で同一番号が使用済み）
        /// </summary>
        PatientCardDuplicate = 3000,

        /// <summary>
        /// 仮登録情報が存在しない
        /// </summary>
        SignUpInfoNotFound = 3001,

        /// <summary>
        /// 同じ登録情報のユーザーが既に存在する
        /// </summary>
        UserInfoDuplicate = 3002,

        /// <summary>
        /// QRコードの情報とDBのユーザー情報が一致しない
        /// </summary>
        PatientCardUserInfoInvalid = 3003,

        /// <summary>
        /// SMS認証コードの有効期限切れ
        /// </summary>
        SmsAuthCodeExpired = 3004,

        /// <summary>
        /// SMS認証コード試行回数オーバー
        /// </summary>
        SmsAuthCodeCountOver = 3005,

        /// <summary>
        /// SMS認証コードが不正
        /// </summary>
        SmsAuthCodeInvalid = 3006,

        /// <summary>
        /// アカウント電話番号は既に使用済み
        /// </summary>
        AccountPhoneDuplicate = 3007,

        /// <summary>
        /// アカウント電話番号が未登録
        /// </summary>
        AccountPhoneNotFound = 3008,

        /// <summary>
        /// アカウント生年月日が不一致
        /// </summary>
        AccountBirthDateMismatch = 3009,

        /// <summary>
        /// お薬手帳データ追加処理で登録は成功したが戻り値用のデータの取得で失敗
        /// </summary>
        NoteMedicineAddPartialSuccess = 4000,

        /// <summary>
        /// お薬手帳データが存在しない
        /// </summary>
        NoteMedicineDataNotFound = 4001,

        /// <summary>
        /// お薬手帳の編集対象のJahisの項目が存在しない
        /// </summary>
        NoteMedicineNoEditJahisData = 4002,

        /// <summary>
        /// OpenApi以外のシステムがメンテナンス中の場合（OpenApiが落ちていたらそもそも応答できない）
        ///</summary> 
        Maintenance = 9990,

        /// <summary>
        /// 不明なエラー
        ///</summary> 
        UnknownError = 9999
    }

}