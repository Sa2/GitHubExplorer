using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Data.Json;
using Windows.ApplicationModel.Resources;
using Octokit;
using Octokit.Helpers;
using Octokit.Internal;
using Octokit.Reflection;
using GitHub_Explorer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHub_Explorer.Data
{
    public class IssueInfoDataGroup
    {
        public IssueInfoDataGroup(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.IssueComments = new ObservableCollection<IssueCommentDataItem>();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string RepositoryOwner { get; private set; }
        public string RepositoryName { get; private set; }
        public IssueDataItem IssueInfo { get; private set; }
        public ObservableCollection<IssueCommentDataItem> IssueComments { get; private set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class IssueInfoDataSource
    {
    }
}
