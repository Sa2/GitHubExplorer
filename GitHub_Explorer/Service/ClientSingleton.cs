using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Octokit;
using Octokit.Helpers;
using Octokit.Internal;
using Octokit.Reflection;

namespace GitHub_Explorer.Service
{
    class ClientSingleton
    {
        private static GitHubClient client = null;
        private static readonly object syncRoot = new object();
        private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private ClientSingleton() {}

        public static GitHubClient GetGitHubClient(Credentials credentials)
        {
            if (credentials == null && client == null)
            {
                var SignInContentDialog = new SignInContentDialog();
                SignInContentDialog.ShowAsync();
            }
            else if (client == null)
            {
                lock (syncRoot)
                {
                    if (client == null)
                    {
                        client = new GitHubClient(new ProductHeaderValue(resourceLoader.GetString("RegisteredAppName")),
                                                  new InMemoryCredentialStore(credentials));
                    }
                }
            }

            return client;
            
        }

        public static void DeleteGitHubClientObject()
        {
            client = null;
        }
    }
}
