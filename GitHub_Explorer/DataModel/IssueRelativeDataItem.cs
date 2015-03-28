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

    public class IssueCommentDataItem
    {
        
        
    }
}
