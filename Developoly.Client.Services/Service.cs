using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Developoly.Client.Entities;
using System.Threading;
using Developoly.Client.Services.Interfaces;

namespace Developoly.Client.Services
{
    public partial class Service
    {

        private SynchronizationContext _contextCompany;
        public SynchronizationContext ContextCompany { get => _contextCompany; set => _contextCompany = value; }

        private CompanyPageInterface _companyPage;
        public CompanyPageInterface CompanyPage { get => _companyPage; set => _companyPage = value; }

        public void SetCompanyPageInterface(CompanyPageInterface companyInterface, SynchronizationContext context)
        {
            if (!(CompanyPage is CompanyPageInterface))
            {
                CompanyPage = companyInterface;
            }
            ContextCompany = context;
        }

        private SynchronizationContext _contextDevelopper;
        public SynchronizationContext ContextDevelopper { get => _contextDevelopper; set => _contextDevelopper = value; }

        private DevelopperPageInterface _developperPage;
        public DevelopperPageInterface DevelopperPage { get => _developperPage; set => _developperPage = value; }

        public void SetDevelopperPageInterface(DevelopperPageInterface developperInterface, SynchronizationContext context)
        {
            if (!(DevelopperPage is DevelopperPageInterface))
            {
                DevelopperPage = developperInterface;       
            }
            ContextDevelopper = context;
        }

        private SynchronizationContext _contextGamePage;
        public SynchronizationContext ContextGamePage { get => _contextGamePage; set => _contextGamePage = value; }

        private GamePageInterface _gamePage;
        public GamePageInterface GamePage { get => _gamePage; set => _gamePage = value; }

        public void SetGamePageInterface(GamePageInterface gamePageInterface, SynchronizationContext context)
        {
            if (!(GamePage is GamePageInterface))
            {
                GamePage = gamePageInterface;               
            }
            ContextGamePage = context;
        }

        private SynchronizationContext _contextListSchoolPage;
        public SynchronizationContext ContextListSchoolPage { get => _contextListSchoolPage; set => _contextListSchoolPage = value; }

        private ListSchoolPageInterface _listSchoolPage;
        public ListSchoolPageInterface ListSchoolPage { get => _listSchoolPage; set => _listSchoolPage = value; }

        public void SetListSchoolPageInterface(ListSchoolPageInterface listSchoolPageInterface, SynchronizationContext context)
        {
            if (!(ListSchoolPage is ListSchoolPageInterface))
            {
                ListSchoolPage = listSchoolPageInterface;
            }
            ContextListSchoolPage = context;
        }

        private SynchronizationContext _contextMainPage;
        public SynchronizationContext ContextMainPage { get => _contextMainPage; set => _contextMainPage = value; }

        private MainPageInterface _mainPage;
        public MainPageInterface MainPage { get => _mainPage; set => _mainPage = value; }

        public void SetMainPageInterface(MainPageInterface mainPageInterface, SynchronizationContext context)
        {
            if (!(MainPage is MainPageInterface))
            {
                MainPage = mainPageInterface;
            }
            ContextMainPage = context;
        }

        private SynchronizationContext _contextMenuPage;
        public SynchronizationContext ContextMenuPage { get => _contextMenuPage; set => _contextMenuPage = value; }

        private MenuPageInterface _menuPage;
        public MenuPageInterface MenuPage { get => _menuPage; set => _menuPage = value; }

        public void SetMenuPageInterface(MenuPageInterface menuPageInterface, SynchronizationContext context)
        {
            if (!(MenuPage is MenuPageInterface))
            {
                MenuPage = menuPageInterface;
            }
            ContextMenuPage = context;
        }

        private SynchronizationContext _contextProjectPage;
        public SynchronizationContext ContextProjectPage { get => _contextProjectPage; set => _contextProjectPage = value; }

        private ProjectPageInterface _projectPage;
        public ProjectPageInterface ProjectPage { get => _projectPage; set => _projectPage = value; }

        public void SetProjectPageInterface(ProjectPageInterface projectPageInterface, SynchronizationContext context)
        {
            if (!(ProjectPage is ProjectPageInterface))
            {
                ProjectPage = projectPageInterface;                
            }
            ContextProjectPage = context;
        }

        private SynchronizationContext _contextSchoolPage;
        public SynchronizationContext ContextSchoolPage { get => _contextSchoolPage; set => _contextSchoolPage = value; }

        private SchoolPageInterface _schoolPage;
        public SchoolPageInterface SchoolPage { get => _schoolPage; set => _schoolPage = value; }

        public void SetSchoolPageInterface(SchoolPageInterface schoolPageInterface, SynchronizationContext context)
        {
            if (!(SchoolPage is SchoolPageInterface))
            {
                SchoolPage = schoolPageInterface;     
            }
            ContextSchoolPage = context;
        }



