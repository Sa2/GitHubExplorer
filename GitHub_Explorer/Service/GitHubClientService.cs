using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Octokit;
using Octokit.Helpers;
using Octokit.Internal;
using Octokit.Reflection;

namespace GitHub_Explorer.Service
{
    class GitHubClientService
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private GitHubClient github;
        public GitHubClientService()
        {

        }

        public async Task<String> SignIn(Credentials credentials)
        {
            
            try {
                github = ClientSingleton.GetGitHubClient(credentials);
                IReadOnlyList<Repository> repo = FetchCurrentUserRepositories().Result;
            }
            catch (Exception exception)
            {
                ClientSingleton.DeleteGitHubClientObject();
                return this.resourceLoader.GetString("SignInFailedMsg");
            }
            return this.resourceLoader.GetString("SignInSuccessfulMsg");
            
        }
        public async Task<IReadOnlyList<Repository>> FetchCurrentUserRepositories()
        {
            github = ClientSingleton.GetGitHubClient(null);
            IReadOnlyList<Repository> repositories = github.Repository.GetAllForCurrent().Result;

            return repositories;
        }

        public async Task<IReadOnlyList<Repository>> FetchUserRepositories(string userId)
        {
            github = ClientSingleton.GetGitHubClient(null);
            IReadOnlyList<Repository> repositories = github.Repository.GetAllForUser(userId).Result;

            return repositories;
        }

        public async Task<IReadOnlyList<Issue>> FetchIssues(string owner, string name)
        {
            github = ClientSingleton.GetGitHubClient(null);
            IReadOnlyList<Issue> issues = github.Issue.GetForRepository(owner, name).Result;

            return issues;
        }
    }
}
