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

namespace GitHub_Explorer.Data
{
    public class RepositoryDataItem
    {
        public RepositoryDataItem(int id, String name, String description, String ownerName, bool Private)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.OwnerName = ownerName;
            this.Private = Private;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string OwnerName { get; private set; }
        public bool Private { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

    }
    public class RepositoryDataGroup
    {
        public RepositoryDataGroup(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Items = new ObservableCollection<RepositoryDataItem>();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<RepositoryDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public sealed class RepositoryDataSource
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private GitHubClientService clientService = new GitHubClientService();

        private static RepositoryDataSource _repositoryDataSource = new RepositoryDataSource();

        private ObservableCollection<RepositoryDataGroup> _groups = new ObservableCollection<RepositoryDataGroup>();

        public ObservableCollection<RepositoryDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<RepositoryDataGroup>> GetGroupAsync()
        {
            await _repositoryDataSource.GetRepositoryDataAsync();

            return _repositoryDataSource.Groups;
        }

        public static async Task<RepositoryDataGroup> GetGroupAsync(string id)
        {
            await _repositoryDataSource.GetRepositoryDataAsync();
            var matches = _repositoryDataSource.Groups.Where((group) => group.Id.Equals(id));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<RepositoryDataItem> GetItemAsync(int id)
        {
            await _repositoryDataSource.GetRepositoryDataAsync();
            var matches = _repositoryDataSource.Groups.SelectMany((group) => group.Items).Where((item) => item.Id.Equals(id));
            if (matches.Count() == 1) return matches.First();

            return null;
        }

        public async Task GetRepositoryDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            GitHubClientService github = new GitHubClientService();
            Task<IReadOnlyList<Repository>> repositories = github.FetchCurrentUserRepositories();

            RepositoryDataGroup group = new RepositoryDataGroup(this.resourceLoader.GetString("PivotGroupIdRepositories"),
                                                            this.resourceLoader.GetString("PivotGroupNameRepositories"));

            foreach (Repository repository in repositories.Result)
            {
                group.Items.Add(new RepositoryDataItem(repository.Id,
                                                       repository.Name,
                                                       repository.Description,
                                                       repository.Owner.Login,
                                                       repository.Private));
            }
            this.Groups.Add(group);
        }
    }
}
