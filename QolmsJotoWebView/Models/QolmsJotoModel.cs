using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// QolmsJotoWebView で使用する メインモデル
    /// このクラスは継承できません。
    /// </summary>
    public sealed class QolmsJotoModel
    {
        #region "Constant"

        /// <summary>
        /// キャッシュ機能を提供します。
        /// </summary>
        private readonly QjCacheManager cacheManager = new QjCacheManager();

        #endregion

        #region "Variable"

        #region "検証用情報"

        /// <summary>
        /// JavaScript が有効かを保持します。
        /// </summary>
        private bool _enableJavaScript = false;

        /// <summary>
        /// クッキーが有効かを保持します。
        /// </summary>
        private bool _enableCookies = false;

        /// <summary>
        /// デバッグ ビルドかを保持します。
        /// </summary>
        private bool _isDebug = false;

        /// <summary>
        /// デバッグログのパスを保持します。
        /// </summary>
        private string _debugLogPath = string.Empty;

        #endregion

        #region "Web API 認証情報"

        /// <summary>
        /// セッション ID を保持します。
        /// </summary>
        private string _sessionId = string.Empty;

        /// <summary>
        /// QolmsApi 用 API 認証 キー を保持します。
        /// </summary>
        private Guid _apiAuthorizeKey = Guid.Empty;

        /// <summary>
        /// QolmsApi 用 API 認証有効期限を保持します。
        /// </summary>
        private DateTime _apiAuthorizeExpires = DateTime.MinValue;

        /// <summary>
        /// QolmsJotoApi 用 API 認証 キー を保持します。
        /// </summary> 
        private Guid _apiAuthorizeKey2 = Guid.Empty;

        /// <summary>
        /// QolmsJotoApi 用 API 認証有効期限を保持します。
        /// </summary>
        private DateTime _apiAuthorizeExpires2 = DateTime.MinValue;

        #endregion

        #region "アカウント情報"

        /// <summary>
        /// 所有者アカウント情報を保持します。
        /// </summary>
        private AuthorAccountItem _authorAccount = null;

        #endregion

        #region "その他"

        /// <summary>
        /// セッション内での各画面の表示回数を保持します。
        /// </summary>
        private Dictionary<QjPageNoTypeEnum, int> _pageViewCounts = new Dictionary<QjPageNoTypeEnum, int>();

        #endregion

        #endregion

        #region "Public Property"

        /// <summary>
        /// JavaScript が有効かを取得します。
        /// </summary>
        public bool EnableJavaScript { get => this._enableJavaScript; }

        /// <summary>
        /// クッキーが有効かを取得します。
        /// </summary>
        public bool EnableCookies { get => this._enableCookies; }

        /// <summary>
        /// クッキーが有効かを取得します。
        /// </summary>
        public bool IsDebug { get => this._isDebug; }

        /// <summary>
        /// デバッグ ログのフォルダパスを取得します。
        /// </summary>
        public string DebugLogPath { get => this._debugLogPath; }

        /// <summary>
        /// QolmsApi 用 API 認証 キー を取得します。
        /// </summary>
        public Guid ApiAuthorizeKey { get => this._apiAuthorizeKey; }

        /// <summary>
        /// QolmsApi 用 API 認証有効期限を取得します。
        /// </summary>
        public DateTime ApiAuthorizeExpires { get => this._apiAuthorizeExpires; }

        /// <summary>
        /// QolmsJotoApi 用 API 認証 キー を取得します。
        /// </summary>
        public Guid ApiAuthorizeKey2 { get => this._apiAuthorizeKey2; }

        /// <summary>
        /// QolmsJotoApi 用 API 認証有効期限を取得します。
        /// </summary>
        public DateTime ApiAuthorizeExpires2 { get => this._apiAuthorizeExpires2; }

        /// <summary>
        /// セッション ID を取得します。
        /// </summary>
        public string SessionId { get => this._sessionId; }

        /// <summary>
        /// 所有者アカウント情報を取得します
        /// </summary>
        public AuthorAccountItem AuthorAccount { get => this._authorAccount; }

        /// <summary>
        /// Web API の実行者アカウント キーを取得します。
        /// </summary>
        public Guid ApiExecutor { get => this._authorAccount.AccountKey; }

        /// <summary>
        /// Web API の実行者名を取得します。
        /// </summary>
        public string ApiExecutorName { get => this._authorAccount.Name; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// 値を指定して、クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <param name="authorAccount">所有者 アカウント 情報。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="apiAuthorizeKey">QolmsApi 用 API 認証 キー。</param>
        /// <param name="apiAuthorizeExpires">QolmsApi 用 API 認証期限。</param>
        /// <param name="apiAuthorizeKey2">QolmsJotoApi 用 API 認証 キー。</param>
        /// <param name="apiAuthorizeExpires2">QolmsJotoApi 用 API 認証期限。</param>
        public QolmsJotoModel(AuthorAccountItem authorAccount, string sessionId, Guid apiAuthorizeKey, DateTime apiAuthorizeExpires, Guid apiAuthorizeKey2, DateTime apiAuthorizeExpires2) : base()
        {
            if (authorAccount == null)
            {
                throw new ArgumentNullException("authorAccount", "所有者アカウント情報が Null 参照です。");
            }
            if (authorAccount.AccountKey == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("authorAccount.AccountKey", "アカウント キーが不正です。");
            }
            if (string.IsNullOrWhiteSpace(authorAccount.UserId))
            {
                throw new ArgumentNullException("authorAccount.UserId", "ユーザー ID が Null 参照もしくは空白です。");
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException("sessionId", "セッション ID が Null 参照もしくは空白です。");
            }
            if (apiAuthorizeKey == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("apiAuthorizeKey", "API 認証キーが不正です。");
            }
            if (apiAuthorizeExpires == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException("apiAuthorizeExpires", "API 認証期限が不正です。");
            }

            this._authorAccount = authorAccount;

            this._sessionId = sessionId;

            // QolmsApi 用
            this._apiAuthorizeKey = apiAuthorizeKey;
            this._apiAuthorizeExpires = apiAuthorizeExpires;

            // QolmsJotoApi 用
            this._apiAuthorizeKey2 = apiAuthorizeKey2;
            this._apiAuthorizeExpires2 = apiAuthorizeExpires2;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// JavaScript が有効かを設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableJavaScript(bool enable)
        {
            this._enableJavaScript = enable;
        }

        /// <summary>
        /// クッキーが有効かを設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableCookies(bool enable)
        {
            this._enableCookies = enable;
        }

        /// <summary>
        /// デバッグ ビルドかを設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetIsDebug(bool isDebug)
        {
            this._enableJavaScript = isDebug;
        }

        /// <summary>
        /// デバッグ ログのフォルダパスを設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetDebugLogPath(string debugLogPath)
        {
            this._debugLogPath = debugLogPath;
        }

        /// <summary>
        /// セッション ID を設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetSessionId(string sessionId)
        {
            this._sessionId = sessionId;
        }

        /// <summary>
        /// QolmsApi 用 API 認証 キー を設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetApiAuthorizeKey(Guid key)
        {
            this._apiAuthorizeKey = key;
        }

        /// <summary>
        /// QolmsApi 用 API 認証有効期限を設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetApiAuthorizeExpires(DateTime expires)
        {
            this._apiAuthorizeExpires = expires;
        }

        /// <summary>
        /// QolmsApi 用 API 認証 キー を設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetApiAuthorizeKey2(Guid key)
        {
            this._apiAuthorizeKey2 = key;
        }

        /// <summary>
        /// QolmsApi 用 API 認証有効期限を設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetApiAuthorizeExpires2(DateTime expires)
        {
            this._apiAuthorizeExpires2 = expires;
        }

        /// <summary>
        /// 自動ログインかを設定します。
        /// </summary>
        /// <param name="enable"></param>
        public void SetIsAutoLogin(bool isAutoLogin)
        {
            this._authorAccount.IsAutoLogin = isAutoLogin;
        }

        /// <summary>
        /// ビュー モデル内のプロパティ値をキャッシュへ追加します。
        /// キャッシュ内の既存の値は上書きされます。
        /// </summary>
        /// <typeparam name="TModel">ビュー モデルの型。</typeparam>
        /// <typeparam name="TProperty">プロパティの型。</typeparam>
        /// <param name="model">プロパティ値が設定されたビュー モデルのインスタンス。</param>
        /// <param name="expression">キャッシュへ追加するプロパティを格納しているオブジェクトを識別する式。</param>
        public void SetModelPropertyCache<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> expression) where TModel: QjPageViewModelBase
        {
            var modelName = model.GetType().Name;
            var propertyName = ((PropertyInfo)expression.Body.GetType().GetProperty("Member").GetValue(expression.Body, null)).Name;
            var key =  string.Format("{0}.{1}", modelName, propertyName);
            var value = model.GetType().GetProperty(propertyName).GetValue(model, null);

            this.cacheManager.RemoveCache(QjCacheTypeEnum.ModelProperty, key);
            this.cacheManager.SetCache(QjCacheTypeEnum.ModelProperty, key, value);
        }

        /// <summary>
        /// ビュー モデルのプロパティ値をキャッシュから取得します。
        /// 取得値が null で無ければ、
        /// その値をビュー モデルのプロパティへ設定します。
        /// </summary>
        /// <typeparam name="TModel">ビュー モデルの型。</typeparam>
        /// <typeparam name="TProperty">プロパティの型。</typeparam>
        /// <param name="model">プロパティ値が設定されたビュー モデルのインスタンス。</param>
        /// <param name="expression">キャッシュへ追加するプロパティを格納しているオブジェクトを識別する式。</param>
        public void GetModelPropertyCache<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> expression) where TModel : QjPageViewModelBase
        {
            var modelName = model.GetType().Name;
            var propertyName = ((PropertyInfo)expression.Body.GetType().GetProperty("Member").GetValue(expression.Body, null)).Name;
            var key = string.Format("{0}.{1}", modelName, propertyName);
            var value = model.GetType().GetProperty(propertyName).GetValue(model, null);

            this.cacheManager.GetCache(QjCacheTypeEnum.ModelProperty, key,ref value);

            if (value != null)
            {
                model.GetType().GetProperty(propertyName).SetValue(model, value, null);
            }
        }

        /// <summary>
        /// インプットモデルをキャッシュへ追加します。
        /// キャッシュ内の既存の値は上書きされます。
        /// </summary>
        /// <typeparam name="TModel">ビュー モデルの型。</typeparam>
        /// <param name="model">プロパティ値が設定されたビュー モデルのインスタンス。</param>
        public void SetInputModelCache<TModel>(TModel model) where TModel : QjPageViewModelBase
        { 
            var key = model.GetType().Name;

            this.cacheManager.RemoveCache(QjCacheTypeEnum.InputModel, key);
            this.cacheManager.SetCache(QjCacheTypeEnum.InputModel, key, model);
        }

        ///// <summary>
        ///// インプットモデルをキャッシュへ追加します。
        ///// キャッシュ内の既存の値は上書きされます。
        ///// </summary>
        ///// <typeparam name="TModel">ビュー モデルの型。</typeparam>
        //public void GetInputModelCache<TModel>() where TModel : QjPageViewModelBase
        //{
        //    TModel result = null;
        //    var key = typeof(TModel).Name;

        //    this.cacheManager.GetCache(QjCacheTypeEnum.InputModel, key,ref result);
        //}

        /// <summary>
        /// インプットモデルをキャッシュから取得します。
        /// </summary>
        /// <typeparam name="TModel">ページViewModelの型。</typeparam>
        /// <returns>キャッシュに存在すればモデル、存在しなければ null。</returns>
        public TModel GetInputModelCache<TModel>() where TModel : QjPageViewModelBase
        {
            TModel result = null;
            string key = typeof(TModel).Name;

            // GetCache の第3引数が ref/out どっちでも通るように書き分けたいけど、
            // ここは既存の定義に合わせて "ref" で統一しておく（ZIP側もref想定やったはず）
            this.cacheManager.GetCache(QjCacheTypeEnum.InputModel, key, ref result);

            return result;
        }
        

        /// <summary>
        /// インプット モデルをキャッシュから削除します。
        /// </summary>
        /// <typeparam name="TModel">ビュー モデルの型。</typeparam>
        public void RemoveInputModelCache<TModel>() where TModel : QjPageViewModelBase
        {
            var key = typeof(TModel).Name;

            this.cacheManager.RemoveCache(QjCacheTypeEnum.InputModel, key);
        }

        /// <summary>
        /// セッション内での指定したページの表示回数を +1 します。
        /// </summary>
        /// <param name="pageNo"></param>
        public void IncrementPageViewCount(QjPageNoTypeEnum pageNo)
        {
            if (pageNo != QjPageNoTypeEnum.None)
            {
                if (this._pageViewCounts.ContainsKey(pageNo))
                {
                    this._pageViewCounts[pageNo] += 1;
                }
                else
                {
                    this._pageViewCounts.Add(pageNo,1);
                }
            }
        }

        /// <summary>
        /// セッション内での指定したページの表示回数を取得します。
        /// </summary>
        /// <param name="pageNo">ページ番号</param>
        /// <returns></returns>
        public int GetPageViewCount(QjPageNoTypeEnum pageNo)
        {
            if (pageNo != QjPageNoTypeEnum.None && this._pageViewCounts.ContainsKey(pageNo))
            {
                return this._pageViewCounts[pageNo];
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}