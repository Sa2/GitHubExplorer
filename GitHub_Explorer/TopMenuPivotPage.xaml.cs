using GitHub_Explorer.Common;
using GitHub_Explorer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Octokit;
using Octokit.Helpers;
using Octokit.Internal;
using Octokit.Reflection;
using GitHub_Explorer.Service;
using GitHub_Explorer.NavigationParam;

// ピボット アプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkID=391641 を参照してください

namespace GitHub_Explorer
{
    public sealed partial class TopMenuPivotPage : Page
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public TopMenuPivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// この <see cref="Page"/> に関連付けられた <see cref="NavigationHelper"/> を取得します。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// この <see cref="Page"/> のビュー モデルを取得します。
        /// これは厳密に型指定されたビュー モデルに変更できます。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="sender">
        /// イベントのソース (通常、<see cref="NavigationHelper"/>)。
        /// </param>
        /// <param name="e">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたナビゲーション パラメーターと、
        /// 前のセッションでこのページによって保存された状態の辞書を提供する
        /// セッション。ページに初めてアクセスするとき、状態は null になります。</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: 対象となる問題領域に適したデータ モデルを作成し、サンプル データを置き換えます
//            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
//            this.DefaultViewModel[FirstGroupName] = sampleDataGroup;

//            try
//            {
//                var repositoryDataGroup = await RepositoryListDataSource.GetGroupAsync(this.resourceLoader.GetString("PivotGroupIdRepositories"));
//                this.DefaultViewModel[FirstGroupName] = repositoryDataGroup;
//            }
//            catch
//            {

//            }
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="sender">イベントのソース (通常、<see cref="NavigationHelper"/>)。</param>
        /// <param name="e">シリアル化可能な状態で作成される空のディクショナリを提供するイベント データ
        ///。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: ページの一意の状態をここに保存します。
        }

        /// <summary>
        /// アプリ バー ボタンがクリックされたときに項目を一覧に追加します。
        /// </summary>
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = this.pivot.SelectedIndex == 0 ? FirstGroupName : SecondGroupName;
            var group = this.DefaultViewModel[groupName] as SampleDataGroup;
            var nextItemId = group.Items.Count + 1;
            var newItem = new SampleDataItem(
                string.Format(CultureInfo.InvariantCulture, "Group-{0}-Item-{1}", this.pivot.SelectedIndex + 1, nextItemId),
                string.Format(CultureInfo.CurrentCulture, this.resourceLoader.GetString("NewItemTitle"), nextItemId),
                string.Empty,
                string.Empty,
                this.resourceLoader.GetString("NewItemDescription"),
                string.Empty);

            group.Items.Add(newItem);

            // 新しい項目をスクロールして表示します。
            var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
            var listView = container.ContentTemplateRoot as ListView;
            listView.ScrollIntoView(newItem, ScrollIntoViewAlignment.Leading);
        }

        /// <summary>
        /// セクション内のアイテムがクリックされたときに呼び出されます。
        /// </summary>
        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 適切な移動先のページに移動し、新しいページを構成します。
            // このとき、必要な情報をナビゲーション パラメーターとして渡します
            RepositoryInfoNaviParam param = new RepositoryInfoNaviParam(((RepositoryListDataItem)e.ClickedItem).OwnerName,
                                                                        ((RepositoryListDataItem)e.ClickedItem).Name);
            //            Frame.Navigate(typeof(LoginContentDialog));
            if (!Frame.Navigate(typeof(RepositoryInfoPage), param))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// スクロールして表示するときに 2 番目のピボット項目のコンテンツを読み込みます。
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            var repositoryDataGroup = await RepositoryListDataSource.GetGroupAsync(this.resourceLoader.GetString("PivotGroupIdRepositories"));
            this.DefaultViewModel[SecondGroupName] = repositoryDataGroup;
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SignInContentDialog = new SignInContentDialog();
                SignInContentDialog.ShowAsync();
            }
            catch
            {

            }
            finally
            {}
            

        }


        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await SetRepositoryList();
        }

        private async Task SetRepositoryList()
        {
            try
            {
                connectProgress.Visibility = Visibility.Visible;
                var repositoryListDataGroup = await RepositoryListDataSource.GetGroupAsync(this.resourceLoader.GetString("PivotGroupIdRepositories"));
                this.DefaultViewModel[FirstGroupName] = repositoryListDataGroup;
            }
            catch
            {

            }
            finally
            {
                connectProgress.Visibility = Visibility.Collapsed;
            }

            return;
        }
        #region NavigationHelper の登録

        /// <summary>
        /// このセクションに示したメソッドは、NavigationHelper がページの
        /// ナビゲーション メソッドに応答できるようにするためにのみ使用します。
        /// <para>
        /// ページ固有のロジックは、
        /// <see cref="NavigationHelper.LoadState"/>
        /// および <see cref="NavigationHelper.SaveState"/>。
        /// LoadState メソッドでは、前のセッションで保存されたページの状態に加え、
        /// ナビゲーション パラメーターを使用できます。
        /// </para>
        /// </summary>
        /// <param name="e">ナビゲーション要求を取り消すことのできないナビゲーション メソッドおよびイベント
        /// ハンドラーにデータを提供します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
