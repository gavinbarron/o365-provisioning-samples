using Microsoft.SharePoint.Client;

namespace SiteProvisioningWeb
{
    public class Library
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool VerisioningEnabled { get; set; }
        public ListTemplateType LibraryType { get; set; }
        public string DefaultContentTypeId { get; set; }
    }
}