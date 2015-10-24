using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace CSOMWrappers
{
    /// <summary>
    /// Site column manager resposible for creating site columns
    /// </summary>
    public class SiteColumnManager
    {
        private readonly ClientContext _clientContext;
        private readonly ITermSetManager _termSetManager;
        private readonly Web _rootWeb;

        /// <summary>
        /// Used to creates site columns
        /// </summary>
        /// <param name="clientContext"><see cref="ClientContext"/> connected to the site where site columns are to be created</param>
        /// <param name="termSetManager"></param>
        public SiteColumnManager(ClientContext clientContext, ITermSetManager termSetManager)
        {
            if (clientContext == null) throw new ArgumentNullException("clientContext");
            if (termSetManager == null) throw new ArgumentNullException("termSetManager");
            _clientContext = clientContext;
            _termSetManager = termSetManager;
            _rootWeb = clientContext.Site.RootWeb;
        }

        /// <summary>
        /// Create normal site column
        /// </summary>
        /// <param name="siteColumn"></param>
        public void CreateSiteColumn(SharePointSiteColumn siteColumn)
        {
            if (siteColumn == null) throw new ArgumentNullException("siteColumn");
            string fieldSchema = siteColumn.FieldSchema;
            bool addToDefaultView = siteColumn.AddToDefaultView;
            AddFieldOptions addFieldOptions = siteColumn.AddFieldOptions;
            _rootWeb.Fields.AddFieldAsXml(fieldSchema, addToDefaultView, addFieldOptions);
            _clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Create managed meta data site column
        /// </summary>
        /// <param name="siteColumn"></param>
        /// <param name="termSetName"></param>
        /// <param name="termGroupName"></param>
        public void CreateManagedMetadataSiteColumn(SharePointSiteColumn siteColumn, string termSetName, string termGroupName)
        {
            if (siteColumn == null) throw new ArgumentNullException("siteColumn");

            Guid termStoreId = _termSetManager.GetTermStoreId();
            Guid termSetId = _termSetManager.GetTermSetId(termSetName, termGroupName);

            string fieldSchema = siteColumn.FieldSchema;
            bool addToDefaultView = siteColumn.AddToDefaultView;
            AddFieldOptions addFieldOptions = siteColumn.AddFieldOptions;
            Field newField = _rootWeb.Fields.AddFieldAsXml(fieldSchema, addToDefaultView, addFieldOptions);
            _clientContext.ExecuteQuery();

            TaxonomyField taxonomyField = _clientContext.CastTo<TaxonomyField>(newField);
            taxonomyField.SspId = termStoreId;
            taxonomyField.TermSetId = termSetId;
            taxonomyField.TargetTemplate = string.Empty;
            taxonomyField.AnchorId = Guid.Empty;
            taxonomyField.AllowMultipleValues = true;
            taxonomyField.Update();
            _clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Check if the column exists
        /// </summary>
        /// <param name="columnInternalName"></param>
        /// <returns></returns>
        public bool SiteColumnExist(string columnInternalName)
        {
            FieldCollection fieldCollection = _rootWeb.Fields;

            _clientContext.Load(fieldCollection,
                fields => fields.Include(f => f.InternalName).Where(f => f.InternalName == columnInternalName));
            _clientContext.ExecuteQuery();
            return fieldCollection.Count > 0;
        }
    }
}