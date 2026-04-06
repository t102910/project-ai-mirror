// mgf.integration.hospitalconnectionrequest 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.hospitalconnectionrequest = mgf.integration.hospitalconnectionrequest || {};

mgf.integration.hospitalconnectionrequest = (function () {
    // 遷移元画面番号を保持している場合のみ、遷移先URLに引き継ぐ。
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    // 画面下部の要約メッセージ領域にエラーを表示する。
    function setSummaryMessage(message) {
        $("#summary-cation").text(message).removeClass("hide");
    }

    // DataContractJsonSerializer は Dictionary を [{Key:"k",Value:"v"}] 配列に変換するため
    // その形式と通常のオブジェクト形式の両方を受け付ける。
    function findInMessages(messages, targetKey) {
        if (Array.isArray(messages)) {
            for (var i = 0; i < messages.length; i++) {
                var item = messages[i];
                if (item && item.Key === targetKey && item.Value) {
                    return item.Value;
                }
            }
            return null;
        }
        return (messages && messages[targetKey]) ? messages[targetKey] : null;
    }

    function getSummaryMessage(messages) {
        var summary = findInMessages(messages, "summary");
        if (summary) {
            return summary;
        }

        // summary がなければ最初の Value を使う
        if (Array.isArray(messages)) {
            for (var i = 0; i < messages.length; i++) {
                if (messages[i] && messages[i].Value) {
                    return messages[i].Value;
                }
            }
        } else if (messages) {
            for (var key in messages) {
                if (Object.prototype.hasOwnProperty.call(messages, key) && messages[key]) {
                    return messages[key];
                }
            }
        }

        return "入力内容を確認してください。";
    }

    // チェックされた開示許可をフラグ値として合算する。
    function buildRelationContentFlags() {
        var checked = 0;
        $("#connection-data :checked").each(function () {
            checked += parseInt($(this).data("content"), 10) || 0;
        });
        return checked;
    }

    // 入力フォームの各値を収集してオブジェクトとして返す。
    function collectInput() {
        var selectedNo = $("#no").val();
        var currentNo = $("#no").data("current-no");
        if ((!selectedNo || selectedNo === "0") && currentNo) {
            selectedNo = String(currentNo);
        }

        return {
            linkageSystemNo: selectedNo,
            linkageSystemId: $("#patient-no").val(),
            familyName: $("#family-name").val(),
            givenName: $("#given-name").val(),
            familyKanaName: $("#family-kana-name").val(),
            givenKanaName: $("#given-kana-name").val(),
            sexType: $("#sex").data("value"),
            birthYear: $("#birthday").data("birthyear"),
            birthMonth: $("#birthday").data("birthmonth"),
            birthDay: $("#birthday").data("birthday"),
            mailAddress: $("#mail").val(),
            relationContentFlags: buildRelationContentFlags(),
            hospitalName: $("#no option:selected").text() || "城東区医師会病院"
        };
    }

    // 申請内容をサーバーへPOSTし、成功時は連携詳細画面へ遷移する。
    function request(identityUpdateFlag) {
        var input = collectInput();
        if (!input.linkageSystemNo || input.linkageSystemNo === "0") {
            setSummaryMessage("病院を選択してください。");
            mgf.unlockScreen();
            return;
        }

        $.ajax({
            type: "POST",
            url: "../Integration/HospitalConnectionRequestResult?ActionSource=Request",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: input.linkageSystemNo,
                LinkageSystemId: input.linkageSystemId,
                FamilyName: input.familyName,
                GivenName: input.givenName,
                FamilyKanaName: input.familyKanaName,
                GivenKanaName: input.givenKanaName,
                SexType: input.sexType,
                BirthYear: input.birthYear,
                BirthMonth: input.birthMonth,
                BirthDay: input.birthDay,
                MailAddress: input.mailAddress,
                IdentityUpdateFlag: identityUpdateFlag ? "True" : "False",
                RelationContentFlags: input.relationContentFlags
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;

            if ($.isTrueString(json.IsSuccess)) {
                var nextLinkageSystemNo = json.LinkageSystemNo || input.linkageSystemNo;
                location.href = "../Integration/HospitalConnection?linkageSystemNo=" + encodeURIComponent(nextLinkageSystemNo)
                    + "&hospitalName=" + encodeURIComponent(input.hospitalName)
                    + fromPageSuffix();
                return;
            }

            var messages = json.Messages || {};
            setSummaryMessage(getSummaryMessage(messages));
            mgf.unlockScreen();
        }).fail(function () {
            setSummaryMessage("登録処理に失敗しました。時間を置いて再度お試しください。");
            mgf.unlockScreen();
        });
    }

    // 申請前の本人確認検証をサーバーへ問い合わせ、結果に応じて確認ダイアログ表示または申請を実行する。
    function identityChangedCheck() {
        var input = collectInput();

        $.ajax({
            type: "POST",
            url: "../Integration/HospitalConnectionRequestResult?ActionSource=IsIdentityChecked",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: input.linkageSystemNo,
                LinkageSystemId: input.linkageSystemId,
                FamilyName: input.familyName,
                GivenName: input.givenName,
                FamilyKanaName: input.familyKanaName,
                GivenKanaName: input.givenKanaName,
                SexType: input.sexType,
                BirthYear: input.birthYear,
                BirthMonth: input.birthMonth,
                BirthDay: input.birthDay,
                MailAddress: input.mailAddress,
                RelationContentFlags: input.relationContentFlags
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;
            var messages = json.Messages || {};

            if (!$.isTrueString(json.IsSuccess)) {
                setSummaryMessage(getSummaryMessage(messages));
                mgf.unlockScreen();
                return;
            }

            if (findInMessages(messages, "summary")) {
                $("#identity-modal").modal("show");
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

    // 申請ボタンクリックで本人確認検証を開始する。
    $("main").on("click", "#request", function () {
        $("#summary-cation").addClass("hide");
        mgf.lockScreen();
        identityChangedCheck();
        return false;
    });
    // 確認ダイアログの「更新」ボタンクリックで本人情報変更ありの申請を実行する。
    $("main").on("click", "#identity-modal .btn-submit", function () {
        $("#identity-modal").modal("hide");
        mgf.lockScreen();
        request(true);
        return false;
    });

    // 確認ダイアログの閉じるボタンクリックでダイアログを閉じる。
    $("main").on("click", "#identity-modal .btn-close, #identity-modal .close", function () {
        $("#identity-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
