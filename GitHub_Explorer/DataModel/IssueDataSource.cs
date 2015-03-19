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
    public class IssueDataItem
    {
        public IssueDataItem(int number, string title, string body, ItemState state, IReadOnlyList<Label> labels,
                             Milestone milestone, int comments, User assignee, Uri htmlUrl, User user, DateTimeOffset createdAt, DateTimeOffset? closedAt)
        {
            this.Number = number;
            this.Title = title;
            this.Body = body;
            this.State = state;
            this.Labels = labels;
            this.Milestone = milestone;
            this.Comments = comments;
            this.Assignee = assignee;
            this.HtmlUrl = htmlUrl;
            this.User = user;
            this.CreatedAt = createdAt;
            this.ClosedAt = closedAt;
            
        }
        // issueの中身を知るために作成したオブジェクト
        Issue issue = new Issue();

        public int Number { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public ItemState State { get; private set; }
        public IReadOnlyList<Label> Labels { get; private set; }
        public Milestone Milestone { get; private set; }
        public int Comments { get; private set; }
        public User Assignee { get; private set; }
        public Uri HtmlUrl { get; private set; }
        public User User { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ClosedAt { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
        
    }

    public class IssuesDataGroup
    {
        // MEMO: ここにリポジトリのOwnerやNameを持たせるべきか…
        public IssuesDataGroup(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Issues = new ObservableCollection<IssueDataItem>();
        }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string RepositoryOwner { get; private set; }
        public string RepositoryName { get; private set; }
        public ObservableCollection<IssueDataItem> Issues { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
    public sealed class IssueDataSource
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private GitHubClientService clientService = new GitHubClientService();

        private static IssueDataSource _issueDataSource = new IssueDataSource();

        private ObservableCollection<IssuesDataGroup> _groups = new ObservableCollection<IssuesDataGroup>();

        public ObservableCollection<IssuesDataGroup> Groups
        {
            get { return this._groups; }
        }
        public static async Task<IEnumerable<IssuesDataGroup>> GetGroupAsync(string owner, string name)
        {
            await _issueDataSource.GetIssuesDataAsync(owner, name);

            return _issueDataSource.Groups;
        }

        public static async Task<IssuesDataGroup> GetGroupAsync(string id, string owner, string name)
        {
            await _issueDataSource.GetIssuesDataAsync(owner, name);
            var matches = _issueDataSource.Groups.Where((group) => group.Id.Equals(id));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<IssueDataItem> GetIssueAsync(int number, string owner, string name)
        {
            await _issueDataSource.GetIssuesDataAsync(owner, name);
            var matches = _issueDataSource.Groups.SelectMany((group) => group.Issues).Where((item) => item.Number.Equals(number));
            if (matches.Count() == 1) return matches.First();

            return null;
        }

        public async Task GetIssuesDataAsync(string owner, string name)
        {
//            if (this._groups.Count != 0)
//                return;

            IReadOnlyList<Issue> issues = await clientService.FetchIssues(owner, name);

            IssuesDataGroup group = new IssuesDataGroup(resourceLoader.GetString("PivotGroupIdIssues"), resourceLoader.GetString("PivotGroupNameIssues"));

            foreach (Issue issue in issues.Where(item => item.State.Equals(ItemState.Open)).OrderByDescending(item => item.Number))
            {
                group.Issues.Add(new IssueDataItem(issue.Number,
                                                    issue.Title,
                                                    issue.Body,
                                                    issue.State,
                                                    issue.Labels,
                                                    issue.Milestone,
                                                    issue.Comments,
                                                    issue.Assignee,
                                                    issue.HtmlUrl,
                                                    issue.User,
                                                    issue.CreatedAt,
                                                    issue.ClosedAt));
            }
            this.Groups.Clear();
            this.Groups.Add(group);
        }
    }
}
