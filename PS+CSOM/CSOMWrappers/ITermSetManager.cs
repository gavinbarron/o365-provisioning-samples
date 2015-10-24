using System;
using Microsoft.SharePoint.Client.Taxonomy;

namespace CSOMWrappers
{
    public interface ITermSetManager
    {
        void CreateTermSet(string termGroupName, string termSetName);
        TermGroup GetTermGroup(TermStore termStore, string termGroupName);
        Guid GetTermSetId(string termSetName, string termGroupName);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Guid GetTermStoreId();
    }
}