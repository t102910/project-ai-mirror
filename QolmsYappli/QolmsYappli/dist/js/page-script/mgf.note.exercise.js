// mgf.note.exercise 名前空間オブジェクト
mgf.note.exercise = mgf.note.exercise || {};

// DOM 構築完了
mgf.note.exercise = (function () {
    function kindSelectClose() {
        $('#kind-stamp').removeClass('active');
        $('body').removeClass('.modal-open');
    };

    function setNowDate() {
        //現在日時をセット
        var d = new Date();

        // 年月日・時分の取得
        var year = d.getFullYear();
        var month = d.getMonth() + 1;
        var day = d.getDate();

        var dstr = year + '年' + month + '月' + day + '日'

        // 午前を0、午後を1として値を取得します
        var noon = d.getHours() < 12 ? 0 : 1;
        var hour = d.getHours() % 12;
        //var hour2 = d.getHours();
        var minute = d.getMinutes();

        //$('#recordDate').val(dstr)
        //$('#recordDate').datepicker().datepicker('setDate', 'today');
        $('#recordDate').datepicker('setDate', dstr);

        if (noon == 0) {
            $('#meridiem').val('am')
        }
        else {
            $('#meridiem').val('pm')
        }

        $("#hour").val(hour);
        $("#minute").val(minute);
    };

    $('#kind-select').click(function(){
        $('#kind-stamp').addClass('active');
        $('body').addClass('.modal-open');
        return false;
    });
	
    $('#kind-stamp-close').click(function(){
        kindSelectClose();
    });
	
    $('#kind-remover').click(function () {
        $('#kind-select').removeClass('hide');
        $('#changer').addClass('hide');
        $(this).addClass('hide');
        $('#calorie').val('')
        $("div.submit-area a.btn-submit").addClass("disabled")
    });
	
    // スタンプを選択
    $('#kind-stamp li span').click(function () {
	           
        $('#changer').html($(this).clone()).removeClass('hide');
        $('#kind-remover').removeClass('hide');
        $('#kind-select').addClass('hide');
        kindSelectClose();
        $('#calorie').val($(this).attr('data-calorie'))
        $("div.submit-area a.btn-submit").removeClass("disabled")
		
    });

    // セレクトをchange
    $('.stamp-wrap select').on("change", function () {
        var sportsKind = $(this).val();
        var cal = $(this).children("option:selected").data("cal");
        var extype = $(this).children("option:selected").data("extype");

        if (extype > 0) {
            $('#changer').html('<span id="selectedstamp" data-extype="'+extype+'"><img src="/dist/img/tmpl/no-image.png"><span id="stampname">' + sportsKind + '</span></span>').removeClass('hide');
            $('#kind-remover').removeClass('hide');
            $('#kind-select').addClass('hide');
            kindSelectClose();
            $('#calorie').val(cal)
            $("div.submit-area a.btn-submit").removeClass("disabled")
        }
               
    });
            
    // 「日時入力」領域の表示切り替え
    $("main").on("click", "#input-all", function () {
        if ($(this).hasClass('all')) {
            $('body').removeClass('input-all').find('#input-all').removeClass('all').html('<i class="la la-calendar"></i>日時を変更して入力する');
            //現在日時をセットして、入力エリアを閉じる
            var d = new Date();

            // 年月日・時分の取得
            var year = d.getFullYear();
            var month = d.getMonth() + 1;
            var day = d.getDate();

            var dstr = year + '年' + month + '月' + day + '日'

            // 午前を0、午後を1として値を取得します
            var noon = d.getHours() < 12 ? 0 : 1;
            var hour = d.getHours() % 12;
            var minute = d.getMinutes();

            //$('#recordDate').val(dstr)
            //$('#recordDate').datepicker().datepicker('setDate', 'today');
            $('#recordDate').datepicker('setDate', dstr);

            if (noon == 0) {
                $('#meridiem').val('am')
            }
            else {
                $('#meridiem').val('pm')
            }

            $("#hour").val(hour);
            $("#minute").val(minute);

        } else {
            $('body').addClass('input-all').find('#input-all').addClass('all').html('<i class="la la-calendar"></i>現在の日時で入力する');
        }

        return false;
    });

    ////「登録」ボタン
    //$("main").on("click", "div.submit-area a.btn-submit", function () {
            
    //    // 画面をロック
    //    mgf.lockScreen();

    //    // 入力領域が隠れている時＝現在時刻で入力
    //    if ($('#input-all').hasClass('all') == false) {
    //        setNowDate();
    //    }
            
    //    var itemname = "" ;
    //    var exercisetype = null;
    //    var foreignkey = "";

    //    // スタンプが選択されていない時
    //    if ($('#kind-select').hasClass('hide') == false) {
    //        // TODO: マスターから取得したものを設定すべき
    //        itemname = "その他カロリーを消費する行動をした"
    //        exercisetype = "255"
    //        foreignkey = "/dist/img/tmpl/no-image.png"

    //    } else { // スタンプが選択されている時
    //        itemname = $("#stampname").text()
    //        exercisetype = $("#selectedstamp").data("extype")
    //        foreignkey = $("#selectedstamp img").attr("src")
    //    }

    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/ExerciseResult?ActionSource=Edit",
    //        data: {
    //            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //            RecordDate: $("input[name='RecordDate']").val(),
    //            Meridiem: $("select[name='meridiem']").val(),
    //            Hour: $("select[name='hour']").val(),
    //            Minute: $("select[name='minute']").val(),
    //            ItemName: itemname,
    //            ExerciseType: exercisetype,
    //            Calorie: $('#calorie').val(),
    //            ForeignKey: foreignkey
    //        },
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッションのチェック
    //            mgf.note.checkSessionByAjax();
    //        }
    //    }).done(function (data, textStatus, jqXHR) {
    //        try {
    //            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

    //                //登録成功
    //                //画面をロック
    //                mgf.lockScreen();
    //                //パーシャルビューを書き換え
    //                $(document).find("section.data-area").replaceWith($(data))

    //                $('#kind-select').removeClass('hide');
    //                $('#changer').addClass('hide');
    //                $(this).addClass('hide');
    //                $('#calorie').val('')
    //                $("div.submit-area a.btn-submit").addClass("disabled")
    //                $('#kind-remover').addClass('hide');

    //                $('#alertarea').addClass('hide')

    //            } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

    //                // 入力検証エラー
    //                $('#alertarea').removeClass('hide').empty().each(function () {
    //                    var $div = $(this);

    //                    $.each($.parseJSON(data)["Messages"], function () {
    //                        $div.append($("<p>" + this + "</p>"))
    //                    });
    //                });
    //                // 画面をアンロック
    //                //mgf.unlockScreen();
    //            }
    //            else {
    //                // エラー
    //                $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
    //            }
    //        } catch (ex) {
    //            // エラー
    //            $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
    //        }
    //    }).fail(function (jqXHR, textStatus, errorThrown) {
    //        // エラー
    //        $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
    //    }).always(function (jqXHR, textStatus) {
    //        // 画面をアンロック
    //        mgf.unlockScreen();
    //    });

    //    return false;
    //});

    //「登録」ボタン
    $("main").on("click", "div.submit-area a.btn-submit", function () {

        // 画面を ロック
        mgf.lockScreen();

        // 入力領域が隠れている時＝現在時刻で入力
        if ($('#input-all').hasClass('all') == false) {
            setNowDate();
        }

        var itemname = "";
        var exercisetype = null;
        var foreignkey = "";

        if ($('#kind-select').hasClass('hide') == false) {
            // スタンプ が選択されていない時
            // TODO: マスター から取得したものを設定すべき
            itemname = "その他カロリーを消費する行動をした"
            exercisetype = "255"
            foreignkey = "/dist/img/tmpl/no-image.png"
        } else {
            // スタンプ が選択されている時
            itemname = $("#stampname").text()
            exercisetype = $("#selectedstamp").data("extype")
            foreignkey = $("#selectedstamp img").attr("src")
        }

        // セッション チェック 後、登録対象を POST
        mgf.note.checkSessionAsync($(this))
            .then(
                function () {
                    $.ajax({
                        type: "POST",
                        url: "../Note/ExerciseResult?ActionSource=Edit",
                        data: {
                            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                            RecordDate: $("input[name='RecordDate']").val(),
                            Meridiem: $("select[name='meridiem']").val(),
                            Hour: $("select[name='hour']").val(),
                            Minute: $("select[name='minute']").val(),
                            ItemName: itemname,
                            ExerciseType: exercisetype,
                            Calorie: $('#calorie').val(),
                            ForeignKey: foreignkey
                        },
                        async: true
                    }).done(function (data, textStatus, jqXHR) {
                        try {
                            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                                // 成功

                                ////画面を ロック
                                //mgf.lockScreen();

                                // パーシャル ビュー を書き換え
                                $(document).find("section.data-area").replaceWith($(data))

                                // 「スタンプ選択」ダイアログ の非表示
                                $('#kind-select').removeClass('hide');
                                $('#changer').addClass('hide');
                                $(this).addClass('hide');
                                $('#calorie').val('')
                                $("div.submit-area a.btn-submit").addClass("disabled")
                                $('#kind-remover').addClass('hide');

                                $('#alertarea').addClass('hide')
                            } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                                // 入力検証 エラー
                                $('#alertarea').removeClass('hide').empty().each(function () {
                                    var $div = $(this);

                                    $.each($.parseJSON(data)["Messages"], function () {
                                        $div.append($("<p>" + this + "</p>"))
                                    });
                                });
                                // 画面をアンロック
                                //mgf.unlockScreen();
                            }
                            else {
                                // エラー
                                $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
                            }
                        } catch (ex) {
                            // エラー
                            $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        // エラー
                        $("#alertarea").empty().text("登録に失敗しました。").removeClass("hide");
                    }).always(function (jqXHR, textStatus) {
                        // 画面を アンロック
                        mgf.unlockScreen();
                    });
                },
                function () {
                    // セッション 切れなら「ログイン」画面へ遷移
                    mgf.redirectToLoginWithReturnUrl();
                }
            );
    });

    // 削除処理
    $("main").on("click", "a.remove", function () {
            
        $("#delete-modal div.modal-footer button.btn-delete").attr("data-recorddate", $(this).data("recorddate"));
        $("#delete-modal div.modal-footer button.btn-delete").attr("data-exercisetype", $(this).data("exercisetype"));
        $("#delete-modal div.modal-footer button.btn-delete").attr("data-seq", $(this).data("seq"));
        $("#delete-modal div.modal-footer button.btn-delete").attr("data-calorie", $(this).data("calorie"));
        // 削除確認モーダルの表示
        $("#delete-modal").modal("show");

    });

    //// 削除確認モーダルダイアログの「はい」ボタン
    //$("body").on("click", "#delete-modal div.modal-footer button.btn-delete", function () {

    //    //if (!confirm('消去してよろしいですか？')) {
    //    //    /* キャンセルの時の処理 */
    //    //    return false;
    //    //} else {

    //    /*　OKの時の処理 */
    //    // 画面をロック
    //    mgf.lockScreen();
    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/ExerciseResult?ActionSource=Delete",
    //        data: {
    //            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //            RecordDate: $(this).attr("data-recorddate"),                 
    //            ExerciseType: $(this).attr("data-exercisetype"),
    //            Sequence: $(this).attr("data-seq"),
    //            Calorie: $(this).attr("data-calorie")
    //        },
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッションのチェック
    //            mgf.note.checkSessionByAjax();
    //        }
    //    }).done(function (data, textStatus, jqXHR) {
    //        try {
    //            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
    //                //登録（消去）成功
    //                // 削除確認モーダルの非表示
    //                $('body').removeClass('modal-open'); // 1
    //                $('.modal-backdrop').remove();       // 2
    //                $('#delete-modal').modal('hide');        // 3
    //                //画面をロック
    //                mgf.lockScreen();
    //                //パーシャルビューを書き換え
    //                $(document).find("section.data-area").replaceWith($(data))
    //            } else {
    //                // TODO: エラー メッセージ を出す
    //            }
    //        } catch (ex) {
    //            // TODO: エラー メッセージ を出す
    //        }
    //    }).fail(function (jqXHR, textStatus, errorThrown) {
    //        // TODO: エラー メッセージ を出す
    //    }).always(function (jqXHR, textStatus) {
    //        // 画面をアンロック
    //        mgf.unlockScreen();
    //    });
    //    return false;
    //});

    // 「削除確認」ダイアログ の「はい」ボタン
    $("body").on("click", "#delete-modal div.modal-footer button.btn-delete", function () {
        // 画面を ロック
        mgf.lockScreen();

        var recordDate = $(this).attr("data-recorddate");
        var exerciseType = $(this).attr("data-exercisetype");
        var sequence = $(this).attr("data-seq");
        var calorie = $(this).attr("data-calorie");

        // セッション チェック 後、削除対象を POST
        mgf.note.checkSessionAsync($(this))
            .then(
                function () {
                    $.ajax({
                        type: "POST",
                        url: "../Note/ExerciseResult?ActionSource=Delete",
                        data: {
                            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                            RecordDate: recordDate,
                            ExerciseType: exerciseType,
                            Sequence: sequence,
                            Calorie: calorie
                        },
                        async: true
                    }).done(function (data, textStatus, jqXHR) {
                        try {
                            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                                // 成功

                                //// 画面を ロック
                                //mgf.lockScreen();

                                // パーシャル ビュー を書き換え
                                $(document).find("section.data-area").replaceWith($(data));

                                // 「削除確認」ダイアログ の非表示
                                $('body').removeClass('modal-open');
                                $('.modal-backdrop').remove();
                                $('#delete-modal').modal('hide');
                            } else {
                                // TODO: エラー メッセージ を出す
                            }
                        } catch (ex) {
                            // TODO: エラー メッセージ を出す
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        // TODO: エラー メッセージ を出す
                    }).always(function (jqXHR, textStatus) {
                        // 画面を アンロック
                        mgf.unlockScreen();
                    });
                },
                function () {
                    // セッション 切れなら「ログイン」画面へ遷移
                    mgf.redirectToLoginWithReturnUrl();
                }
            );
    });

    // カロリーが入力されたら登録ボタン有効化
    $("main").on("input keyup blur", "#calorie", function (ev) {
            
        if ($.trim($(this).val()) == "") {
            $("div.submit-area a.btn-submit").addClass("disabled")
        }else{
            $("div.submit-area a.btn-submit").removeClass("disabled")
        }

    });

    $("main").on("click", "a.home-btn", function () {

        // 画面をロック
        mgf.note.promiseLockScreen($(this))
         .then(function () {
             location.href = "../Portal/Home"
         });
        return false;

    })

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();
})();
