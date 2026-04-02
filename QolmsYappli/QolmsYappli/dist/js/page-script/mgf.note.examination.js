// mgf.note.examination 名前空間オブジェクト
mgf.note.examination = mgf.note.examination || {};

// DOM 構築完了
mgf.note.examination = (function () {
  
    function headerOpen() {

        var resultItems = $('.kenshin-result-table tr.header')

        resultItems.each(function (index, value1) {

            $(value1).children("td").each(function (index2, value2) {

                var cls = $(value2).attr("class");
                var arrCls = cls.split(" ");
                var findCls = arrCls.find(value => value.match(/^td/g));

                if ($(value2).find("span").length == 0) {

                    var name = $(value1).children("th").data("name");
                    var opnflag = false;
                    $("."+name+" ."+findCls).each(function (index3, value3) {
                        if ($.trim( $(value3).find("span").text()).length > 0) {
                            //console.log(value3);
                            opnflag = true;
                        }
                    })

                    if (opnflag) {
                        //console.log($(value1).children("th"));
                        $(".th-wrapper .table-striped .header").children("th").eq(index).addClass('open');

                        var tBody = $(value1).children("th").data('name');
                        $('tr.t-body.' + tBody).addClass('open').parents('.fixed-table').addClass('open');
                    }
                }
            });

        });
    }

    function unit(i) {

        var resultItems = $('.kenshin-result-table tbody .t-body td' + '.td-' + i)

        var hederItems = $(".th-wrapper .table-striped tbody td.unit")

        resultItems.each(function (index, value) {

            if (!$(value).hasClass("header")) {

                var unit = $(value).children(".value-modal").data("unit");
                var standard = $(value).children(".value-modal").data("standard");

                if (!(unit == undefined && standard == undefined) && (unit.length > 0 || standard.length > 0)) {

                    var updateUnit = '<span class="">' + standard

                    if (unit.length > 0 && standard.length > 0) {
                        updateUnit += '<br>';
                    }

                    if (unit.length > 0)
                    {
                        updateUnit += '(' + unit + ')';
                    }

                    updateUnit += '</span>';

                    hederItems.eq(index).html($.parseHTML(updateUnit));
                } else {
                    hederItems.eq(index).html($.parseHTML(""));
                }
            }
        });
    }

    //デザイン用スクリプト
     $(function sliderPreparation() {

        //スライダー操作後のtable操作（スライダー設定より前に記述すること）
        $('.kenshin-result').on('init reInit afterChange', function (event, slick, currentSlide) {
            var i = (currentSlide ? currentSlide : 0) + 1;
            var slidePos = i-1;
            $('.table-responsive').scrollLeft(0).scrollLeft(89 * slidePos);
            $('.kenshin-result-table td').removeClass('on').parents('table').find('.td-' + i).addClass('on');
            //alert(i);
            unit(i);

        });
        //スライダー設定
        var slideCount = $(".kenshin-result .item").length;

        $('.kenshin-result').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            dots: true,
            centerPadding: '0px',
            arrows: true,
            centerMode: true,
            focusOnSelect: true,
            infinite: false,
            initialSlide: slideCount -1
        });

        $('.kenshin-result').attr("style", "");

        $('.kenshin-result-table td').removeClass('on').parents('table').find('.td-' + slideCount).addClass('on');

        unit(slideCount);
        headerOpen();
    });

    //table タップでスライダーを動かす
    $('main').on("click", '.kenshin-result-table td',function () {
        var tablePos = $(this).attr('class').split('-');
        var arrayLast = tablePos.pop();
        var slideNum = parseInt(arrayLast, 10) - 1
        $('.kenshin-result').slick('slickGoTo', slideNum, false);

        unit(slideNum+1);

    });


    //tableのheaderタップで展開
    $('main').on("click", ".header th", function () {
        var tBody = $(this).data('name');
        if ($(this).hasClass('open')) {
            $(this).removeClass('open').parents('.fixed-table').removeClass('open');
            $('tr.t-body.' + tBody).removeClass('open');
        } else {
            $(this).addClass('open');
            $('tr.t-body.' + tBody).addClass('open').parents('.fixed-table').addClass('open');
        }

    });
    //基準値列の展開
    $('main').on("click", '.open-closer',function () {
        if ($(this).hasClass('open')) {
            $(this).removeClass('open');
            $('td.unit').addClass('td-close');
            $('td.unit span').addClass("hide");
        } else {
            $(this).addClass('open');
            $('td.unit').removeClass('td-close');
            $('td.unit span').removeClass("hide");
        }
    });
    $('[data-toggle="tooltip"]').tooltip({ container: 'body' });

    $('[data-toggle="tooltip"]').tooltip();

    $("main").on("click", "#unit-hideout", function () {

            if ($(this).hasClass('on')) {
                $(this).removeClass('on').text('基準値を隠す');
                $('td.unit').show();

            } else {
                $(this).addClass('on').text('基準値を表示する');
                $('td.unit').hide();

            }
    });

    //モーダル
    //総合判定の説明
    $("main").on("click", "#result-description", function () {

        var title = "総合判定";
        var value = "＊詳細の内容については検査データ(検査結果及び所見欄）を御参照ください。 ";

        $("#modal-text h3 span").text(title);
        $("#modal-text div.text-body").text(value);
        $("#modal-text div.text-unit").text("");
        $("#modal-text div.text-standard").text("");
        $("#modal-text").removeClass("hide");

        $('body').addClass('modal-open');
        return false;
    });


    //数値が長すぎた場合のモーダル
    $('main').on("click", "td .modal-text", function () {

        var title = $(this).data("title");
        var value = $(this).text();

        $("#modal-text h3 span").text(title);
        $("#modal-text div.text-body").text(value);
        $("#modal-text div.text-unit").text("");
        $("#modal-text div.text-standard").text("");
        $("#modal-text").removeClass("hide");

        $('body').addClass('modal-open');
        return false;
    });

    //数値をタップ
    $('main').on("click", "td .value-modal", function () {

        var title = $(this).data("title");
        var value = $(this).html();
        //alert(value);
        var unit = $(this).data("unit");
        var standard = $(this).data("standard");

        $("#modal-text h3 span").text(title);
        $("#modal-text div.text-body").html(value);
        $("#modal-text div.text-unit").text("単位 : " + unit);
        $("#modal-text div.text-standard").text("基準値 : " + standard);
        $("#modal-text").removeClass("hide");

        $('body').addClass('modal-open');
        return false;
    });

    //タイトルをタップ
    $('main').on("click", "th.title", function () {

        var title = $(this).children("span").text();
        var value = $(this).data("comment"); 
        var unit = $(this).data("unit");
        var standard = $(this).data("standard");

        $("#modal-text h3 span").text(title);
        $("#modal-text div.text-body").text(value);
        $("#modal-text div.text-unit").text("単位 : " + unit);
        $("#modal-text div.text-standard").text("基準値 : " + standard);
        $("#modal-text").removeClass("hide");
        
        $('body').addClass('modal-open');
        return false;
    });


    //モーダル閉じる
    $('body').on("click", '#modal-close', function () {
        $('body').removeClass('modal-open');
        $("#modal-text").addClass("hide");

    });


    $('body').on("click", "#info-modal .btn-close", function () {

        $("#info-modal").removeClass("transition");
        location.href = "../health/age?fromPageNo=23"

    })

    //ajax
    $("body").on("click", "#calculation", function () {
        mgf.lockScreen();

        var reference = $(this).data("reference");

        $.ajax({
            type: "POST",
            url: "../Note/ExaminationResult?ActionSource=HealthAge",
            traditional: true,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                reference: reference
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                try {

                    if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                        //成功
                        //console.log($.parseJSON(data)["Messages"]);
                        //一件以上成功したら、健康年齢の結果画面へ遷移
                        location.href = "../health/age?fromPageNo=23"

                    } else {
                        //失敗
                        if ($.parseJSON(data)["Messages"].length > 0) {
                            var messages = $.parseJSON(data)["Messages"];
                            var str = ""
                            //console.log(messages);
                            $(messages).each(function (index, element) {
                                str = str + element["Value"];
                            })
                            $("#info-modal .modal-title").text("健康年齢測定");
                            $("#info-modal .modal-body").text(str);
                            $("#info-modal").addClass("transition");

                            $("#info-modal").modal("show");

                        } else {
                            $("#info-modal .modal-title").text("健康年齢測定");
                            $("#info-modal .modal-body").text("測定できる検査がありません。");
                            $("#info-modal").addClass("transition");

                            $("#info-modal").modal("show");
                        }
                    }

                } catch (ex) {

                    $("#info-modal .modal-title").text("エラー");
                    //$("#info-modal .modal-body").text(ex.message);//
                    $("#info-modal .modal-body").text("エラーが発生しました。");

                    $("#info-modal").modal("show");
                }
            } else {
                // エラー
                $("#info-modal .modal-title").text("エラー");
                $("#info-modal .modal-body").text("処理に失敗しました。");

                $("#info-modal").modal("show");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("#info-modal .modal-title").text("エラー");
            $("#info-modal .modal-body").text("サーバーでエラーが発生しました。");

            $("#info-modal").modal("show");

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    });
    
    //ajax
    $("body").on("click", ".associated-file", function () {

        var reference = $(this).data("reference");
        var token = $("input[name='__RequestVerificationToken']").val()
        location.href = "../Note/ExaminationPdf?__RequestVerificationToken=" + token + "&reference=" + reference

        //location.href = "native:/action/open_browser?url=https%3A%2F%2Fjoto-hdrsub.qolms.com%2F%2FNote%2FExaminationPdf%3F__RequestVerificationToken%3D" + token + "%26reference%3D" + reference

        //mgf.lockScreen();
        ////console.log($(this))
        //var reference = $(this).data("reference");
        //console.log(reference);

        //var req = new XMLHttpRequest();
        //req.open("GET", "../Note/ExaminationPdf?__RequestVerificationToken=" + token + "&reference=" + reference, true);
        //req.responseType = "blob";
        
        //var day = new Date()
        //var filename = day.getFullYear()
        //        + ('0' + (day.getMonth() + 1)).slice(-2)
        //        + ('0' + day.getDate()).slice(-2)
        //        + ('0' + day.getHours()).slice(-2)
        //        + ('0' + day.getMinutes()).slice(-2)
        //        + ('0' + day.getSeconds()).slice(-2)
        //        + ('0' + day.getMilliseconds()).slice(-2);

        //console.log(filename);

        //try {
        //    req.onload = function (event) {
        //        var blob = req.response;
        //        console.log(event);
        //        console.log(blob);
        //        console.log(blob.size);
        //        var link = document.createElement('a');
        //        link.href = window.URL.createObjectURL(blob);
        //        link.download = filename + ".pdf";
        //        link.click();
        //        mgf.unlockScreen();
        //    };

        //} catch (e) {
        //    console.log(e.message);

        //}

        //req.send();

        //$.ajax({
        //    type: "GET",
        //    url: "../Note/ExaminationPdf",
        //    traditional: true,
        //    data: {
        //        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        //        reference: reference
        //    },
        //    async: true,
        //    beforeSend: function (jqXHR) {
        //        // セッションのチェック
        //        mgf.note.checkSessionByAjax();
        //    }
        //}).done(function (data, textStatus, jqXHR) {

        //    if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
        //        try {
        //            //成功
        //            var binary = $.parseJSON(data)["PdfBinary"]

        //            //var bin = atob(base64.replace(/^.*,/, ''));
        //            //var buffer = new Uint8Array(bin.length);
        //            //for (var i = 0; i < bin.length; i++) {
        //            //    buffer[i] = bin.charCodeAt(i);
        //            //}

        //            console.log(binary);

        //            var fileName = $.parseJSON(data)["PdfFileName"]

        //            var downloadData = new Blob(binary, { "type": "application/pdf" });
        //            console.log(downloadData);
        //            var downloadUrl = window.URL.createObjectURL(downloadData);

        //            window.open(downloadUrl)

        //            var link = document.createElement('a');
        //            link.href = downloadUrl;
        //            link.download = fileName;
        //            link.click();

        //            (window.URL || window.webkitURL).revokeObjectURL(downloadUrl);

        //        } catch (ex) {
        //            alert("catch:" + ex.message)
        //        }
        //    } else {
        //        // エラー

        //        if (!$.isTrueString($.parseJSON(data)["IsSuccess"])) {

        //            alert("else:" + $.parseJSON(data)["Message"])
        //        } else {
        //            alert("else:" + textStatus)

        //        }
        //    }
        //}).fail(function (jqXHR, textStatus, errorThrown) {
        //    // エラー
        //    alert(textStatus + errorThrown)

        //}).always(function (jqXHR, textStatus) {
        //    // 画面をアンロック
        //    mgf.unlockScreen();
        //});

    });

    $("main").on("change", "#filter input", function () {

        mgf.lockScreen();

        //絞り込み条件

        var narrowInFacility=[];
        $("#facility-filter input:checked").each(function () {

                narrowInFacility.push($(this).val());
            });

        var narrowInGroup = [];
        $('#group-filter input:checked').each(function () {

            narrowInGroup.push($(this).val());
        });

        var narrowInAbnormal = $("#abnormal-only-filter input").prop('checked');

        $.ajax({
            type: "POST",
            url: "../Note/ExaminationResult?ActionSource=Narrow",
            traditional: true,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                narrowInFacility: narrowInFacility,
                narrowInGroup: narrowInGroup,
                narrowInAbnormal:narrowInAbnormal
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                try {

                    $(document).find(".update-area").replaceWith($(data));

                    //デザイン用スクリプト
                    //スライダー操作後のtable操作（スライダー設定より前に記述すること）
                    $('.kenshin-result').on('init reInit afterChange', function (event, slick, currentSlide) {
                        var i = (currentSlide ? currentSlide : 0) + 1;
                        var slidePos = i - 1;
                        $('.table-responsive').scrollLeft(0).scrollLeft(89 * slidePos);
                        $('.kenshin-result-table td').removeClass('on').parents('table').find('.td-' + i).addClass('on');
                        //alert(i);
                    });
               
                    var slideCount = $(".kenshin-result .item").length;

                    $('.kenshin-result').slick({
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        dots: true,
                        centerPadding: '0px',
                        arrows: true,
                        centerMode: true,
                        focusOnSelect: true,
                        infinite: false,
                        initialSlide: slideCount - 1
                    });

                    $('.kenshin-result').attr("style", "");

                    $('.kenshin-result-table td').removeClass('on').parents('table').find('.td-' + slideCount).addClass('on');

                    unit(slideCount);
                    headerOpen();

                } catch (ex) {
                    //alert("catch:" + ex.message)
                }
            } else {
                // エラー

                    //alert("else:" + textStatus)

            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
           // alert(jqXHR + textStatus + errorThrown)

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
    });

    //console.log(document.cookie);
    //document.cookie = "font-size=b max-age=3600";
    //console.log(document.cookie);

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();
})();