        private TcpClient _client;
        private NetworkStream _stream;

        private bool _isConnected;
        private int _byteSize = 1024 * 1024;

        public JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore

        };


        public Service() { }

        public Service(string ip, int port, MainPageInterface mainPage)
        {
            _mainPage = mainPage;
            _client = new TcpClient();
            try
            {
                _client.Connect(ip, port);
                _stream = _client.GetStream();
                _isConnected = true;
                Thread threadClientListen = new Thread(Listen);
                threadClientListen.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        public void Listen()
        {
            while (true)
            {
                Communication info = this.ReceiveData();
                if (info != null)
                {
                    this.ReceiveInfo(info);
                }
            }
        }


        public Communication ReceiveData()
        {
            try
            {
                byte[] inStream = new byte[_byteSize];
                int sizeBuffer = _stream.Read(inStream, 0, inStream.Length);
                try
                {
                    if (sizeBuffer > 0)
                    {
                        string commu = Encoding.Unicode.GetString(inStream);
                        return JsonConvert.DeserializeObject<Communication>(Encoding.Unicode.GetString(inStream), jsonSetting);
                    }
                }  catch (Exception e)
                {
                    Console.WriteLine("DeserializeQuiBeug. Exception: " + e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The server is not started. Exception: " + e);
            }
            return null;
        }

        public void SendData(Communication info)
        {
            if (_isConnected)
            {
                byte[] outStream = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(info, jsonSetting));
                _stream.Write(outStream, 0, outStream.Length);
                _stream.Flush();
            }
        }

        public void ReceiveInfo(Communication info)
        {
            switch (info.Action)
            {
                case "MyCompanyAddSuccess":
                    MyCompanyAddSuccess(info);
                    break;

                case "AddNewPlayerSuccess":
                    MainPage.ChangeLoading(info.Data);
                    break;

                case "RemovePlayerSuccess":
                    MainPage.ChangeLoading(info.Data);
                    break;

                case "MyCompanyAlreadyExist":
                    MainPage.MyCompanyAlreadyExist();
                    break;

                case "AddTheirCompany":
                    TheirCompanysAddSuccess(info);
                    break;

                case "MyDevAddSuccess":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.MyDevAddSuccess(info.Data); }, null); });
                    }

                    break;

                case "HisDevAddSuccess":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.HisDevAddSuccess(info.Data); }, null); });
                    }
                    break;

                case "MyDevAddNotEnoughMoney":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.MyDevAddNotEnoughMoney(info.Data); }, null); });
                    }
                    break;

                case "MyDevelopperNotQualified":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.MyDevelopperNotQualified(info.Data); }, null); });
                    }
                    break;

                case "MyProjectAddSuccess":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.MyProjectAddSuccess(info.Data); }, null); });
                    }
                    break;

                case "HisProjectAddSuccess":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.HisProjectAddSuccess(info.Data); }, null); });
                    }
                    break;

                case "MyDevToCourseSuccess":
                    if (SchoolPage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextSchoolPage.Post(delegate { SchoolPage.MyDevToCourseSuccess(info.Data); }, null); });
                    }
                    break;

                case "HisDevToCourseSuccess":
                    if (SchoolPage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextSchoolPage.Post(delegate { SchoolPage.HisDevToCourseSuccess(info.Data); }, null); });
                    }
                    else
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.HisDevToCourseSuccess(info.Data); }, null); });
                    }
                    break;

                case "SetupGame":
                    if (MainPage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextMainPage.Post(delegate { MainPage.SetupGame(info.Data); }, null); });
                    }
                    break;

                case "WhoStart":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.PlayerToPlay(info.Data); }, null); });
                    }
                    break;

                case "WhoNext":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.PlayerToPlay(info.Data); }, null); });
                    }
                    break;

                case "MyNextTurn":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.MyTurnOver(info.Data); }, null); });
                    }
                    break;

                case "HisNextTurn":
                    if (GamePage != null)
                    {
                        ThreadPool.QueueUserWorkItem(delegate { ContextGamePage.Post(delegate { GamePage.HisTurnOver(info.Data); }, null); });
                    }
                    break;

                case "Win":
                    MenuPage.WinGame();
                    break;

                case "ACompanyLost":
                    MenuPage.ACompanyLost();
                    break;

                case "GameOver":
                    MenuPage.GameOver();
                    break;

                default:
                    break;
            }

            

        }



        private void MyCompanyAddSuccess(Communication info)
        {
            MainPage.General = JsonConvert.DeserializeObject<General>(info.Data, jsonSetting);
            MainPage.LoadingScreen();
        }
        
        private void TheirCompanysAddSuccess(Communication info)
        {
            (JsonConvert.DeserializeObject<List<Company>>(info.Data, jsonSetting)).ForEach(c =>
            {
                MainPage.General.TheirCompanys.Add(c);
            });
        }



    


    }
}
