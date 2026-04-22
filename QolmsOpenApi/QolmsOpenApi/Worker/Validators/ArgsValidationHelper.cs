using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 引数チェックに関するヘルパー
    /// </summary>
    public static class ArgsValidationHelper
    {
        /// <summary>
        /// 必須チェックを行う
        /// </summary>
        /// <param name="value">対象の値</param>
        /// <param name="name">引数のパラメータ名</param>
        /// <param name="result">結果引数クラス</param>
        /// <returns>チェック通過ならTrue、NGならFalseを返しresultに判定結果が入る</returns>
        public static bool CheckArgsRequired(this string value, string name, QoApiResultsBase result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{name}は必須です。");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 変換可能かチェックする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">対象の値</param>
        /// <param name="name">引数のパラメータ名</param>
        /// <param name="defaultValue">規定値(NGにする値)</param>
        /// <param name="result">結果引数クラス</param>
        /// <param name="converted">変換後の値</param>
        /// <returns>変換できたらTrue, 失敗したらFalseを返し結果引数にメッセージが格納される</returns>
        public static bool CheckArgsConvert<T>(this string value, string name, T defaultValue, QoApiResultsBase result, out T converted) where T: struct
        {
            converted = value.TryToValueType<T>(defaultValue);
            if (converted.Equals(defaultValue))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{name}が不正です。");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enum値に変換可能か、または定義内かをチェックする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <param name="ngValue">NGの値</param>
        /// <param name="result"></param>
        /// <param name="converted"></param>
        /// <returns>変換できたらTrue, 失敗したらFalseを返し結果引数にメッセージが格納される</returns>
        public static bool CheckArgsEnumConvert<T>(this byte value, string name, T ngValue, QoApiResultsBase result, out T converted) where T:Enum
        {
            converted = (T)Enum.ToObject(typeof(T), value);

            if (!value.IsDefined<T>())
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{name}が不正です。");
                return false;
            }

            if(converted.Equals(ngValue))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{name}が不正です。");
                return false;
            }

            return true;            
        }
    }
}