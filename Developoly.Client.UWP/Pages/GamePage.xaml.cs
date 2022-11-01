using Developoly.Client.Entities;
using Developoly.Client.Services;
using Developoly.Client.Services.Interfaces;
using Developoly.Client.UWP.Pages.FrameResult;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Developoly.Client.UWP.Pages
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page, GamePageInterface
    {
        private Frame _rootFrame;

        private Project _projectSelected;

        public List<Dev> devSelected = new List<Dev>();

        private General _general;
        General GamePageInterface.General { get => _general; set => _general = value; }

        private Service _service;

        public GamePage() { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.InitializeComponent();
            this.DataContext = this;
            _general = (e.Parameter as List<Object>)[0] as General;
            _service = _general.Service as Service;
            _service.SetGamePageInterface(this, SynchronizationContext.Current);
            _rootFrame = (e.Parameter as List<Object>)[1] as Frame;
            InitializeView();
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


        private void InitializeView()
        {
            txtTurnOfCompany.Text = _general.MyCompany.Name.ToString();
            txtTurnOfCompany.Tag = _general.MyCompany;
            txtTurnOfCompanyMoney.Text = _general.MyCompany.Money.ToString();
            txtNumberOfTurn.Text = _general.NbTurnActual.ToString();
            txtNumberOfTurnMax.Text = _general.NbTurn.ToString();
            txtNumberOfPlayer.Text = _general.NbPlayerMax.ToString();
            listTheirCompany.ItemsSource = _general.TheirCompanys;
            CheckListEmptyCompany();
            listProjectAvailable.ItemsSource = _general.NewProjects;
            CheckListEmptyProject();
            listDevelopperAvailable.ItemsSource = _general.UnemployedDevs;
            CheckListEmptyDevelopper();
            listAddDev.ItemsSource = _general.MyCompany.Devs.Where(d => d.Course == null && d.Projet == null).ToList();
            CheckListEmptyAddDevelopper();
        }

        private void CheckListEmptyDevelopper()
        {
            if(listDevelopperAvailable.Items.Count == 0)
            {
                gridEmptyDevelopper.Visibility = Visibility.Visible;
            }
        }

        private void CheckListEmptyCompany()
        {
            if (listTheirCompany.Items.Count == 0) { 

                gridEmptyCompany.Visibility = Visibility.Visible;
            }
        }

        private void CheckListEmptyProject()
        {
            if (listProjectAvailable.Items.Count == 0)
            {
                gridEmptyProject.Visibility = Visibility.Visible;
            }
        }

        private void CheckListEmptyAddDevelopper()
        {
            if (listAddDev.Items.Count == 0)
            {
                gridEmptyAddDevelopper.Visibility = Visibility.Visible;
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


        async void GamePageInterface.InitializePlayerPlayed(Object company)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txtTurnOfCompany.Text = (company as Company).Name;
                txtTurnOfCompany.Tag = (company as Company);
                txtTurnOfCompanyMoney.Text = (company as Company).Money.ToString();
            });
        }



        public void MyTurnOver(string companyNext)
        { 
            Company company = JsonConvert.DeserializeObject<Company>(companyNext, _service.jsonSetting);
            _general.MyCompany = company;
            _service.SendData(new Communication("WhoNext", null));
        }

        public void HisTurnOver(string companyNext)
        {
            Company company = JsonConvert.DeserializeObject<Company>(companyNext, _service.jsonSetting);
            Company otherComp = _general.TheirCompanys.Where(c => c.Id == company.Id).First();
            otherComp.Devs = company.Devs;
            otherComp.Projects = company.Projects;
            otherComp.Money = company.Money;
            _service.SendData(new Communication("WhoNext", null));
        }





        private void LinkCompany_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            contentAddDev.Hide();
            _rootFrame.Navigate(typeof(CompanyPage), new List<Object>() { _general, _rootFrame, (sender as TextBlock).Tag as Company }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void LinkProject_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            contentAddDev.Hide();
            _rootFrame.Navigate(typeof(ProjectPage), new List<Object>() { _general, _rootFrame, (sender as TextBlock).Tag as Project }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void LinkDevelopper_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            contentAddDev.Hide();
            _rootFrame.Navigate(typeof(DevelopperPage), new List<Object>() { _general, _rootFrame, (sender as TextBlock).Tag as Dev }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void BtnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            _service.SendData(new Communication("NextTurn", JsonConvert.SerializeObject(_general.MyCompany, _service.jsonSetting)));
            _general.NbTurnActual += 1;
        }

        private async void AddProjectToCompany_Click(object sender, RoutedEventArgs e)
        {
            devSelected = new List<Dev>();
            _projectSelected = (sender as Button).Tag as Project;
            await contentAddDev.ShowAsync();
            listAddDev.SelectedItem = null;
            
        }

        private void contentAddDev_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (devSelected.Count != 0) { 
                _service.SendData(new Communication("AddProjectToCompany", JsonConvert.SerializeObject(new DevToProject(_projectSelected, devSelected), _service.jsonSetting)));
            }
        }


        private void AddDevToCompany_Click(object sender, RoutedEventArgs e)
        {
            _service.SendData(new Communication("AddDevToCompany", JsonConvert.SerializeObject((sender as Button).Tag as Dev, _service.jsonSetting)));
        }

        void GamePageInterface.MyDevelopperNotQualified(string skillMissing)
        {
            string skillMiss = "";
            JsonConvert.DeserializeObject<List<Skill>>(skillMissing).ForEach(s =>
            {
                skillMiss += s.Name + ", ";
            });
            _rootFrame.Navigate(typeof(ErrorProject), new List<Object>() { _rootFrame, _general, skillMiss }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        void GamePageInterface.MyDevAddNotEnoughMoney(string dev)
        {
            _rootFrame.Navigate(typeof(ErrorDevMoney), new List<Object>() { _rootFrame, _general, JsonConvert.DeserializeObject<Dev>(dev), _general.MyCompany.Money }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

        }


        void GamePageInterface.MyDevAddSuccess(string company)
        {
            Company companyHiredDev = JsonConvert.DeserializeObject<Company>(company, _service.jsonSetting);
            _general.MyCompany = companyHiredDev;
            _general.MyCompany.Devs.ForEach(d => d.Company = companyHiredDev);
            _general.MyCompany.Projects.ForEach(p => p.Company = companyHiredDev);
            _general.UnemployedDevs.Remove(_general.UnemployedDevs.Where(u => u.Id == _general.MyCompany.Devs.Last().Id).First());

            if(gridEmptyAddDevelopper.Visibility == Visibility.Visible)
            {
                gridEmptyAddDevelopper.Visibility = Visibility.Collapsed;
            }

            if (_general.UnemployedDevs.Count == 0)
            {
                gridEmptyDevelopper.Visibility = Visibility.Visible;
            }
            listDevelopperAvailable.ItemsSource = null;
            listDevelopperAvailable.ItemsSource = _general.UnemployedDevs;
            _rootFrame.Navigate(typeof(DevAjout), new List<Object>() { _rootFrame, _general, _general.MyCompany.Devs.Last() }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

        }

        void GamePageInterface.MyProjectAddSuccess(string company)
        {
            Company companyProjectSelected = JsonConvert.DeserializeObject<Company>(company, _service.jsonSetting);
            _general.MyCompany = companyProjectSelected;
            _general.MyCompany.Projects.Last().Company = companyProjectSelected;
            _general.MyCompany.Projects.ForEach(p => p.Company = companyProjectSelected);
            _general.MyCompany.Devs.ForEach(d => d.Company = companyProjectSelected);
            string projectDev = "";
            _general.MyCompany.Projects.Last().Devs.ForEach(d => {
                d.Company = companyProjectSelected;
                d.Projet = _general.MyCompany.Projects.Last();
                _general.MyCompany.Devs.Where(dev => dev.Id == d.Id).FirstOrDefault().Projet = _general.MyCompany.Projects.Last();
                projectDev += d.Name + ", ";
            });
            _general.NewProjects.Remove(_general.NewProjects.Where(u => u.Id == _general.MyCompany.Projects.Last().Id).First());
            if (_general.NewProjects.Count == 0)
            {
                gridEmptyProject.Visibility = Visibility.Visible;
            }
            listProjectAvailable.ItemsSource = null;
            listProjectAvailable.ItemsSource = _general.NewProjects;
            _rootFrame.Navigate(typeof(ProjectAjout), new List<Object>() { _rootFrame, _general,_general.MyCompany.Projects.Last(), projectDev }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }


        void GamePageInterface.HisDevAddSuccess(string company)
        {
            Company companyHiredDev = JsonConvert.DeserializeObject<Company>(company, _service.jsonSetting);
            _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First().Money = companyHiredDev.Money;
            _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First().Devs = companyHiredDev.Devs;
            _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First().Projects = companyHiredDev.Projects;
            _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First().Devs.ForEach(d => d.Company = companyHiredDev);
            _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First().Projects.ForEach(p => p.Company = companyHiredDev);
            _general.UnemployedDevs.Remove(_general.UnemployedDevs.Where(u => u.Id == companyHiredDev.Devs.Last().Id).First());
            companyHiredDev.Devs.Last().Company = companyHiredDev;
            listDevelopperAvailable.ItemsSource = null;
            listDevelopperAvailable.ItemsSource = _general.UnemployedDevs;
            
            if (_general.UnemployedDevs.Count == 0)
            {
                gridEmptyDevelopper.Visibility = Visibility.Visible;
            }

            listTheirCompany.ItemsSource = null;
            listTheirCompany.ItemsSource = _general.TheirCompanys;
            _rootFrame.Navigate(typeof(DevHisAjout), new List<Object>() { _rootFrame, _general, companyHiredDev.Devs.Last(), _general.TheirCompanys.Where(t => t.Id == companyHiredDev.Id).First() }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });    
        }

        void GamePageInterface.HisProjectAddSuccess(string company)
        {

            Company companyProjectSelected = JsonConvert.DeserializeObject<Company>(company, _service.jsonSetting);
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Money = companyProjectSelected.Money;
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Devs = companyProjectSelected.Devs;
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects = companyProjectSelected.Projects;
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Devs.ForEach(d => d.Company = companyProjectSelected);
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects.ForEach(p => p.Company = companyProjectSelected);
            _general.NewProjects.Remove(_general.NewProjects.Where(u => u.Id == companyProjectSelected.Projects.Last().Id).First());
            companyProjectSelected.Projects.Last().Company = companyProjectSelected;
            string projectDev = "";
            _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects.Last().Devs.ForEach(d => {
                d.Company = _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First();
                d.Projet = _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects.Last();
                _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Devs.Where(dev => dev.Id == d.Id).FirstOrDefault().Projet = _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects.Last();
                projectDev += d.Name + ", ";
            });

            if (_general.NewProjects.Count == 0)
            {
                gridEmptyProject.Visibility = Visibility.Visible;
            }
            listProjectAvailable.ItemsSource = null;
            listProjectAvailable.ItemsSource = _general.NewProjects;
            listTheirCompany.ItemsSource = null;
            listTheirCompany.ItemsSource = _general.TheirCompanys;
            _rootFrame.Navigate(typeof(ProjectHisAjout), new List<Object>() { _rootFrame, _general, _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First().Projects.Last(), projectDev, _general.TheirCompanys.Where(t => t.Id == companyProjectSelected.Id).First() }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

        }


        void GamePageInterface.HisDevToCourseSuccess(string devAndCourse)
        {

            DevToCourse devToCourse = JsonConvert.DeserializeObject<DevToCourse>(devAndCourse, _service.jsonSetting);
            Company company = _general.TheirCompanys.Where(c => c.Id == devToCourse.Dev.Company.Id).First();
            company.Devs.Where(d => d.Id == devToCourse.Dev.Id).FirstOrDefault().Course = devToCourse.Course;
            _general.Schools.Where(s => s.Id == devToCourse.Course.School.Id).FirstOrDefault().Courses.Where(c => c.Id == devToCourse.Course.Id).FirstOrDefault().Dev = devToCourse.Dev;
            _rootFrame.Navigate(typeof(CourseHisAddDev), new List<Object>() { _rootFrame, _general, devToCourse, company }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }


        private void contentAddDev_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender.Hide();
        }

        private void listAddDev_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!devSelected.Contains(e.ClickedItem as Dev))
            {
                devSelected.Add(e.ClickedItem as Dev);
            } else
            {
                devSelected.Remove(e.ClickedItem as Dev);
            }
        }

        void GamePageInterface.PlayerToPlay(string idCompany)
        {
            if (_general.MyCompany.Id == Convert.ToInt32(idCompany)) 
            {
                PlayerPlayed(idCompany);
                txtTurnGrid.Text = "It's your turn !!";
                StartTimer();
            } else
            {
                PlayerNotPlayed(idCompany);
                txtTurnGrid.Text = "It's NOT your turn !!";
                StartTimer();
            }
        }

        private void PlayerPlayed(string idCompany)
        {
            _general.Playable = true;
            _general.IdPlayerActual = Convert.ToInt32(idCompany);
            listAddDev.IsEnabled = true;
            listDevelopperAvailable.IsEnabled = true;
            listProjectAvailable.IsEnabled = true;
            BtnNextTurn.IsEnabled = true;
        }

        private void PlayerNotPlayed(string idCompany)
        {
            _general.Playable = false;
            _general.IdPlayerActual = Convert.ToInt32(idCompany);
            listAddDev.IsEnabled = false;
            listDevelopperAvailable.IsEnabled = false;
            listProjectAvailable.IsEnabled = false;
            BtnNextTurn.IsEnabled = false;
            Company actualPlayed = _general.TheirCompanys.Where(c => c.Id == Convert.ToInt32(idCompany)).First();
            txtTurnOfCompany.Text = actualPlayed.Name.ToString();
            txtTurnOfCompany.Tag = actualPlayed;
            txtTurnOfCompanyMoney.Text = actualPlayed.Money.ToString();
        }

        private void StartTimer()
        {
            gridYourTurn.Visibility = Visibility.Visible;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (RadialProgressBarTurn.Value + 1 < 4)
            {
                RadialProgressBarTurn.Value = RadialProgressBarTurn.Value + 1;
                txtTimer.Text = (Convert.ToInt16(txtTimer.Text) - 1).ToString();
            }
            else
            {
                gridYourTurn.Visibility = Visibility.Collapsed;
                (sender as DispatcherTimer).Tick -= dispatcherTimer_Tick;
            }
        }

    }
}
