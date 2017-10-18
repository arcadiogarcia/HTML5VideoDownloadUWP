using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HTML5VideoDownloadUWP
{
    class AppLogic
    {
        static public Page currentPage;
        private enum AppState { analyzing, selectingURL, selectingDevice, opening };
        static private AppState currentState = AppState.analyzing;
        static private string currentURL = null;
        static public void Analyze(Uri uri)
        {
            currentState = AppState.analyzing;
            (currentPage as WebpageAnalyzer).setMessage("Scanning the webpage for videos");

            var webView = new WebView();
            webView.NavigationCompleted += WebViewTargetPageLoadedAsync;
            webView.Navigate(uri);
        }

        static private async void WebViewTargetPageLoadedAsync(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (currentState == AppState.analyzing && args.IsSuccess)
            {
                string jsResult = await sender.InvokeScriptAsync("eval", new string[] { "Array.from(document.getElementsByTagName('video')).map(x=>x.src).join('>')" });
                sender.NavigateToString("");
                var urls = jsResult.Split('>').Where(x => x != "").ToArray();
                if (urls.Length > 0)
                {
                    (currentPage as WebpageAnalyzer).setMessage("Select the desired video");
                    (currentPage as WebpageAnalyzer).navigateToSelectList(urls);
                    currentState = AppState.selectingURL;
                }
                else
                {
                    (currentPage as WebpageAnalyzer).setMessage("No videos have been found");
                }
            }
            else
            {
                if (currentPage is WebpageAnalyzer)
                {
                    (currentPage as WebpageAnalyzer).setMessage("The page can't be loaded");
                }
            }
        }

        private static string[] startingDevices = new String[] { "This device" };

        internal static async void SelectOption(string text)
        {
            switch (currentState)
            {
                case AppState.selectingURL:
                    currentURL = text;
                    RemoteSystemAccessStatus accessStatus = await RemoteSystem.RequestAccessAsync();
                    if (accessStatus == RemoteSystemAccessStatus.Allowed)
                    {
                        var remoteSystemWatcher = RemoteSystem.CreateWatcher();
                        remoteSystemWatcher.RemoteSystemAdded += RemoteSystemWatcher_RemoteSystemAdded;
                        remoteSystemWatcher.RemoteSystemRemoved += RemoteSystemWatcher_RemoteSystemRemoved;
                        remoteSystemWatcher.RemoteSystemUpdated += RemoteSystemWatcher_RemoteSystemUpdated;
                        remoteSystemWatcher.Start();
                        (currentPage as SelectList).setMessage("Finding devices");
                        (currentPage as SelectList).setList(startingDevices);
                        currentState = AppState.selectingDevice;
                    }
                    else
                    {
                        (currentPage as SelectList).setMessage("Select a device");
                        (currentPage as SelectList).setList(startingDevices);
                        currentState = AppState.selectingDevice;
                    }
                    break;
                case AppState.selectingDevice:
                    var downloadUrl = new Uri("https://arcadiogarcia.github.io/HTML5VideoDownloadUWP/?" + Uri.EscapeDataString(currentURL)) ;
                    if (startingDevices.Contains(text))
                    {
                        //Asuming it never fails
                        await Windows.System.Launcher.LaunchUriAsync(downloadUrl);
                        Application.Current.Exit();
                    }
                    else
                    {
                        var launchUriStatus = await RemoteLauncher.LaunchUriAsync(new RemoteSystemConnectionRequest(remoteSystems.Find(x => x.DisplayName == text)), downloadUrl);
                        if (launchUriStatus  == RemoteLaunchUriStatus.Success)
                        {
                            Application.Current.Exit();
                        }
                        else
                        {
                            (currentPage as SelectList).setMessage(launchUriStatus.ToString());
                        }
                    }
                    break;
            }
        }

        static List<RemoteSystem> remoteSystems = new List<RemoteSystem>();

        private static void RemoteSystemWatcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
        {
            remoteSystems[remoteSystems.FindIndex(x => x.Id == args.RemoteSystem.Id)] = args.RemoteSystem;
            updateSystems();
        }

        private static void RemoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            remoteSystems.RemoveAll(x => x.Id == args.RemoteSystemId);
            updateSystems();
        }

        private static void RemoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            remoteSystems.Add(args.RemoteSystem);
            updateSystems();
        }

        private static void updateSystems()
        {
            CoreApplication.Views.First().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                (currentPage as SelectList).setMessage("Select a device");
                (currentPage as SelectList).setList(startingDevices.Concat(remoteSystems.Select(x => x.DisplayName)).ToArray());
            });
        }
    }
}
