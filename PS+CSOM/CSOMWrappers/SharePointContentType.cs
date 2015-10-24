using System;

namespace CSOMWrappers
{
    /// <summary>
    ///  SharePoint site column business object
    /// </summary>
    public class SharePointContentType
    {
        private readonly string _contentTypeName;
        private readonly string _groupName;
        private readonly string _parentContentTypeName;
        private readonly string _contentTypeId;

        //TODO will refactor later
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public SharePointContentType(string contentTypeName, string groupName, string contentTypeId = "", string parentContentTypeName = "Item")
        {
            if (contentTypeName == null) throw new ArgumentNullException("contentTypeName");
            if (groupName == null) throw new ArgumentNullException("groupName");
            if (parentContentTypeName == null) throw new ArgumentNullException("parentContentTypeName");
            _contentTypeName = contentTypeName;
            _groupName = groupName;
            _contentTypeId = contentTypeId;
            _parentContentTypeName = parentContentTypeName;
        }


        public string ContentTypeName
        {
            get
            {
                return _contentTypeName;
            }
        }

        public string GroupName
        {
            get
            {
                return _groupName;
            }
        }

        public string ContentTypeId
        {
            get
            {
                if (string.IsNullOrEmpty(_contentTypeId))
                {
                    string guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    return "0x0100" + guid;
                }
                return _contentTypeId;
            }
        }

        public string ParentContentTypeName
        {
            get
            {
                return _parentContentTypeName;
            }
        }
    }
}