using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

/// <summary>
/// 「コルムス ヤプリ サイト」で使用するアクション メソッドを、
/// アクション ソースを元に選択するかを指定する属性を表します。
/// このクラスは継承できません。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal sealed class QjActionMethodSelectorAttribute : ActionMethodSelectorAttribute
{
    #region Public Property

    /// <summary>
    /// アクション ソースを取得または設定します。
    /// </summary>
    public string ActionSources { get; set; } = string.Empty;

    #endregion

    #region Constructor

    /// <summary>
    /// <see cref="QjActionMethodSelectorAttribute" /> クラスの新しいインスタンスを初期化します。
    /// </summary>
    public QjActionMethodSelectorAttribute()
        : base()
    {
    }

    /// <summary>
    /// アクション ソースを指定して、
    /// <see cref="QjActionMethodSelectorAttribute" /> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="actionSources">
    /// アクション ソース（"," 区切りで複数のソースを指定可能）。
    /// </param>
    public QjActionMethodSelectorAttribute(string actionSources)
        : base()
    {
        this.ActionSources = actionSources;
    }

    #endregion

    #region Public Method

    /// <summary>
    /// アクション メソッドの選択が、
    /// 指定されたコントローラー コンテキストで有効かどうかを判断します。
    /// </summary>
    /// <param name="controllerContext">コントローラー コンテキスト。</param>
    /// <param name="methodInfo">アクション メソッドに関する情報。</param>
    /// <returns>
    /// アクション メソッドの選択が、
    /// 指定されたコントローラー コンテキストで有効である場合は true、
    /// それ以外の場合は false。
    /// </returns>
    public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
    {
        const string key = "ActionSource";
        bool result = false;

        var value = controllerContext.Controller.ValueProvider.GetValue(key);
        if (value != null)
        {
            string source = (string)value.ConvertTo(typeof(string));

            result =
                !string.IsNullOrWhiteSpace(this.ActionSources) &&
                this.ActionSources
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(i => string.Compare(i.Trim(), source.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
        }

        return result;
    }

    #endregion
}
