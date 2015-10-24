using System;
using System.Security;
using Microsoft.SharePoint.Client;

namespace CSOMWrappers
{
    public static class ManagerFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static SiteProvisioner CreateSiteProvisioner(Uri siteUri, string userName, string password)
        {
            return new SiteProvisioner(CreateContext(siteUri, userName, password));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope")]
        public static ClientContext CreateContext(Uri siteUri, string userName, string password)
        {
            if (siteUri == null) throw new ArgumentNullException("siteUri");
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            ClientContext clientContext = new ClientContext(siteUri);
            if (userName.Contains("@"))
            {
                var passPhrase = new SecureString();

                foreach (char c in password)
                {
                    passPhrase.AppendChar(c);
                }

                clientContext.Credentials = new SharePointOnlineCredentials(userName, passPhrase);
            }
            return clientContext;
        }
    }
}