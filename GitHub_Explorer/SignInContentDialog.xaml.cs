using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Octokit;
using Octokit.Helpers;
using Octokit.Internal;
using Octokit.Reflection;
using GitHub_Explorer.Service;
using Windows.UI.Popups;

// コンテンツ ダイアログ項目テンプレートについては、http://go.microsoft.com/fwlink/?LinkID=390556 を参照してください

namespace GitHub_Explorer
{
    public sealed partial class SignInContentDialog : ContentDialog
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public SignInContentDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string message;
            GitHubClientService github;

            if (username.Text != "" && password.Password != "")
            {
                github = new GitHubClientService();
                Credentials credentials = new Credentials(username.Text, password.Password);
                message = await github.SignIn(credentials);
            }
            else
            {
                message = this.resourceLoader.GetString("SignInInputErrorMsg");
            }

            var messageDialog = new MessageDialog(message);

            body.Text = message;

            await messageDialog.ShowAsync();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
