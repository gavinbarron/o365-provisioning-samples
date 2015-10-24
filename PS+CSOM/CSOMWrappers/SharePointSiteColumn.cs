using System;
using System.Globalization;
using Microsoft.SharePoint.Client;

namespace CSOMWrappers
{
    public class SharePointSiteColumn
    {
        private const string Template = "<Field DisplayName='{0}' Name='{1}' ID='{2}' Group='{3}' Type='{4}' />";
        private const string DateOnlyTemplate = "<Field DisplayName='{0}' Name='{1}' ID='{2}' Group='{3}' Type='DateTime' Format='DateOnly' />";
        private const string RichTemplate = "<Field DisplayName='{0}' Name='{1}' ID='{2}' Group='{3}' Type='{4}' RichText='TRUE' RichTextMode='ThemeHtml' />";
        private readonly string _displayName;
        private readonly string _columnId;
        private readonly string _groupName;
        private readonly string _columnType;
        private readonly string _internalName;

        public SharePointSiteColumn(string displayName, string columnId, string groupName, string columnType, bool addToDefaultView, AddFieldOptions addFieldOptions)
        {
            if (displayName == null) throw new ArgumentNullException("displayName");
            if (columnId == null) throw new ArgumentNullException("columnId");
            if (groupName == null) throw new ArgumentNullException("groupName");
            if (columnType == null) throw new ArgumentNullException("columnType");
            _displayName = displayName;
            _columnId = columnId;
            _groupName = groupName;
            _columnType = columnType;
            _internalName = _displayName.Replace(" ", string.Empty).Trim();
            AddFieldOptions = addFieldOptions;
            AddToDefaultView = addToDefaultView;
        }

        public string FieldSchema
        {
            get
            {
                string columnIdString = "{" + _columnId + "}";
                string template = "";
                if (_columnType == "HTML" || _columnType == "Image")
                    template = RichTemplate;
                else if (_columnType == "DateOnly")
                {
                    template = DateOnlyTemplate;
                }
                else
                {
                    template = Template;
                }
                return string.Format(CultureInfo.InvariantCulture, template, _displayName, _internalName, columnIdString, _groupName, _columnType);
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string InternalName
        {
            get { return _internalName; }
        }

        public bool AddToDefaultView { get; private set; }

        public AddFieldOptions AddFieldOptions { get; private set; }
    }
}