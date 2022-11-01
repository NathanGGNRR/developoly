using Developoly.Client.Entities;
using System;
using System.Collections.Generic;
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

namespace Developoly.Client.UWP.Pages.FrameResult
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevHisAjout : Page
    {

        private Frame _rootFrame;

        private Dev _dev;
        public Dev Dev { get => _dev; set => _dev = value; }

        private Company _company;
        public Company Company { get => _company; set => _company = value; }


        private General _general;
        public General General { get => _general; set => _general = value; }

        public DevHisAjout() { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.InitializeComponent();
            this.DataContext = this;
            _rootFrame = (e.Parameter as List<Object>)[0] as Frame;
            General = (e.Parameter as List<Object>)[1] as General;
            Dev = (e.Parameter as List<Object>)[2] as Dev;
            Company = (e.Parameter as List<Object>)[3] as Company;
            InitializeDev();
        }

        private void InitializeDev()
        {
            txtNameOfCompany.Text = Company.Name;
            txtNameOfDev.Text = Dev.Name;
            txtSalary.Text = Dev.Salary.ToString();
            txtHiring.Text = Dev.HiringCost.ToString();
            StartTimer();
        }

        private void StartTimer()
        {

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (RadialProgressBarReady.Value + 1 < 3)
            {
                RadialProgressBarReady.Value = RadialProgressBarReady.Value + 1;
                txtTimer.Text = (Convert.ToInt16(txtTimer.Text) - 1).ToString();
            }
            else
            {
                try
                {
                    (sender as DispatcherTimer).Tick -= dispatcherTimer_Tick;
                    _rootFrame.Navigate(typeof(GamePage), new List<Object>() { General, _rootFrame }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                }
                catch { }
            }
        }
    }
}
