using System.Collections.Generic;
using System.Web.Optimization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// スタイル および スクリプト ファイルの バンドル と ミニフィケーション に関する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// バンドル 内の ファイル に対する、
        /// 既定の順序付けを無視する機能を提供します。
        /// </summary>
        private class NonOrderingBundleOrderer : IBundleOrderer
        {
            /// <summary>
            /// バンドル 内の ファイル を並べ替えます。
            /// </summary>
            /// <param name="context">バンドル リクエスト の処理に必要な情報。</param>
            /// <param name="files">バンドル に含まれる ファイル の コレクション。</param>
            /// <returns>
            /// 並べ替えられた ファイル の コレクション。
            /// </returns>
            IEnumerable<BundleFile> IBundleOrderer.OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
                => files ?? new List<BundleFile>();
        }

        /// <summary>
        /// JS の変換（ミニファイ等）を一切行わないための Transform。
        /// </summary>
        private sealed class NoJsTransform : IBundleTransform
        {
            public void Process(BundleContext context, BundleResponse response)
            {
                // 何もしない（= 変換なし）
                // ContentType だけ JS として明示
                response.ContentType = "text/javascript";
            }
        }

        /// <summary>
        /// スタイル を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleStyles(BundleCollection bundles)
        {
            // Start 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/start")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );

            // point 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/point")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );

            // integration 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/integration")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );
            // portal 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/portal")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );

            // note 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/note")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );
            // health 系ページ用 CSS
            bundles.Add(
                new StyleBundle("~/dist/css/health")
                {
                    Orderer = new NonOrderingBundleOrderer()
                }.Include(
                    new string[]
                    {
                        "~/dist/css/bootstrap.min.css",
                        "~/dist/css/slick-theme.css",
                        "~/dist/css/slick.css",
                        "~/dist/css/styles.min.css"
                    }
                )
            );
        }

        /// <summary>
        /// Start 系画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// ※ JsMinify を一切通さない（結合のみ）
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfStart(BundleCollection bundles)
        {
            // /Start/LoginById ページ用 Script（結合のみ）
            var bundle = new Bundle("~/dist/js/start/loginbyid", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };

            bundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.loginbyid.js"
                }
            );

            bundles.Add(bundle);

            // 必要分を追加
        }

        /// <summary>
        /// Point 系画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// ※ JsMinify を一切通さない（結合のみ）
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfPoint(BundleCollection bundles)
        {

            // /point/history ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/point/history", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.point.js",
                    "~/dist/js/page-script/mgf.point.history.js"
                    )
                );

            // point / exchange ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/point/exchange", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.point.js",
                    "~/dist/js/page-script/mgf.point.exchange.js"
                    )
                );

            // /point/amazongiftcard ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/point/amazongiftcard", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.point.js",
                    "~/dist/js/page-script/mgf.point.amazongiftcard.js"
                    )
                );

            // /point/localhistory ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/point/localhistory", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.point.js",
                    "~/dist/js/page-script/mgf.point.localhistory.js"
                    )
                );
        }

        /// <summary>
        /// Note 画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// ※ JsMinify を一切通さない（結合のみ）
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfNote(BundleCollection bundles)
        {
            var bundle = new Bundle("~/dist/js/note/examination", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };

            bundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.examination.js"
                }
            );

            bundles.Add(bundle);

            // ガルフスポーツ動画 TOP
            var gulfIndexBundle = new Bundle("~/dist/js/note/gulfsportsmovieindex", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };
            gulfIndexBundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.gulfsportsmovieindex.js"
                }
            );
            bundles.Add(gulfIndexBundle);

            // ガルフスポーツ動画 詳細
            var gulfBundle = new Bundle("~/dist/js/note/gulfsportsmovie", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };
            gulfBundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.gulfsportsmovie.js"
                }
            );
            bundles.Add(gulfBundle);
        }

        /// <summary>
        /// Health 画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// ※ JsMinify を一切通さない（結合のみ）
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfHealth(BundleCollection bundles)
        {
            var bundle = new Bundle("~/dist/js/health/age", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };

            bundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.health.age.js"
                }
            );

            bundles.Add(bundle);
        }


        /// <summary>
        /// Portal 画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfPortal(BundleCollection bundles)
        {
            var bundle = new Bundle("~/dist/js/portal/LocalId", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };

            bundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.health.age.js"
                }
            );

            bundles.Add(bundle);

            var connectionSettingBundle = new Bundle("~/dist/js/portal/ConnectionSetting", new NoJsTransform())
            {
                Orderer = new NonOrderingBundleOrderer()
            };

            connectionSettingBundle.Include(
                new string[]
                {
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js"
                }
            );

            bundles.Add(connectionSettingBundle);
        }

        /// <summary>
        /// Portal 画面用 スクリプト を オプティマイズ するための、
        /// バンドル オブジェクト の セット を設定します。
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        private static void BundleScriptsOfIntegration(BundleCollection bundles)
        {
            // integration 共通ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.health.age.js"
                )
            );

            // /integration/companyconnection ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/companyconnection", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.companyconnection.js"
                )
            );

            // /integration/companyconnectionedit ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/companyconnectionedit", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.companyconnectionedit.js"
                )
            );

            // /integration/companyconnectionhome ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/companyconnectionhome", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.companyconnectionhome.js"
                )
            );

            // /integration/companyconnectionrequest ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/companyconnectionrequest", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.companyconnectionrequest.js"
                )
            );

            // /integration/hospitalconnection ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/hospitalconnection", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.hospitalconnection.js"
                )
            );

            // /integration/hospitalconnectionrequest ページ用 Script（結合のみ）
            bundles.Add(
                new Bundle("~/dist/js/integration/hospitalconnectionrequest", new NoJsTransform()) { Orderer = new NonOrderingBundleOrderer() }.Include(
                    "~/dist/js/jquery-3.7.1.min.js",
                    "~/dist/js/bootstrap.bundle.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.integration.hospitalconnectionrequest.js"
                )
            );
        }

        /// <summary>
        /// スタイル および スクリプト ファイル の バンドル と ミニフィケーション を登録します。
        /// </summary>
        /// <param name="bundles">バンドル オブジェクト の セット の コレクション。</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // スタイル の バンドル
            BundleConfig.BundleStyles(bundles);

            // Start 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfStart(bundles);

            // Point 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfPoint(bundles);

            // Note 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfNote(bundles);

            // Health 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfHealth(bundles);

            // Portal 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfPortal(bundles);

            // Integration 系ページ用スクリプトのバンドル
            BundleConfig.BundleScriptsOfIntegration(bundles);

            // バンドル と ミニフィケーション を有効にする
            // JsMinify は使わないが、Bundle 化（結合）は行うので true のままでOK
            BundleTable.EnableOptimizations = true;

        }
    }
}
