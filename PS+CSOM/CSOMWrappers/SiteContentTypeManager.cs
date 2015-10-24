using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace CSOMWrappers
{
    /// <summary>
    /// Site Content Manager responsible for creating content type and
    /// link site column to content type 
    /// </summary>
    public class SiteContentTypeManager
    {
        private readonly ClientContext _clientContext;
        private readonly Web _rootWeb;

        /// <summary>
        /// Site Content Manager
        /// </summary>
        /// <param name="clientContext"><see cref="ClientContext"/> to connect to the site</param>
        public SiteContentTypeManager(ClientContext clientContext)
        {
            if (clientContext == null) throw new ArgumentNullException("clientContext");
            _clientContext = clientContext;
            _rootWeb = _clientContext.Site.RootWeb;
        }

        /// <summary>
        /// Create Content Type by id
        /// </summary>
        /// <param name="contentType"></param>
        public void CreateSiteContentTypeById(SharePointContentType contentType)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            ContentTypeCreationInformation contentTypeCreationInformation = new ContentTypeCreationInformation
            {
                Name = contentType.ContentTypeName,
                Id = contentType.ContentTypeId,
                Group = contentType.GroupName
            };

            _rootWeb.ContentTypes.Add(contentTypeCreationInformation);
            _clientContext.ExecuteQuery();
        }

        //TODO this method need to be refactor later use create by id
        /// <summary>
        /// Create content type by reference
        /// </summary>
        /// <param name="contentType"></param>
        public void CreateSiteContentTypeByReference(SharePointContentType contentType)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            string parentContentTypeName = contentType.ParentContentTypeName;
            ContentTypeCollection allContentTypes = _rootWeb.ContentTypes;
            _clientContext.Load(allContentTypes, cts => cts.Include(ct => ct.Name));
            _clientContext.ExecuteQuery();
            ContentType parentContentType = allContentTypes.FirstOrDefault(ct => ct.Name == parentContentTypeName);
            if (parentContentType != null)
            {
                ContentTypeCreationInformation contentTypeCreationInformation = new ContentTypeCreationInformation
                {
                    Name = contentType.ContentTypeName,
                    ParentContentType = parentContentType,
                    Group = contentType.GroupName
                };
                _rootWeb.ContentTypes.Add(contentTypeCreationInformation);
                _clientContext.ExecuteQuery();
            }
            else
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Parent Content Type {0} not found", parentContentTypeName));
            }
        }

        /// <summary>
        /// Link site columns to content type
        /// </summary>
        /// <param name="siteColumnInternalNames"></param>
        /// <param name="targetContentTypeName"></param>
        public void AddSiteColumns(IList<string> siteColumnInternalNames, string targetContentTypeName, bool isMandatory)
        {
            if (siteColumnInternalNames == null) throw new ArgumentNullException("siteColumnInternalNames");
            if (targetContentTypeName == null) throw new ArgumentNullException("targetContentTypeName");
            foreach (string internalName in siteColumnInternalNames)
            {
                AddSiteColumn(internalName, targetContentTypeName, isMandatory);
            }
        }

        /// <summary>
        /// Link site column to content type
        /// </summary>
        /// <param name="siteColumnInternalName"></param>
        /// <param name="targetContentTypeName"></param>
        /// <param name="isMandatory"></param>
        public void AddSiteColumn(string siteColumnInternalName, string targetContentTypeName, bool isMandatory)
        {
            if (siteColumnInternalName == null) throw new ArgumentNullException("siteColumnInternalName");
            if (targetContentTypeName == null) throw new ArgumentNullException("targetContentTypeName");
            Field field = _rootWeb.Fields.GetByInternalNameOrTitle(siteColumnInternalName);
            ContentTypeCollection allContentTypes = _rootWeb.ContentTypes;
            _clientContext.Load(allContentTypes, cts => cts.Include(ct => ct.Name));
            _clientContext.ExecuteQuery();
            ContentType targetContentType = allContentTypes.FirstOrDefault(ct => ct.Name == targetContentTypeName);
            if (field != null && targetContentType != null)
            {
                FieldLinkCreationInformation creationInformation = new FieldLinkCreationInformation
                {
                    Field = field
                };
                FieldLink fieldLink = targetContentType.FieldLinks.Add(creationInformation);
                fieldLink.Required = isMandatory;
                targetContentType.Update(true);
                _clientContext.ExecuteQuery();
            }
            else
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Site Column {0} or Content Type {1} cannot be found", siteColumnInternalName,
                    targetContentTypeName));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public bool SiteContentTypeExist(string contentTypeName)
        {
            ContentTypeCollection contentTypeCollection = _rootWeb.ContentTypes;

            _clientContext.Load(contentTypeCollection,
                contentType => contentType.Include(f => f.Name).Where(ct => ct.Name == contentTypeName));
            _clientContext.ExecuteQuery();
            return contentTypeCollection.Count > 0;
        }
    }
}