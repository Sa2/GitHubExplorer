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
    public class RepositoryListDataItem
    {
        public RepositoryListDataItem(int id, String name, String description, String ownerName, bool Private, DateTimeOffset? pushedAt)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.OwnerName = ownerName;
            this.Private = Private;
            this.PushedAt = pushedAt;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string OwnerName { get; private set; }
        public bool Private { get; private set; }
        public DateTimeOffset? PushedAt { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

    }
    public class RepositoryListDataGroup
    {
        public RepositoryListDataGroup(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Items = new ObservableCollection<RepositoryListDataItem>();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<RepositoryListDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public sealed class RepositoryListDataSource
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private GitHubClientService clientService = new GitHubClientService();

        private static RepositoryListDataSource _repositoryListDataSource = new RepositoryListDataSource();

        private ObservableCollection<RepositoryListDataGroup> _groups = new ObservableCollection<RepositoryListDataGroup>();

        public ObservableCollection<RepositoryListDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<RepositoryListDataGroup>> GetGroupAsync()
        {
            await _repositoryListDataSource.GetRepositoryListDataAsync();

            return _repositoryListDataSource.Groups;
        }

        public static async Task<RepositoryListDataGroup> GetGroupAsync(string id)
        {
            await _repositoryListDataSource.GetRepositoryListDataAsync();
            var matches = _repositoryListDataSource.Groups.Where((group) => group.Id.Equals(id));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<RepositoryListDataItem> GetItemAsync(int id)
        {
            await _repositoryListDataSource.GetRepositoryListDataAsync();
            var matches = _repositoryListDataSource.Groups.SelectMany((group) => group.Items).Where((item) => item.Id.Equals(id));
            if (matches.Count() == 1) return matches.First();

            return null;
        }

        public async Task GetRepositoryListDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Task<IReadOnlyList<Repository>> repositories = clientService.FetchCurrentUserRepositories();

            RepositoryListDataGroup group = new RepositoryListDataGroup(this.resourceLoader.GetString("PivotGroupIdRepositories"),
                                                            this.resourceLoader.GetString("PivotGroupNameRepositories"));

            foreach (Repository repository in repositories.Result.OrderByDescending(item => item.PushedAt))
            {
                group.Items.Add(new RepositoryListDataItem(repository.Id,
                                                       repository.Name,
                                                       repository.Description,
                                                       repository.Owner.Login,
                                                       repository.Private,
                                                       repository.PushedAt));
                
            }

            this.Groups.Add(group);
        }
    }
}
