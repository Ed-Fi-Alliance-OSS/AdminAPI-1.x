namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class TemporaryAuthenticationHelper
    {
        public static bool RequestIsAuthenticatedOrTemporarilyAssumingAuthenticated()
        {
        #if NET48
            return Request.IsAuthenticated();
        #else
            //TODO: We must temporarily assume the user is authenticated,
            //      until authentication under .NET Core becomes enabled
            //      in AA-1120. At that time, this must begin returning
            //      an accurate value similar to the NET48 implementation,
            //      and at that time likely inline this method back to a simple
            //      expression at the call site(s).
            return true;
        #endif
        }
    }
}
