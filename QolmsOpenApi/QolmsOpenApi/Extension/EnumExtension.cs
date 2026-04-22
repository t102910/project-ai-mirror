using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// Enumに関する拡張メソッド
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 指定したbyte値がEnumに定義されているかを確認します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(this byte value) where T:Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// 指定したint値がEnumに定義されているかを確認します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(this int value) where T : Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// byte値が指定のEnum値に定義済みであれば変換、未定義であればdefaultValueを返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryConvertOrDefault<T>(this byte value, T defaultValue) where T:Enum
        {
            if (!value.IsDefined<T>())
            {
                return defaultValue;
            }

            var convert = (T)Enum.ToObject(defaultValue.GetType(), value);

            return convert;
        }
    }
}