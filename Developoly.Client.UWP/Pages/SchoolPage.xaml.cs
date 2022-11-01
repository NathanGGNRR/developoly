using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Developoly.Client.Entities;
using Developoly.Client.Services;
using Developoly.Client.Services.Interfaces;
using System.Threading;
using Newtonsoft.Json;
using Developoly.Client.UWP.Pages.FrameResult;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Developoly.Client.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SchoolPage : Page, SchoolPageInterface
    {
        private Frame _rootFrame;

        private School _school;

        public Dev devSelected = null;

        private Course _courseSelected;


        private General _general;
        General SchoolPageInterface.General { get => _general; set => _general = value; }

        private Service _service;

        public SchoolPage() { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.InitializeComponent();
            _general = (e.Parameter as List<Object>)[0] as General;
            _service = _general.Service as Service;
            _service.SetSchoolPageInterface(this, SynchronizationContext.Current);
            _rootFrame = (e.Parameter as List<Object>)[1] as Frame;
            _school = (e.Parameter as List<Object>)[2] as School;
            InitializeSchool();
            if (_general.IdPlayerActual != 0)
            {
                if (_general.Playable)
                {
                    PlayerPlayed(_general.IdPlayerActual.ToString());
                }
                else
                {
                    PlayerNotPlayed(_general.IdPlayerActual.ToString());
                }
            }
        }

        private void InitializeSchool()
        {
            txtNameSchool.Text = _school.Name;

            List<Dev> devsSchool = new List<Dev>();
            _general.Schools.Where(s => s.Id == _school.Id).FirstOrDefault().Courses.ForEach(c =>
            {
                if (c.Dev != null)
                {
                    devsSchool.Add(c.Dev);
                }
            });
            listDevelopperSchool.ItemsSource = devsSchool;
            CheckListEmptyDevelopper();
            List<Course> courses = _school.Courses.Where(c => c.Dev == null).ToList();
            courses.ForEach(c => c.School = _school);
            listCoursesSchool.ItemsSource = courses;
            CheckListEmptyCourseAffected();
            List<Dev> devsAvailable = _general.MyCompany.Devs.Where(d => d.Course == null & d.Projet == null).ToList(); ;
            listAddDevCourses.ItemsSource = devsAvailable;
            CheckListEmptyAddDevCourse();
        }


        private void CheckListEmptyDevelopper()
        {
            if (listDevelopperSchool.Items.Count == 0)
            {
                gridEmptyDevelopper.Visibility = Visibility.Visible;
            }
        }

        private void CheckListEmptyCourseAffected()
        {
            if (listCoursesSchool.Items.Count == 0)
            {
                gridEmptyCourses.Visibility = Visibility.Visible;
            }
        }

        private void CheckListEmptyAddDevCourse()
        {
            if (listAddDevCourses.Items.Count == 0)
            {
                gridEmptyAddDevCourses.Visibility = Visibility.Visible;
            }
        }


        private void Link_PointerOver(object sender, PointerRoutedEventArgs e)
        {
            (sender as TextBlock).Foreground = new SolidColorBrush((Color)Application.Current.Resources["CadetBlue"]);
        }

        private void Link_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as TextBlock).Foreground = new SolidColorBrush((Color)Application.Current.Resources["PayneGrey"]);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await contentAddDev.ShowAsync();
        }


        private void contentAddDev_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (devSelected != null)
            {
                _service.SendData(new Communication("AddDevToCourse", JsonConvert.SerializeObject(new DevToCourse(_courseSelected, devSelected), _service.jsonSetting)));
            }
        }

        private void LinkDevelopper_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _rootFrame.Navigate(typeof(DevelopperPage), new List<Object>() { _general, _rootFrame, (sender as TextBlock).Tag as Dev }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }


        private void contentAddDev_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender.Hide();
        }


        void SchoolPageInterface.MyDevToCourseSuccess(string devAndCourse)
        {

            DevToCourse devToCourse = JsonConvert.DeserializeObject<DevToCourse>(devAndCourse, _service.jsonSetting);
            _general.MyCompany.Devs.Where(d => d.Id == devToCourse.Dev.Id).FirstOrDefault().Course = devToCourse.Course;
            _general.Schools.Where(s => s.Id == devToCourse.Course.School.Id).FirstOrDefault().Courses.Where(c => c.Id == devToCourse.Course.Id).FirstOrDefault().Dev = devToCourse.Dev;
            if (_general.Schools.Where(s => s.Id == devToCourse.Course.School.Id).FirstOrDefault().Courses.Where(cd => cd.Dev == null).ToList().Count == 0)
            {
                gridEmptyCourses.Visibility = Visibility.Visible;
            }
            listCoursesSchool.ItemsSource = null;
            listCoursesSchool.ItemsSource = _general.Schools.Where(s => s.Id == _school.Id).FirstOrDefault().Courses.Where(cd => cd.Dev == null);
            _rootFrame.Navigate(typeof(CourseAddDev), new List<Object>() { _rootFrame, _general, devToCourse }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        void SchoolPageInterface.HisDevToCourseSuccess(string devAndCourse)
        {

            DevToCourse devToCourse = JsonConvert.DeserializeObject<DevToCourse>(devAndCourse, _service.jsonSetting);
            Company company = _general.TheirCompanys.Where(c => c.Id == devToCourse.Dev.Company.Id).First();
            company.Devs.Where(d => d.Id == devToCourse.Dev.Id).FirstOrDefault().Course = devToCourse.Course;
            _general.Schools.Where(s => s.Id == devToCourse.Course.School.Id).FirstOrDefault().Courses.Where(c => c.Id == devToCourse.Course.Id).FirstOrDefault().Dev = devToCourse.Dev;
            if (_general.Schools.Where(s => s.Id == devToCourse.Course.School.Id).FirstOrDefault().Courses.Where(cd => cd.Dev == null).ToList().Count == 0)
            {
                gridEmptyCourses.Visibility = Visibility.Visible;
            }
            listCoursesSchool.ItemsSource = null;
            listCoursesSchool.ItemsSource = _general.Schools.Where(s => s.Id == _school.Id).FirstOrDefault().Courses.Where(cd => cd.Dev == null);
            _rootFrame.Navigate(typeof(CourseHisAddDev), new List<Object>() { _rootFrame, _general, devToCourse, company }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void AddDevToCompany_Click(object sender, RoutedEventArgs e)
        {
            _courseSelected = (sender as Button).Tag as Course;
            await contentAddDev.ShowAsync();
            listAddDevCourses.SelectedItem = null;
        }

        private void listAddDevCourses_ItemClick(object sender, ItemClickEventArgs e)
        {
            devSelected = (e.ClickedItem as Dev);
        }

        private void PlayerPlayed(string idCompany)
        {
            _general.Playable = true;
            _general.IdPlayerActual = Convert.ToInt32(idCompany);
            listCoursesSchool.IsEnabled = true;

        }

        private void PlayerNotPlayed(string idCompany)
        {
            _general.Playable = false;
            _general.IdPlayerActual = Convert.ToInt32(idCompany);
            listCoursesSchool.IsEnabled = false;
        }
    }
}
