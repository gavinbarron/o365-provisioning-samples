using System;
using System.Linq;
using System.Threading;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;

namespace CSOMWrappers
{
    public class SiteProvisioner
    {
        private readonly ClientContext _client;
        private const int _waitTime = 10000;

        /// <summary>
        /// Creates a SiteProvisioner object for use in automation tasks
        /// </summary>
        /// <param name="client"><see cref="ClientContext"/> to the Office 365 admin portal</param>
        public SiteProvisioner(ClientContext client)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;
        }

        /// <summary>
        /// Make a Site using SPO
        /// </summary>
        /// <param name="siteUrl">Url of the site to make</param>
        /// <param name="title">Title of the site to make</param>
        /// <param name="owner">Email of the account that will be the site owner</param>
        /// <param name="template">Name of the site template to use. e.g. STS#0</param>
        public void ProvisionSite(Uri siteUrl, string title, string owner, string template)
        {
            if (siteUrl == null) throw new ArgumentNullException("siteUrl");
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException("owner");
            if (string.IsNullOrEmpty(template)) throw new ArgumentNullException("template");
            ProvisionSite(siteUrl, title, owner, template, 100, 0);
        }

        /// <summary>
        /// Make a Site using SPO
        /// </summary>
        /// <param name="siteUrl">Url of the site to make</param>
        /// <param name="title">Title of the site to make</param>
        /// <param name="owner">Email of the account that will be the site owner</param>
        /// <param name="template">Name of the site template to use. e.g. STS#0</param>
        /// <param name="storageMaximumLevel">Maximum size of the site in MB</param>
        /// <param name="userCodeMaximumLevel">UserCode Resource Points Allowed</param>
        public void ProvisionSite(Uri siteUrl, string title, string owner, string template, int storageMaximumLevel, int userCodeMaximumLevel)
        {
            if (siteUrl == null) throw new ArgumentNullException("siteUrl");
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");
            if (string.IsNullOrEmpty(owner)) throw new ArgumentNullException("owner");
            if (string.IsNullOrEmpty(template)) throw new ArgumentNullException("template");

            var tenant = new Tenant(_client);

            var siteCreationProperties = new SiteCreationProperties
            {
                Url = siteUrl.ToString(),
                Title = title,
                Owner = owner,
                Template = template,
                StorageMaximumLevel = storageMaximumLevel,
                UserCodeMaximumLevel = userCodeMaximumLevel
            };
            SpoOperation spo = tenant.CreateSite(siteCreationProperties);

            ExecuteAndWaitForCompletion(tenant, spo);
        }

        /// <summary>
        /// Delete the SiteCollection at the given Uri, also removes it from the recycle bin.
        /// Checks whether the site exists before deletion.
        /// </summary>
        /// <param name="siteUrl"><see cref="Uri"/> of the site to delete</param>
        public void DeleteSite(Uri siteUrl)
        {
            if (siteUrl == null) throw new ArgumentNullException("siteUrl");
            var tenant = new Tenant(_client);
            string siteToRemoveUrl = siteUrl.ToString().ToLowerInvariant();

            // Check whether the site exists.
            SPOSitePropertiesEnumerable sitePropEnumerable = tenant.GetSiteProperties(0, true);
            _client.Load(sitePropEnumerable);
            _client.ExecuteQuery();
            bool siteExists = Enumerable.Any(sitePropEnumerable,
                property => property.Url.ToLowerInvariant() == siteToRemoveUrl);

            if (siteExists)
            {
                SpoOperation spo = tenant.RemoveSite(siteUrl.ToString());

                ExecuteAndWaitForCompletion(tenant, spo);

                spo = tenant.RemoveDeletedSite(siteUrl.ToString());

                ExecuteAndWaitForCompletion(tenant, spo);
            }
        }

        /// <summary>
        /// Executes tennant operations, waits with a timeout until completed
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="spo"></param>
        private void ExecuteAndWaitForCompletion(Tenant tenant, SpoOperation spo)
        {
            _client.Load(tenant);
            _client.Load(spo, i => i.IsComplete);
            _client.ExecuteQuery();

            //Check if provisioning of the SiteCollection is complete.
            while (!spo.IsComplete)
            {
                //Wait for a bit and then try again
                Thread.Sleep(_waitTime);
                spo.RefreshLoad();
                _client.ExecuteQuery();
            }
        }
    }
}
