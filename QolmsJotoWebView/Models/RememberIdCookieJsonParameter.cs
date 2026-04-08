namespace MGF.QOLMS.QolmsJotoWebView
{
    internal class RememberIdCookieJsonParameter : QjJsonParameterBase
    {
        public RememberIdCookieJsonParameter()
        {
        }

        public string UserId { get; set; }
        public string LoginAt { get; set; }
    }
}