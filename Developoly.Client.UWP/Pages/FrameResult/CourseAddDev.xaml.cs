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
    public sealed partial class CourseAddDev : Page
    {

        private Frame _rootFrame;

        private DevToCourse _devToCourse;
        public DevToCourse DevToCourse { get => _devToCourse; set => _devToCourse = value; }
       

        private General _general;
        public General General { get => _general; set => _general = value; }

        public CourseAddDev() { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.InitializeComponent();
            this.DataContext = this;
            _rootFrame = (e.Parameter as List<Object>)[0] as Frame;
            General = (e.Parameter as List<Object>)[1] as General;
            DevToCourse = (e.Parameter as List<Object>)[2] as DevToCourse;
            InitializeCourse();
        }

        private void InitializeCourse()
        {
            txtNameOfDev.Text = DevToCourse.Dev.Name;
            txtNameOfSchool.Text = DevToCourse.Course.School.Name;
            txtNameSkill.Text = DevToCourse.Course.Skill.Name;
            txtLevelSkill.Text = DevToCourse.Course.Skill.Level.ToString();
            txtTurn.Text = DevToCourse.Course.Duration.ToString();
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
            if (RadialProgressBarReady.Value + 1 < 4)
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
                } catch { }
            }
        }
    }
}
