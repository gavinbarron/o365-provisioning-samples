using System;
using Microsoft.SharePoint.Client;

namespace SiteProvisioningWeb
{
    public class ColumnDescriptor
    {
        string _fieldName = "ContosoStringCSOM";
        FieldType _columnType = FieldType.Text;
        private Guid _columnId = Guid.NewGuid();
        string _displayName = "Contoso String CSOM";
        string _groupName = "Hybrid Solutions Demo";

        public ColumnDescriptor(string fieldName, FieldType columnType, string displayName, string groupName, string columnGuid) : this(fieldName, columnType, displayName, groupName, Guid.Parse(columnGuid))
        {
        }

        public ColumnDescriptor(string fieldName, FieldType columnType, string displayName, string groupName) : this(fieldName, columnType, displayName, groupName,Guid.NewGuid())
        {
        }

        public ColumnDescriptor(string fieldName, FieldType columnType, string displayName, string groupName, Guid columnId)
        {
            _fieldName = fieldName;
            _columnType = columnType;
            _displayName = displayName;
            _groupName = groupName;
            _columnId = columnId;
        }

        public ColumnDescriptor()
        {
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public FieldType ColumnType
        {
            get { return _columnType; }
            set { _columnType = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        public Guid ColumnId
        {
            get { return _columnId; }
            set { _columnId = value; }
        }
    }
}