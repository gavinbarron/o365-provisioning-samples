using System;
using Microsoft.SharePoint.Client;
using NUnit.Framework;

namespace CSOMWrappers.Tests
{
    [TestFixture]
    public class SiteProvisioningTests
    {
        private ClientContext _adminContext;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _adminContext = ContextFixture.GenerateAdminContext();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            _adminContext.Dispose();
        }

        [Test]
        public void CreateNewSiteCollection()
        {
            var provisioner = new SiteProvisioner(_adminContext);

            var siteUrl = Guid.NewGuid().ToString("N");

            var url = new Uri("https://[tennant-name].sharepoint.com/sites/"+ siteUrl);

            Assert.DoesNotThrow(() =>
            {
                provisioner.ProvisionSite(url, siteUrl, TestConstants.OnlineLoginName, "STS#0");
            });

            ClientContext newSiteContext = ContextFixture.GenerateContext(url);

            
            newSiteContext.Load(newSiteContext.Web);
            newSiteContext.ExecuteQuery();
            string title = newSiteContext.Web.Title;
            Assert.AreEqual(siteUrl, title);
        }
    }
}
