mgf.premium.payjpagree = mgf.premium.payjpagree || {};

mgf.premium.payjpagree = (function () {

	// スクロール エリア描画
	$(function () {
		var bb = $('.bottom-fix-wrap').outerHeight() + 100;
		var ds = $('.document-scroll-wrap').outerHeight();
		if ($(window).height() < bb + ds) {
			$('.document-scroll-wrap').css({ 'height': 'calc(100vh - ' + bb + 'px)' });
		}
	});

	// 規約同意
	// --同意チェックでチャレンジボタンの表示を切替
	$("body").on("change", "#policy", function () {

		if ($(this).prop('checked')) {
			console.log("checked");
			$("a.agree").removeClass("disabled")

		} else {
			console.log("checked=false");

			$("a.agree").addClass("disabled")
		}
	});

	// ヒストリーバックの禁止
	mgf.prohibitHistoryBack();

	//// submit
	//$(document).on("click", "button[type='submit']", function () {
	//	if ($("form")[0].checkValidity()) {
	//		// 画面をロック後、サブミット
	//		mgf.premium.promiseLockScreen($(this));
	//	}
	//	return true;
	//});

	//$('main').on("change", "input[name='payment']", function () {
	//	//console.log($('input[name="payment"]:checked').val());
	//	$('#paymentType').val($('input[name="payment"]:checked').val());
	//})

})();
