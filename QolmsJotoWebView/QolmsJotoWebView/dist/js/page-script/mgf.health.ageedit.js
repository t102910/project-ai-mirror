// mgf.health.ageedit 名前空間オブジェクト
mgf.health.ageedit = mgf.health.ageedit || {};

// DOM 構築完了
mgf.health.ageedit = (function () {
    // 「登録」ボタン
    $("main").on("click", "section.contents-area div.submit-area a.btn-submit", function () {
        // 画面をロック後、サブミット
        mgf.health.promiseLockScreen($(this))
            .then(function () {
                //
                try {
                    $("<form>", {
                        action: "../Health/AgeEditResult?ActionSource=Edit",
                        method: "POST"
                    }).appendTo(document.body);

                    $("<input>").attr({
                        type: "hidden",
                        name: "__RequestVerificationToken",
                        value: $("input[name='__RequestVerificationToken']").val()
                    }).appendTo($("form:last"));

                    // 健診受診日
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.FromPageNo",
                        value: $("#page-no").data("pageno")
                    }).appendTo($("form:last"));


                    // 健診受診日
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.RecordDate",
                        value: $("section.contents-area input[name='RecordDate']").val()
                    }).appendTo($("form:last"));

                    // BMI
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.BMI",
                        value: $("section.contents-area input[name='BMI']").val()
                    }).appendTo($("form:last"));

                    // 血圧（上）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch014",
                        value: $("section.contents-area input[name='Ch014']").val()
                    }).appendTo($("form:last"));

                    // 血圧（下）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch016",
                        value: $("section.contents-area input[name='Ch016']").val()
                    }).appendTo($("form:last"));

                    // 中性脂肪
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch019",
                        value: $("section.contents-area input[name='Ch019']").val()
                    }).appendTo($("form:last"));

                    // HD Lコレステロール
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch021",
                        value: $("section.contents-area input[name='Ch021']").val()
                    }).appendTo($("form:last"));

                    // LDL コレステロール
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch023",
                        value: $("section.contents-area input[name='Ch023']").val()
                    }).appendTo($("form:last"));

                    // GOT（AST）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch025",
                        value: $("section.contents-area input[name='Ch025']").val()
                    }).appendTo($("form:last"));

                    // GPT（ALT）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch027",
                        value: $("section.contents-area input[name='Ch027']").val()
                    }).appendTo($("form:last"));

                    // γ-GT（γ-GTP）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch029",
                        value: $("section.contents-area input[name='Ch029']").val()
                    }).appendTo($("form:last"));

                    // HbA1c（NGSP）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch035",
                        value: $("section.contents-area input[name='Ch035']").val()
                    }).appendTo($("form:last"));

                    // 空腹時血糖
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch035FBG",
                        value: $("section.contents-area input[name='Ch035FBG']").val()
                    }).appendTo($("form:last"));

                    // 尿糖
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch037",
                        value: $("section.contents-area select[name='Ch037']").val()
                    }).appendTo($("form:last"));

                    // 尿蛋白（定性）
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Ch039",
                        value: $("section.contents-area select[name='Ch039']").val()
                    }).appendTo($("form:last"));

                    // サブミット
                    $("form:last").submit();
                } catch (ex) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            });

        return false;
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();
})();
