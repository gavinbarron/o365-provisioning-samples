﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Web.UI;
﻿using System.Xml.Linq;
﻿using Microsoft.SharePoint.Client;
using System.Globalization;

namespace SiteProvisioningWeb
{
    public partial class Default : Page
    {
#region  Page Boilerplate
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }
            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };
                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);
        }
#endregion

        protected void AddListsToHostWeb(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                CreateSiteColumns(clientContext);
                CreateContentTypes(clientContext);
                CreateLists(clientContext);
                AddClausesToList(clientContext);
            }
        }

        private void CreateSiteColumns(ClientContext cc)
        {
            var document = XDocument.Load(Server.MapPath("~/XmlDefinitions/Fields.xml"));
            XNamespace ns = "http://schemas.microsoft.com/sharepoint/";
            IEnumerable<XElement> fields = from m in document.Descendants(ns + "Field")
                                           select m;
            foreach (XElement fieldElement in fields)
            {
                var idAttribute = fieldElement.Attribute("ID");
                if (!cc.Web.FieldExistsById(idAttribute.Value))
                {
                    cc.Web.Fields.AddFieldAsXml(fieldElement.ToString(), false, AddFieldOptions.AddToNoContentType);
                    cc.ExecuteQuery();
                }
            }
        }

        private void CreateContentTypes(ClientContext clientContext)
        {
            var document = XDocument.Load( Server.MapPath("~/XmlDefinitions/ContentTypes.xml"));
            XNamespace ns = "http://schemas.microsoft.com/sharepoint/";
            IEnumerable<XElement> contentTypeElements = from c in document.Descendants(ns + "ContentType")
                                                        select c;
            foreach (XElement contentTypeElement in contentTypeElements)
            {
                XAttribute id = contentTypeElement.Attribute("ID");
               
                if (!clientContext.Web.ContentTypeExistsById(id.Value))
                {
                    var xDocument = contentTypeElement.Document;
                    clientContext.Web.CreateContentTypeFromXML(xDocument);
                }
            }
            clientContext.ExecuteQuery();
        }

        private void CreateLists(ClientContext clientContext)
        {
            var libraries = LibraryInfo.Libraries;

            foreach (Library library in libraries)
            {
                CreateLibrary(clientContext, library);
            }
        }

        private void CreateLibrary(ClientContext ctx, Library library)
        {
            if (!ctx.Web.ListExists(library.Title))
            {
                ctx.Web.CreateList(library.LibraryType, library.Title, false);
                List list = ctx.Web.GetListByTitle(library.Title);

                if (!string.IsNullOrEmpty(library.Description))
                {
                    list.Description = library.Description;
                }

                if (library.VerisioningEnabled)
                {
                    list.EnableVersioning = true;
                }

                list.ContentTypesEnabled = true;
                list.Update();
                ctx.Web.AddContentTypeToListById(library.Title, library.DefaultContentTypeId, true);

                //we are going to remove the default Content Type
                switch (library.LibraryType)
                {
                    case ListTemplateType.DocumentLibrary:
                        list.RemoveContentTypeByName("Document");
                        break;
                    case ListTemplateType.GenericList:
                        list.RemoveContentTypeByName("Item");
                        break;
                }
                ctx.Web.Context.ExecuteQuery();
            }
        }


        private void AddClausesToList(ClientContext clientContext)
        {
            List list = clientContext.Web.Lists.GetByTitle("License Clauses");
            var document = XDocument.Load(Server.MapPath("~/XmlDefinitions/LicenseClauses.xml"));
            IEnumerable<XElement> items = from i in document.Descendants("Row")
                                          select i;
            foreach (XElement itemElement in items)
            {
                XElement clauseId = itemElement.Descendants("Field")
                    .FirstOrDefault(x => (string)x.Attribute("Name") == "ClauseID");
                //Check if the item exits, if so then don't attempt to add it again and move to the next item.
                if (ClauseExists(clientContext, list, clauseId)) continue;

                XElement title = itemElement.Descendants("Field")
                    .FirstOrDefault(x => (string)x.Attribute("Name") == "Title");
                XElement clauseType = itemElement.Descendants("Field")
                    .FirstOrDefault(x => (string)x.Attribute("Name") == "ClauseType");
                XElement licenseClause = itemElement.Descendants("Field")
                    .FirstOrDefault(x => (string)x.Attribute("Name") == "LicenseClause");
                var itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = list.AddItem(itemCreateInfo);
                newItem["ClauseID"] = int.Parse(clauseId.Value);
                newItem["Title"] = title.Value;
                newItem["ClauseType"] = clauseType.Value;
                newItem["LicenseClause"] = licenseClause.Value;
                newItem.Update();
            }
            clientContext.ExecuteQuery();
        }

        private bool ClauseExists(ClientContext clientContext, List list, XElement clauseId)
        {
            const string clauseQueryTemplate = "<Query><Where><Eq><FieldRef Name=\"ClauseID\" /><Value Type=\"Number\">{0}</Value></Eq></Where></Query>";
            var query = new CamlQuery
            {
                ViewXml = string.Format(CultureInfo.InvariantCulture, clauseQueryTemplate, clauseId)
            };
            ListItemCollection queryResults = list.GetItems(query);
            clientContext.Load(queryResults);
            clientContext.ExecuteQuery();
            return queryResults.Count > 0;
        }
        #region CleanUp
        protected void DeleteMediaAssetsColumns(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                DeleteSiteColumns(clientContext);
            }
        }
        protected void DeleteMediaAssetsContentTypes(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                DeleteSiteContentTypes(clientContext);
            }
        }

        private void DeleteSiteColumns(ClientContext clientContext)
        {
            FieldCollection fields = clientContext.Web.Fields;
            clientContext.Load(fields);
            clientContext.ExecuteQuery();
            IList<Field> fieldsToDelete = fields.Where(field => field.Group == "Media Asset Library").ToList();
            foreach (Field deleteMe in fieldsToDelete)
            {
                deleteMe.DeleteObject();
            }
            clientContext.ExecuteQuery();
        }

        private void DeleteSiteContentTypes(ClientContext clientContext)
        {
            ContentTypeCollection contentTypes = clientContext.Web.ContentTypes;
            clientContext.Load(contentTypes);
            clientContext.ExecuteQuery();
            IList<ContentType> contentTypesToDelete = contentTypes.Where(contentType => contentType.Group == "Media Asset Library").ToList();
            foreach (ContentType deleteMe in contentTypesToDelete)
            {
                deleteMe.DeleteObject();
            }
            clientContext.ExecuteQuery();
        }

#endregion
    }

}