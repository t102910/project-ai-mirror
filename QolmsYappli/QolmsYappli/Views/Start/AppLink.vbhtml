@Code
    ViewData("Title") = "AppLink"
    Layout = "~/Views/Shared/_PasswordResetLayout.vbhtml"
End Code

<a href="@ViewData("AppLink")">アプリへ </a>
@ViewData("AppLink")
5秒後に遷移します
@QyHtmlHelper.RenderScriptTag("~/dist/js/passwordreset/recoveryidentifier")


<script>

    var link = $("a").attr("href");
    console.log(link);
    var timer = setTimeout(function () {
        location.href = link
    }, 5000);

</script>