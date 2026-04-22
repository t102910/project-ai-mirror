using System;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{

    internal static class QoEncryptedReferenceConverterExtension
    {

        /// <summary>
        /// セパレータ文字を表します。
        /// </summary>
        /// <remarks></remarks>
        private const char SEPARATOR = ':';

        /// <summary>
        /// タイムスタンプの日付フォーマットを表します。
        /// </summary>
        /// <remarks></remarks>
        private const string TIMESTAMP_FORMAT = "fffssmmHHddMMyyyy";

        /// <summary>
        /// キー参照文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ToEncrypedReference(this string target, Nullable<DateTime> timestamp = null, QsCrypt crypt = null)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(target))
            {
                if (crypt == null)
                    crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb);

                if (timestamp == null)
                    result = crypt.EncryptString(target);
                else
                    result = crypt.EncryptString(string.Format("{0}{1}{2}", timestamp.Value.ToString(QoEncryptedReferenceConverterExtension.TIMESTAMP_FORMAT), QoEncryptedReferenceConverterExtension.SEPARATOR, target));
            }

            return result;
        }

        /// <summary>
        /// キー参照文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ToEncrypedReference(this Guid target, Nullable<DateTime> timestamp = null, QsCrypt crypt = null)
        {
            string result = string.Empty;

            if (target != default(Guid) && target != Guid.Empty)
            {
                if (crypt == null)
                    crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb);

                if (timestamp == null)
                    result = crypt.EncryptString(target.ToApiGuidString());
                else
                    result = crypt.EncryptString(string.Format("{0}{1}{2}", timestamp.Value.ToString(QoEncryptedReferenceConverterExtension.TIMESTAMP_FORMAT), QoEncryptedReferenceConverterExtension.SEPARATOR, target.ToApiGuidString()));
            }

            return result;
        }

        /// <summary>
        /// キー参照文字列を元の文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ToDecrypedReference(this string target, ref Nullable<DateTime> timestamp)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(target))
            {
                try
                {
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                    {
                        string[] elements = crypt.DecryptString(target).Split(QoEncryptedReferenceConverterExtension.SEPARATOR);
                        result = elements.Last();

                        if (timestamp != null)
                        {
                            if (elements.Count() > 1)
                            {
                                if (DateTime.TryParseExact(elements.First(), QoEncryptedReferenceConverterExtension.TIMESTAMP_FORMAT, null, System.Globalization.DateTimeStyles.None, out DateTime d))
                                    timestamp = d;
                            }
                            else
                                // タイムスタンプを含まない参照文字列は無期限で有効
                                timestamp = DateTime.MaxValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                }
                
            }

            return result;
        }
        /// <summary>
        /// キー参照文字列を元の文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        public static string ToDecrypedReference(this string target)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(target))
            {
                try
                {
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                    {
                        string[] elements = crypt.DecryptString(target).Split(QoEncryptedReferenceConverterExtension.SEPARATOR);
                        result = elements.Last();
                    }
                }
                catch (Exception ex)
                {
                    QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                }

            }

            return result;
        }
        /// <summary>
        /// キー参照文字列を元の文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static T ToDecrypedReference<T>(this string target, ref Nullable<DateTime> timestamp ) where T : struct
        {
            T result;

            if (!string.IsNullOrWhiteSpace(target))
            {
                try
                {
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                    {
                        string[] elements = crypt.DecryptString(target).Split(QoEncryptedReferenceConverterExtension.SEPARATOR);
                        result = elements.Last().ToValueType<T>();

                        if (timestamp != null)
                        {
                            if (elements.Count() > 1)
                            {
                                if (DateTime.TryParseExact(elements.First(), QoEncryptedReferenceConverterExtension.TIMESTAMP_FORMAT, null, System.Globalization.DateTimeStyles.None, out DateTime d))
                                    timestamp = d;
                            }
                            else
                                // タイムスタンプを含まない参照文字列は無期限で有効
                                timestamp = DateTime.MaxValue;
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                }

            }

            return new T();
        }
        /// <summary>
        /// キー参照文字列を元の文字列へ変換します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static T ToDecrypedReference<T>(this string target) where T : struct 
        {
            if (!string.IsNullOrWhiteSpace(target))
            {
                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                {
                    try
                    {
                        string[] elements = crypt.DecryptString(target).Split(QoEncryptedReferenceConverterExtension.SEPARATOR);
                        return elements.Last().ToValueType<T>();
                    }
                    catch (Exception ex)
                    {
                        QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                    }
                }
            }

            return new T();
        }
    }

}