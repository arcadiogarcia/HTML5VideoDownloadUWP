using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HTML5VideoDownloadUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectList : Page
    {
        public SelectList()
        {
            this.InitializeComponent();
            AppLogic.currentPage = this;
            this.SelectOptions = new ObservableCollection<string>();
        }

        private ObservableCollection<string> SelectOptions;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation imageAnimation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("message");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(this.statePanel);
            }
            foreach (var x in (e.Parameter as string[]))
            {
                this.SelectOptions.Add(x);
            }
        }

        internal void setMessage(string v)
        {
            this.activityTitle.Text = v;
        }

        internal void setList(string[] v)
        {
            while (this.SelectOptions.Count > 0) { 
                this.SelectOptions.RemoveAt(0);
            }
            foreach (var x in v)
            {
                this.SelectOptions.Add(x);
            }
        }

        private void devicesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            AppLogic.SelectOption(e.ClickedItem as String);
        }
    }
}
