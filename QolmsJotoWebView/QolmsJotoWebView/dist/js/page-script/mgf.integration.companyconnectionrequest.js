// mgf.integration.companyconnectionrequest 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionrequest = mgf.integration.companyconnectionrequest || {};

mgf.integration.companyconnectionrequest = (function () {
    // 遷移元画面番号を保持している場合のみ、遷移先URLに引き継ぐ。
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    // チェックされた開示許可をフラグ値として合算する。
    function buildRelationContentFlags() {
        var checked = 0;
        $("#connection-data :checked").each(function () {
            checked += parseInt($(this).data("content"), 10) || 0;
        });
        return checked;
    }

    // 画面下部の要約メッセージ領域にエラーを表示する。
    function setSummaryMessage(message) {
        $("#summary-cation").text(message).removeClass("hide");
    }

    // DataContractJsonSerializer 由来の Key/Value 配列と連想配列の両方から summary を取得する。
    function getSummaryMessage(messages) {
        var summary = "";

        $.each(messages || [], function (key, value) {
            if (value && value.Key === "summary") {
                summary = value.Value || "";
                return false;
            }

            if (key === "summary") {
                summary = value || "";
                return false;
            }

            return true;
        });

        return summary;
    }

    function request(identityUpdateFlag) {
        var linkageSystemName = $("#request").data("linkagesystemname") || "企業";

        // 申請内容をサーバーへPOSTし、成功時は連携詳細へ遷移する。
        $.ajax({
            type: "POST",
            url: "../Integration/CompanyConnectionRequestResult?ActionSource=Request",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FacilityId: $("#code").val(),
                EmployeeNo: $("#id").val(),
                FamilyName: $("#family-name").val(),
                GivenName: $("#given-name").val(),
                FamilyKanaName: $("#family-kana-name").val(),
                GivenKanaName: $("#given-kana-name").val(),
                SexType: $("#sex").data("value"),
                BirthYear: $("#birthday").data("birthyear"),
                BirthMonth: $("#birthday").data("birthmonth"),
                BirthDay: $("#birthday").data("birthday"),
                IdentityUpdateFlag: identityUpdateFlag ? "True" : "False",
                RelationContentFlags: buildRelationContentFlags()
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;

            if ($.isTrueString(json.IsSuccess)) {
                var url = "../Integration/CompanyConnection?linkageSystemNo="
                    + encodeURIComponent(json.LinkageSystemNo)
                    + "&linkageSystemName=" + encodeURIComponent(linkageSystemName)
                    + fromPageSuffix();
                location.href = url;
                return;
            }

            var messages = json.Messages || [];
            var summaryMessage = getSummaryMessage(messages);
            setSummaryMessage(summaryMessage || "入力内容を確認してください。");
            mgf.unlockScreen();
        }).fail(function () {
            // 通信失敗時は要約エラーのみ表示する。
            setSummaryMessage("登録処理に失敗しました。時間を置いて再度お試しください。");
            mgf.unlockScreen();
        });

        return false;
    }
    // フィールドエラーをsummaryキーのメッセージとして画面に表示する。
    function showFieldErrors(messages) {
        var summaryMessage = getSummaryMessage(messages);

        if (summaryMessage) {
            setSummaryMessage(summaryMessage);
        } else {
            setSummaryMessage("入力内容を確認してください。");
        }
    }
    // 申請前の本人確認検証をサーバーへ問い合わせ、結果に応じて確認ダイアログ表示または申請を実行する。
    function identityChangedCheck() {
        $.ajax({
            type: "POST",
            url: "../Integration/CompanyConnectionRequestResult?ActionSource=IsIdentityChecked",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FacilityId: $("#code").val(),
                EmployeeNo: $("#id").val(),
                FamilyName: $("#family-name").val(),
                GivenName: $("#given-name").val(),
                FamilyKanaName: $("#family-kana-name").val(),
                GivenKanaName: $("#given-kana-name").val(),
                SexType: $("#sex").data("value"),
                BirthYear: $("#birthday").data("birthyear"),
                BirthMonth: $("#birthday").data("birthmonth"),
                BirthDay: $("#birthday").data("birthday"),
                RelationContentFlags: buildRelationContentFlags()
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;
            var messages = json.Messages || [];
            var summaryMessage = getSummaryMessage(messages);

            if (!$.isTrueString(json.IsSuccess)) {
                showFieldErrors(messages);
                mgf.unlockScreen();
                return;
            }

            if (summaryMessage) {
                $("#identity-modal").modal("show");
                mgf.unlockScreen();
                return;
            }

            if ($("#disconnect-modal").length > 0) {
                $("#disconnect-modal").modal("show");
                mgf.unlockScreen();
                return;
            }

            request(false);
            mgf.unlockScreen();
        }).fail(function () {
            setSummaryMessage("登録処理に失敗しました。時間を置いて再度お試しください。");
            mgf.unlockScreen();
        });
    }

    $("main").on("click", "#request", function () {
        $("#summary-cation").addClass("hide");
        mgf.lockScreen();
        identityChangedCheck();
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-delete", function () {
        $("#disconnect-modal").modal("hide");
        mgf.lockScreen();
        request(false);
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-close, #disconnect-modal .close", function () {
        $("#disconnect-modal").modal("hide");
        return false;
    });

    $("main").on("click", "#identity-modal .btn-submit", function () {
        $("#identity-modal").modal("hide");

        if ($("#disconnect-modal").length > 0) {
            $("#disconnect-modal").modal("show");
            return false;
        }

        mgf.lockScreen();
        request(true);
        return false;
    });

    $("main").on("click", "#identity-modal .btn-close, #identity-modal .close", function () {
        $("#identity-modal").modal("hide");
        return false;
    });
    // 企業コードが無料企業コードの場合はバイタル共有項目を非表示にする。    // 企業コードが無料企業コードの場合はバイタル共有項目を非表示にする。
    function toggleVitalShareByFacilityCode() {
        var isFreeBusinessCode = $.trim($("#code").val()) === "47500107";
        var $vital = $("#VitalNotebook");
        var $label = $("label[for='VitalNotebook']");

        if (isFreeBusinessCode) {
            $vital.prop("checked", false).addClass("hide");
            $label.addClass("hide");
            return;
        }

        $vital.removeClass("hide");
        $label.removeClass("hide");
    }

    $("#code").on("blur change", function () {
        toggleVitalShareByFacilityCode();
    });

    toggleVitalShareByFacilityCode();

    mgf.prohibitHistoryBack();

    return {};
})();
