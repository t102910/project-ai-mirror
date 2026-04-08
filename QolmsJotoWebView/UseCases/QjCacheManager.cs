using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// JOTO WebView で使用する、
    /// キャッシュ機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class QjCacheManager
    {
        #region "Constant"

        /// <summary>
        /// キャッシュを表します。
        /// </summary>
        private readonly Dictionary<QjCacheTypeEnum, Dictionary<object, object>> caches = new Dictionary<QjCacheTypeEnum, Dictionary<object, object>>() {
            {QjCacheTypeEnum.ModelProperty, new Dictionary<object, object>()},
            {QjCacheTypeEnum.InputModel, new Dictionary<object, object>()},
            {QjCacheTypeEnum.PostedFile, new Dictionary<object, object>()},
            {QjCacheTypeEnum.Thumbnail, new Dictionary<object, object>()}
        };

        #endregion

        #region "Constructor"

        public QjCacheManager() : base() { }

        #endregion

        #region "Private Method"

        /// <summary>
        /// オブジェクトのディープ コピーを作成します。
        /// </summary>
        /// <typeparam name="T">コピーするオブジェクトの型。</typeparam>
        /// <param name="target">コピー対象のオブジェクト。</param>
        /// <param name="refValue">コピーされたオブジェクト。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        private bool Copy<T>(object target, ref T refValue)
        {
            bool result = false;

            if (target.GetType().IsValueType)
            {
                try
                {
                    refValue = (T)target;
                    result = true;
                }
                catch 
                {
                }
            }
            else
            {
                if (target.GetType().IsSerializable)
                {
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            var bformt = new BinaryFormatter();
                            bformt.Serialize(stream, target);

                            stream.Position = 0;

                            refValue = (T)bformt.Deserialize(stream);
                            result = true;
                        }
                    }
                    catch 
                    {
                    }
                }
                else
                {
                    throw new InvalidOperationException("オブジェクトはシリアル化できません。");
                }
            }

            return result;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// キャッシュから指定した種別の値を取得します。
        /// </summary>
        /// <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
        /// <typeparam name="TValue">値の型。</typeparam>
        /// <param name="cacheType">キャッシュの種別。</param>
        /// <param name="key">キー。</param>
        /// <param name="refValue">取得した値が格納される変数。</param>
        /// <returns></returns>
        public bool GetCache<TKey,TValue>(QjCacheTypeEnum cacheType,TKey key, ref TValue refValue)
        {
            bool result = false;
            Type keyType = key.GetType();

            if ((keyType.IsValueType || keyType == typeof(string))
                && this.caches.ContainsKey(cacheType) 
                && this.caches[cacheType].ContainsKey(key))
            {
                result = this.Copy<TValue>(this.caches[cacheType][key],ref refValue);
            }

            return result;
        }

        /// <summary>
        /// キャッシュから指定した種別の全てのキーを取得します。
        /// </summary>
        /// <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
        /// <param name="cacheType">キャッシュの種別。</param>
        /// <returns>キーのリスト。</returns>
        public List<TKey> GetCacheKeys<TKey>(QjCacheTypeEnum cacheType)
        {
            List<TKey> result = new List<TKey>();

            if (this.caches.ContainsKey(cacheType) && this.caches[cacheType].Count > 0 )
            {
                this.caches[cacheType].Keys.ToList()
                    .ForEach(
                        i => {
                            TKey returnKey = default(TKey);
                            if (this.Copy<TKey>(i,ref returnKey))
                            {
                                result.Add(returnKey);
                            }
                        } 
                    );
            }

            return result;
        }

        /// <summary>
        /// キャッシュへ指定した種別の値を追加します。
        /// </summary>
        /// <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
        /// <typeparam name="TValue">値の型</typeparam>
        /// <param name="cacheType">キャッシュの種別</param>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public bool SetCache<TKey, TValue>(QjCacheTypeEnum cacheType, TKey key, TValue value)
        { 
            bool result = false;
            Type keyType = key.GetType();

            if (keyType.IsValueType || keyType == typeof(string)
                &&  this.caches.ContainsKey(cacheType)
                &&  !this.caches[cacheType].ContainsKey(key)
                && value!= null)
            {
                TKey copyKey = default(TKey);
                TValue copyValue = default(TValue);

                if (this.Copy<TKey>(key,ref copyKey) && this.Copy<TValue>(value, ref copyValue))
                {
                    this.caches[cacheType].Add(copyKey, copyValue);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// キャッシュから指定した種別の値を削除します。
        /// </summary>
        /// <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
        /// <param name="cacheType">キャッシュの種別</param>
        /// <param name="key">キー</param>
        public void RemoveCache<TKey>(QjCacheTypeEnum cacheType, TKey key) 
        {
            Type keyType = key.GetType();

            if ((keyType.IsValueType || keyType == typeof(string))
                && this.caches.ContainsKey(cacheType)
                && this.caches[cacheType].ContainsKey(key))
            {
                this.caches[cacheType].Remove(key);
            }
        }

        /// <summary>
        /// キャッシュから指定した種別の全ての値を削除します。
        /// </summary>
        /// <param name="cacheType">キャッシュの種別</param>
        public void ClearCache(QjCacheTypeEnum cacheType)
        {
            if (this.caches.ContainsKey(cacheType))
            {
                this.caches[cacheType].Clear();
            }
        }

        #endregion
    }
}