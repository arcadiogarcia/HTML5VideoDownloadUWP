using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HTML5VideoDownloadUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebpageAnalyzer : Page
    {
        public WebpageAnalyzer()
        {
            AppLogic.currentPage = this;
            this.InitializeComponent();
        }

        internal void setMessage(string v)
        {
            this.activityTitle.Text = v;
        }

        internal void navigateToSelectList()
        {

        }

        internal void navigateToSelectList(string[] urls)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("message", this.statePanel);
            Frame.Navigate(typeof(SelectList),urls);
        }
    }
}