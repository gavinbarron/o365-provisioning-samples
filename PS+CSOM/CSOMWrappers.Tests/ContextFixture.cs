using System;
using Microsoft.SharePoint.Client;

namespace CSOMWrappers.Tests
{
    public static class ContextFixture
    {
        public static ClientContext GenerateAdminContext()
        {
            return ManagerFactory.CreateContext(new Uri(TestConstants.OnlineTennantUrl),
                TestConstants.OnlineLoginName,
                TestConstants.OnlinePassword);
        }


        public static ClientContext GenerateContext(Uri siteUri)
        {
            return ManagerFactory.CreateContext(siteUri,
                TestConstants.OnlineLoginName,
                TestConstants.OnlinePassword);
        }
    }
}