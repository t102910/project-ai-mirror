// 登録ボタン
$(document).on("click", "div.submit-area button[type='submit']", function () {
    if ($("form")[0].checkValidity()) {
        // 画面をロック後、サブミット
        mgf.start.promiseLockScreen($(this));
    }

    return true;
});


// DOM構築完了
$(function () {
    // ヒストリーバックの禁止
    mgf.prohibitHistoryBack();
});

////　ウィンドウロード時
//$(window).load(function () {
//    //alert("onload");
//    // 「登録完了」ダイアログの表示
//    if ($("#ThanksMessage")[0]) $("#ThanksMessage").modal();
//});

//// 「登録完了」ダイアログの「閉じる」ボタン
//$(document).on("click", "#ThanksMessage button", function () {
//    // 画面をロック後、サブミット
//    promiseLockScreen($(this))
//    .then(function () {

//        // ID,Passwordを指定して、ログイン（Home画面表示）
//        $("<form>", {
//            action: "../Start/LoginResult",
//            method: "POST"
//        }).appendTo(document.body);

//        $("<input>").attr({
//            type: "hidden",
//            name: "__RequestVerificationToken",
//            value: $("input[name='__RequestVerificationToken']").val()
//        }).appendTo($("form:last"));

//        // ID
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.UserId",
//            value: $("input[name='model.UserId']").val()
//        }).appendTo($("form:last"));

//        // Pass
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.Password",
//            value: $("input[name='model.Password']").val()
//        }).appendTo($("form:last"));

//        // RememverId
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.RememverId",
//            value: "False"
//        }).appendTo($("form:last"));

//        // RememberLogin
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.RememberLogin",
//            value: "False"
//        }).appendTo($("form:last"));

//        // LoginResultType
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.LoginResultType",
//            value: "None"
//        }).appendTo($("form:last"));

//        // Message
//        $("<input>").attr({
//            type: "hidden",
//            name: "model.Message",
//            value: ""
//        }).appendTo($("form:last"));

//        $("form:last").submit();

//    });

//    //// ログイン画面へ遷移する
//    //location.href = "../Start/LoginById";

//    return true;
//});