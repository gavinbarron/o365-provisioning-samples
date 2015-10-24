using System.Collections.Generic;
using Microsoft.SharePoint.Client;

namespace SiteProvisioningWeb
{
    public static class LibraryInfo
    {
        public static List<Library> Libraries
        {
            get
            {
                return new List<Library>()
                {
                    new Library()
                    {
                        Description = "Accreditation List",
                        Title = "Accreditation List",
                        VerisioningEnabled = false,
                        LibraryType = ListTemplateType.GenericList,
                        DefaultContentTypeId = "0x010078768679b88145f8965f070ee173080d"
                    },
                    new Library()
                    {
                        Description = "License Clauses List",
                        Title = "License Clauses",
                        VerisioningEnabled = false,
                        LibraryType = ListTemplateType.GenericList,
                        DefaultContentTypeId = "0x0100242ddd30e570455fa54df8e0f8068c28"
                    },
                    new Library()
                    {
                        Description = "License Request Library",
                        Title = "License List",
                        VerisioningEnabled = false,
                        LibraryType = ListTemplateType.DocumentLibrary,
                        DefaultContentTypeId = "0x0101001b7c15a2a337478da29736c1685a6423"
                    },
                    new Library()
                    {
                        Description = "Media List",
                        Title = "Media List",
                        VerisioningEnabled = false,
                        LibraryType = ListTemplateType.GenericList,
                        DefaultContentTypeId = "0x0100e30780cdf1fe48dd8c3cd0d1d4ec6677"
                    },
                };
            }
        }
    }
}