namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class TemporaryAuthenticationHelper
    {
        public static bool RequestIsAuthenticatedOrTemporarilyAssumingAuthenticated()
        {
        #if NET48
            return Request.IsAuthenticated();
        #else
            return true;
        #endif
        }
    }
}
