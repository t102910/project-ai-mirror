// mgf.point.localhistory 名前空間オブジェクト
mgf.point.localhistory = mgf.point.localhistory || {};

// DOM 構築完了
mgf.point.localhistory = (function () {

	// JSON文字列/オブジェクトどちらでも扱えるように正規化する
	function parseJsonResult(data) {
		if (!data) {
			return null;
		}

		if (typeof data === "string") {
			return $.parseJSON(data);
		}

		return data;
	}

	function getCurrentPointFromView() {
		// 画面表示テキスト(例: 1,234pt)から数値のみを取り出す
		var pointText = $("#point-field").text() || "0";
		var normalized = pointText.replace(/[^0-9]/g, "");
		var point = parseInt(normalized, 10);

		if (!Number.isFinite(point) || point < 0) {
			point = 0;
		}

		return point;
	}

	function formatPoint(value) {
		// 数値を日本語ロケールの3桁区切りで整形
		var number = Number(value);

		if (!Number.isFinite(number)) {
			return "0";
		}

		return number.toLocaleString("ja-JP");
	}

	function hideRedeemError() {
		// 変換エラー表示を初期化
		$("#localPointRedeemMessageHeading").text("入力エラー");
		$("#localPointRedeemErrorText").text("");
		$("#localPointRedeemErrorMessage")
			.removeClass("default caution alert alert-danger alert-info")
			.addClass("hide");
	}

	function showRedeemError(message) {
		// 変換エラー文言を表示
		$("#localPointRedeemMessageHeading").text("入力エラー");
		$("#localPointRedeemErrorText").text(message || "ポイント変換に失敗しました。");
		$("#localPointRedeemErrorMessage")
			.removeClass("default caution alert-info hide")
			.addClass("alert alert-danger");
	}

	function showRedeemSuccess(message) {
		// 成功文言は通常スタイル（赤表示ではない）で表示
		$("#localPointRedeemMessageHeading").text("変換完了");
		$("#localPointRedeemErrorText").text(message || "ポイント変換が完了しました。");
		$("#localPointRedeemErrorMessage")
			.removeClass("default caution alert-danger hide")
			.addClass("alert alert-info");
	}

	function setCurrentPoint(point) {
		// 合計ポイント表示と変換ダイアログ入力上限を同期
		var currentPoint = Number(point);

		if (!Number.isFinite(currentPoint) || currentPoint < 0) {
			currentPoint = 0;
		}

		$("#point-field").html(formatPoint(currentPoint) + "<i>pt</i>");
		$("#localPointMax").text(formatPoint(currentPoint));
		$("#localPointRedeemInput").val(currentPoint);
		$("#localPointRedeemInput").attr("placeholder", "1～" + currentPoint);
	}

	function initializeRedeemDialog() {
		// ダイアログ表示時にエラーを消し、最新のポイント値を反映
		hideRedeemError();
		setCurrentPoint(getCurrentPointFromView());
	}

	// 年月指定で履歴パーシャルを再取得し、表示領域を差し替える
	function loadLocalHistory() {

		// 前回エラー表示をクリア
		$("#yearMonthErrorMessage").addClass("hide").text("");

		// 通信中の多重操作を防ぐ
		mgf.lockScreen();

		// セッション有効時のみAjaxを実行
		mgf.point.checkSessionAsync($(this)).then(function () {

			// 指定年月の履歴パーシャルを取得
			$.ajax({
				type: "POST",
				url: "../Point/LocalHistoryResult?ActionSource=Send",
				data: {
					__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
					"model.Year": $("select[name='Year']").val(),
					"model.Month": $("select[name='Month']").val()
				},
				traditional: true,
				async: true
			}).done(function (data, textStatus, jqXHR) {
				// 成功時: HTML(パーシャル)を受け取り履歴領域のみ更新
				if (jqXHR.status === 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
					$("main").find("#point_daily_log").replaceWith($(data));
					mgf.unlockScreen();
				// 入力検証エラー時: JSONメッセージを表示
				} else if (jqXHR.status === 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json") >= 0) {
					var resultJson = parseJsonResult(data);

					mgf.unlockScreen();

					if (resultJson && $.isFalseString(resultJson["IsSuccess"])) {
						$("#yearMonthErrorMessage").text(resultJson["Message"] || "入力内容が不正です。").removeClass("hide");
					} else {
						$("#yearMonthErrorMessage").text("履歴の取得に失敗しました。").removeClass("hide");
					}
				} else {
					// 予期しないレスポンス
					mgf.unlockScreen();
					$("#yearMonthErrorMessage").text("履歴の取得に失敗しました。").removeClass("hide");
				}
			}).fail(function () {
				// 通信失敗
				mgf.unlockScreen();
				$("#yearMonthErrorMessage").text("履歴の取得に失敗しました。").removeClass("hide");
			});
		}, mgf.redirectToLogin);
	}

	// ポイント変換を実行し、合計ポイントと履歴表示を更新
	function redeemLocalPoint() {

		hideRedeemError();

		mgf.lockScreen();

		mgf.point.checkSessionAsync($(this)).then(function () {

			// ポイント変換を実行し、成功時は履歴HTMLと残高を受け取る
			$.ajax({
				type: "POST",
				url: "../Point/LocalPointRedeemResult?ActionSource=Redeem",
				data: {
					__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
					"model.RedeemPoint": $("#localPointRedeemInput").val()
				},
				traditional: true,
				async: true
			}).done(function (data, textStatus, jqXHR) {
				if (jqXHR.status === 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json") >= 0) {
					var resultJson = parseJsonResult(data);

					mgf.unlockScreen();

					if (resultJson && $.isTrueString(resultJson["IsSuccess"])) {
						if (resultJson["HistoryHtml"]) {
							$("main").find("#point_daily_log").replaceWith($(resultJson["HistoryHtml"]));
						}

						setCurrentPoint(resultJson["Point"]);
						showRedeemSuccess(resultJson["Message"]);
					} else {
						showRedeemError(resultJson ? resultJson["Message"] : "ポイント変換に失敗しました。");
					}
				} else {
					mgf.unlockScreen();
					showRedeemError("ポイント変換に失敗しました。");
				}
			}).fail(function () {
				mgf.unlockScreen();
				showRedeemError("ポイント変換に失敗しました。");
			});
		}, mgf.redirectToLogin);
	}

	// 年月変更ボタン押下で履歴を非同期更新
	$("main").on("click", ".js-localhistory-change", function () {
		loadLocalHistory.call(this);

		return false;
	});

	$(document).on("click", ".js-localpoint-redeem", function () {
		// モーダル内の「変換する」押下
		redeemLocalPoint.call(this);

		return false;
	});

	$(document).on("show.bs.modal", "#cancel-modal", function () {
		// モーダル表示のたびに入力欄とエラー表示をリセット
		initializeRedeemDialog();
	});

	// theadクリックで次のtbodyを開閉
	$("main").on("click", "#log-table thead", function () {
		var $tbody = $(this).next("tbody");

		if ($tbody.length === 1) {
			$tbody.toggle();
		}

		return false;
	});



})();
