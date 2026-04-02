Imports System.Web
Imports System.Web.Optimization

Public NotInheritable Class BundleConfig

    Private Class NonOrderingBundleOrderer
        Implements IBundleOrderer

        Public Function OrderFiles(context As BundleContext, files As IEnumerable(Of BundleFile)) As IEnumerable(Of BundleFile) Implements IBundleOrderer.OrderFiles

            Return files

        End Function

    End Class

    ''' <summary>
    ''' スタイルをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleStyles(ByVal bundles As BundleCollection)

        ' Start 系ページ用 CSS
        bundles.Add(
            New StyleBundle("~/dist/css/start") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )

        ' Portal 系ページ用スタイル
        bundles.Add(
            New StyleBundle("~/dist/css/portal") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )

        ' Note 系ページ用スタイル
        bundles.Add(
            New StyleBundle("~/dist/css/note") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )

        ' Health 系ページ用スタイル
        bundles.Add(
            New StyleBundle("~/dist/css/health") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )
        ' Premium 系ページ用スタイル
        bundles.Add(
            New StyleBundle("~/dist/css/premium") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )

        ' passwordreset 系ページ用 CSS
        bundles.Add(
            New StyleBundle("~/dist/css/passwordreset") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        )

        ' Error ページ用スタイル
        bundles.Add(
            New StyleBundle("~/dist/css/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/css/styles.min.css"
                }
            )
        ) 
    
    End Sub

    ''' <summary>
    ''' Google アナリティクス用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfAnalytics(ByVal bundles As BundleCollection)

        '' Google アナリティクス用スクリプト
        'bundles.Add(
        '    New ScriptBundle("~/js/analytics.min") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
        '        {
        '            "~/js/mgf.analytics.js"
        '        }
        '    )
        ')

    End Sub

    ''' <summary>
    ''' Start 系ページ用 Script をオプティマイズするための、
    ''' バンドルオブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドルオブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfStart(ByVal bundles As BundleCollection)

        ' /Start/LoginById ページ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/loginbyid") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.loginbyid.js"
                }
            )
        )

        ' /Start/Register ページ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/register") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.register.js"
                }
            )
        )

        ' /Start/Setup パージ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/setup") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.setup.js"
                }
            )
        )

        ' /Start/Setup パージ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/setup2") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.setup2.js"
                }
            )
        )


        ' /Start/reregister パージ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/reregister") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.reregister.js"
                }
            )
        )

        ' /Start/LoginEdit ページ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/loginedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.start.js",
                    "~/dist/js/page-script/mgf.start.loginedit.js"
                }
            )
        )
        ' /Start/Error ページ用 Script
        bundles.Add(
            New ScriptBundle("~/dist/js/start/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub

    ''' <summary>
    ''' Portal 系ページ用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfPortal(ByVal bundles As BundleCollection)

        ' /Portal/Home ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/home") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.home.js"
                }
            )
        )

        ' /Portal/Search ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/search") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.search.js"
                }
            )
        )

        ' /Portal/SearchDetail ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/searchdetail") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

        ' /Portal/Information ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/information") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.information.js"
                }
            )
        )

        ' /Portal/TargetSetting ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/targetsetting") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.targetsetting.js"
                }
            )
        )

        ' /Portal/TargetSetting2 ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/targetsetting2") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.targetsetting2.js"
                }
            )
        )

        ' /Portal/terms ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/terms") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.terms.js"
                }
            )
        )

        ' /Portal/unsubscribe ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/unsubscribe") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.unsubscribe.js"
                }
            )
        )

        ' /Portal/datacharge ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/datacharge") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.datacharge.js"
                }
            )
        )
        ' /Portal/unsubscribe ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/pointexchange") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.pointexchange.js"
                }
            )
        )

        ' /Portal/History ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/history") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.history.js"
                }
            )
        )

        ' /Portal/tanitaConnection ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/tanitaconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.tanitaconnection.js"
                }
            )
        )
        ' /portal/alkooconnectionページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/alkooconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.alkooconnection.js"
                }
            )
        )

        ' portal/aupointページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/aupoint") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.aupoint.js"
                }
            )
        )

        ' portal/amazongiftcardページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/amazongiftcard") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.amazongiftcard.js"
                }
            )
        )

        ' portal/hospitalconnectionrequestページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/hospitalconnectionrequest") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.hospitalconnectionrequest.js"
                }
            )
        )

        ' portal/hospitalconnection ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/hospitalconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.hospitalconnection.js"
                 }
             )
         )

        ' /portal/challenge ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/challenge") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.challenge.js"
                 }
             )
         )
        ' /portal/challengeentry ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/challengeentry") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.challengeentry.js"
                 }
             )
         )

        ' /portal/challengedetail ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/challengedetail") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.challengedetail.js"
                 }
             )
         )

        ' /portal/challenge ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/challengecolumn") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.challengecolumn.js"
                 }
             )
         )
        ' /portal/challengeedit ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/challengeedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.challengeedit.js"
                 }
             )
         )
        ' /portal/medicineconnection ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/medicineconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.medicineconnection.js"
                 }
             )
         )


        ' /portal/medicineconnectionsearch ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/medicineconnectionsearch") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.medicineconnectionsearch.js"
                 }
             )
         )

        ' /portal/medicineconnectionrequest ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/medicineconnectionrequest") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.medicineconnectionrequest.js"
                 }
             )
         )

        ' /portal/kayoinobaconnection ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/kayoinobaconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.kayoinobaconnection.js"
                 }
             )
         )

        ' /portal/kayoinobaconnectionagree ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/kayoinoba") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.kayoinoba.js"
                 }
             )
         )

        ' /dist/js/portal/kayoinobachecklist ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/kayoinobachecklist") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.kayoinobachecklist.js"
                 }
             )
         )

        ' portal/companyconnectionrequestページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/companyconnectionrequest") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.portal.js",
                    "~/dist/js/page-script/mgf.portal.companyconnectionrequest.js"
                }
            )
        )

        ' portal/companyconnection ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/companyconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.companyconnection.js"
                 }
             )
         )
        ' portal/companyconnectionedit ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/companyconnectionedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.companyconnectionedit.js"
                 }
             )
         )

        ' portal/companyconnectionhome ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/companyconnectionhome") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.companyconnectionhome.js"
                 }
             )
         )


        ' portal/fitbitconnection ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/fitbitconnection") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.fitbitconnection.js"
                 }
             )
         )

        ' portal/movforfemale ページ用スクリプト
        bundles.Add(
         New ScriptBundle( "~/dist/js/portal/movforfemale") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.movforfemale.js"
                 }
             )
         )
        ' portal/userinfomation ページ用スクリプト
        bundles.Add(
         New ScriptBundle( "~/dist/js/portal/userinfomation") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.userinfomation.js"
                 }
             )
         )
        ' portal/userinfomationedit ページ用スクリプト
        bundles.Add(
         New ScriptBundle( "~/dist/js/portal/userinfomationedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.userinfomationedit.js"
                 }
             )
         )
        ' portal/couponforfitbit ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/couponforfitbit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.couponforfitbit.js"
                 }
             )
         )

        ' portal/couponforfitbit ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/smsauthentication") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.smsauthentication.js"
                 }
             )
         )
        ' portal/localidverification ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/localidverification") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.localidverification.js"
                 }
             )
         )
        ' portal/localidverificationagreement ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/localidverificationagreement") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.localidverificationagreement.js"
                 }
             )
         )
        ' portal/localidverificationdetail ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/localidverificationdetail") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.localidverificationdetail.js"
                 }
             )
         )
        ' portal/localidverificationregister ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/localidverificationregister") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.localidverificationregister.js"
                 }
             )
         )
        ' portal/localidverificationrequest ページ用スクリプト
        bundles.Add(
         New ScriptBundle("~/dist/js/portal/localidverificationrequest") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                 {
                     "~/dist/js/bootstrap-3.3.7.min.js",
                     "~/dist/js/spc.lib.min.js",
                     "~/dist/js/spc.util.min.js",
                     "~/dist/js/page-script/mgf.js",
                     "~/dist/js/page-script/mgf.portal.js",
                     "~/dist/js/page-script/mgf.portal.localidverificationrequest.js"
                 }
             )
         )

        ' /Portal/Error ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/portal/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub

    ''' <summary>
    ''' Note 系ページ用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfNote(ByVal bundles As BundleCollection)

        ' /Note/Walk ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/walk") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/highcharts-draw.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.walk.js"
                }
            )
        )

        ' /Note/Exercise ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/exercise") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.exercise.js"
                }
            )
        )

        '' /Note/Meal ページ用スクリプト（廃止）
        'bundles.Add(
        '    New ScriptBundle("~/dist/js/note/meal") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
        '        {
        '            "~/dist/js/bootstrap-3.3.7.min.js",
        '            "~/dist/js/bootstrap-datepicker.js",
        '            "~/dist/js/exif.min.js",
        '            "~/dist/js/spc.lib.min.js",
        '            "~/dist/js/spc.util.min.js",
        '            "~/dist/js/slick.min.js",
        '            "~/dist/js/page-script/mgf.js",
        '            "~/dist/js/page-script/mgf.note.js",
        '            "~/dist/js/page-script/mgf.note.meal.js"
        '        }
        '    )
        ')

        '' /Note/Meal2 ページ用スクリプト（廃止）
        'bundles.Add(
        '    New ScriptBundle("~/dist/js/note/meal2") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
        '        {
        '            "~/dist/js/bootstrap-3.3.7.min.js",
        '            "~/dist/js/bootstrap-datepicker.js",
        '            "~/dist/js/exif.min.js",
        '            "~/dist/js/spc.lib.min.js",
        '            "~/dist/js/spc.util.min.js",
        '            "~/dist/js/slick.min.js",
        '            "~/dist/js/page-script/mgf.js",
        '            "~/dist/js/page-script/mgf.note.js",
        '            "~/dist/js/page-script/mgf.note.meal2.js"
        '        }
        '    )
        ')

        ' /Note/Meal3 ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/meal3") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/slick.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.meal3.js"
                }
            )
        )


        ' /Note/Meal4 ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/meal4") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.meal4.js"
                }
            )
        )

        ' /Note/MealSearchResult ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mealsearchresult") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mealsearchresult.js"
                }
            )
        )

        ' /Note/Meal/MealRegisterFromHistory ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mealregisterfromhistory") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mealregisterfromhistory.js"
                }
            )
        )

        ' /Note/Meal/MealRegisterFromPhoto ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mealregisterfromphoto") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mealregisterfromphoto.js"
                }
            )
        )


        ' /Note/Meal/MealRegisterFromSearch ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mealregisterfromsearch") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mealregisterfromsearch.js"
                }
            )
        )

        ' /Note/Meal/MealEdit ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mealedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/exif.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mealedit.js"
                }
            )
        )


        ' /Note/Vital ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/vital") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/jquery-qrcode-0.18.0.min.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/highcharts-draw.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.vital.js"
                }
            )
        )

        ' /Note/gulfsportsmovie ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/gulfsportsmovieindex") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.gulfsportsmovieindex.js"
                }
            )
        )

        ' /Note/gulfsportsmovie ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/gulfsportsmovie") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.gulfsportsmovie.js"
                }
            )
        )

        ' /Note/examination ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/examination") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.examination.js"
                }
            )
        )

        ' /Note/recipemovieindex ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/recipemovieindex") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.recipemovieindex.js"
                }
            )
        )

        ' /Note/recipemovie ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/recipemovie") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.recipemovie.js"
                }
            )
        )

        ' /Note/Heartrate ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/heartrate") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/highcharts-draw.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.heartrate.js"
                }
            )
        )

        ' /Note/Mets ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/mets") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/highcharts-draw.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.note.js",
                    "~/dist/js/page-script/mgf.note.mets.js"
                }
            )
        )

        ' /Note/Error ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/note/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub

    ''' <summary>
    ''' Health 系ページ用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfHealth(ByVal bundles As BundleCollection)

        ' /Health/Age ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/health/age") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/health-age.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.health.js",
                    "~/dist/js/page-script/mgf.health.age.js"
                }
            )
        )

        ' /Health/AgeEdit ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/health/ageedit") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.health.js",
                    "~/dist/js/page-script/mgf.health.ageedit.js"
                }
            )
        )

        ' /Health/Consult ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/health/consult") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

        ' /Health/Error ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/health/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub

    ''' <summary>
    ''' Premium 系ページ用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfPremium(ByVal bundles As BundleCollection)

        ' /premium/index ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/index") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/highcharts.js",
                    "~/dist/js/highcharts-more.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/health-age.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.index.js"
                }
            )
        )

        ' /premium/regist ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/regist") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.regist.js"
                }
            )
        )

        ' /premium/payjpcardregister ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/payjpcardregister") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.payjpcardregister.js"
                }
            )
        )
        '"~/dist/js/bootstrap-3.3.7.min.js",
        '"~/dist/js/bootstrap-datepicker.js",
        '"~/dist/js/spc.lib.min.js",
        '"~/dist/js/spc.util.min.js",
        '"~/dist/js/page-script/mgf.premium.js",

        ' /premium/payjpcardregister ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/payjpcardupdate") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.payjpcardupdate.js"
                }
            )
        )

        ' /premium/payjpcardregister ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/methodchange") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/bootstrap-datepicker.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.methodchange.js"
                }
            )
        )
        ' /Premium/History ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/history") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.history.js"
                }
            )
        )

        ' /Premium/PayjpAgree ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/payjpagree") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.premium.js",
                    "~/dist/js/page-script/mgf.premium.payjpagree.js"
                }
            )
        )

        ' /Premium/Error ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/premium/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub
    
    ''' <summary>
    ''' Premium 系ページ用スクリプトをオプティマイズするための、
    ''' バンドル オブジェクトのセットを設定します。
    ''' </summary>
    ''' <param name="bundles">バンドル オブジェクトのセットのコレクション。</param>
    ''' <remarks></remarks>
    Private Shared Sub BundleScriptsOfPasswordReset(ByVal bundles As BundleCollection)

        ' /passwordreset/recover ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/passwordreset/recover") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/passwordreset.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.passwordreset.js",
                    "~/dist/js/page-script/mgf.passwordreset.recover.js"
                }
            )
        )

        ' /passwordreset/recoveryidentifier ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/passwordreset/recoveryidentifier") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/passwordreset.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.passwordreset.js",
                    "~/dist/js/page-script/mgf.passwordreset.recoveryidentifier.js"
                }
            )
        )

        ' /passwordreset/recoversms ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/passwordreset/recoversms") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js",
                    "~/dist/js/page-script/passwordreset.js",
                    "~/dist/js/page-script/mgf.js",
                    "~/dist/js/page-script/mgf.passwordreset.js",
                    "~/dist/js/page-script/mgf.passwordreset.recoversms.js"
                }
            )
        )


        ' /passwordreset/Error ページ用スクリプト
        bundles.Add(
            New ScriptBundle("~/dist/js/passwordreset/error") With {.Orderer = New NonOrderingBundleOrderer()}.Include(
                {
                    "~/dist/js/bootstrap-3.3.7.min.js",
                    "~/dist/js/spc.lib.min.js",
                    "~/dist/js/spc.util.min.js"
                }
            )
        )

    End Sub
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)

        ' スタイルのバンドル
        BundleConfig.BundleStyles(bundles)

        '' Google アナリティクス用スクリプトのバンドル
        'BundleConfig.BundleScriptsOfAnalytics(bundles)

        ' Start 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfStart(bundles)

        ' Portal 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfPortal(bundles)

        ' Note 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfNote(bundles)

        ' Health 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfHealth(bundles)

        ' Premium 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfPremium(bundles)
        
        ' Premium 系ページ用スクリプトのバンドル
        BundleConfig.BundleScriptsOfPasswordReset(bundles)

        ' バンドルとミニフィケーションを有効にする
        ' 個別にスタイルやスクリプトの検証が必要な場合は下記を False に設定する
        BundleTable.EnableOptimizations = True

    End Sub

End Class
