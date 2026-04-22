using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// QsQryptの暗号化・復号化を補助する拡張メソッド
    /// </summary>
    public static class QsCryptExtension
    {
        /// <summary>
        /// QolmsSystemで暗号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TryEncrypt(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    return crypt.EncryptString(source);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 暗号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="crypt"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TryEncrypt(this QsCrypt crypt, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }
            try
            {
                
                return crypt.EncryptString(source);                
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 復号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="crypted"></param>
        /// <returns></returns>
        public static string TryDecrypt(this string crypted)
        {
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    return crypt.DecryptString(crypted);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// QolmsSystemで復号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="crypt"></param>
        /// <param name="cryptedSource"></param>
        /// <returns></returns>
        public static string TryDecrypt(this QsCrypt crypt, string cryptedSource)
        {
            try
            {                
                return crypt.DecryptString(cryptedSource);                
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// QolmsWebで暗号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TryEncryptForWeb(this string source)
        {
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                {
                    return crypt.EncryptString(source);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// QolmsWebで復号化を試みます。
        /// 失敗した場合は空文字を返します。
        /// </summary>
        /// <param name="crypted"></param>
        /// <returns></returns>
        public static string TryDecryptForWeb(this string crypted)
        {
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                {
                    return crypt.DecryptString(crypted);
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}