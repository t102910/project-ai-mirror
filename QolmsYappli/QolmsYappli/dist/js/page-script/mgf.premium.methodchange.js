mgf.premium.methodchange = mgf.premium.methodchange || {};

mgf.premium.methodchange = (function () {

	// ヒストリーバックの禁止
	mgf.prohibitHistoryBack();

	// submit
	$(document).on("click", "button[type='submit']", function () {
		if ($("form")[0].checkValidity()) {
			// 画面をロック後、サブミット
			mgf.premium.promiseLockScreen($(this));
		}
		return true;
	});

	$('main').on("change", "input[name='payment']", function () {
		//console.log($('input[name="payment"]:checked').val());
		$('#paymentType').val($('input[name="payment"]:checked').val());
	})

})();

