﻿
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Developoly.Server.Services.Entities;
using Developoly.Server.Entities;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Developoly.Server.Services
{
    
    public partial class Service
    {

        #region BasicValue

        #region GeneralParameters

        private int _startNumberDev = 0;
        private int _numberTurn = 10;
        private int _numberPlayer = 2;

        #endregion
        
        #region MoneyParameters

        private int _money = 40000;
        private int _salary = 1500;
        private int _hiringCost = 3000;
        private int _priceCourse = 200;
        private int _rewardProject = 2000;
        private int _multiHiring = 2;
        private int _multiSalary = 2;
        private int _multiPriceCourse = 2;
        private int _winParameter = 50000;
        private int _looseParameter = -5000;

        #endregion

        #region Quantity

        private int _quantityNumberMinimumCourse = 2;
        private int _quantityNumberMaximumCourse = 5;
        private int _durationMinimumTurnProject = 1;
        private int _durationMaximumTurnProject = 4;

        #endregion

        #region LevelDifficulty

        private int _skillDevLevel = 30;
        private int _skillProjectLevel = 10;

        #endregion

        List<int> keysList;

        #region Accesseurs

        public int StartNumberDev { get => _startNumberDev; set => _startNumberDev = value; } //A GERER


        public int NumberTurn { get => _numberTurn; set => _numberTurn = value; } //A GERER
        public int NumberPlayer { get => _numberPlayer; set => _numberPlayer = value; } //A GERER


        public int Money { get => _money; set => _money = value; }
        public int Salary { get => _salary; set => _salary = value; }
        public int HiringCost { get => _hiringCost; set => _hiringCost = value; }
        public int PriceCourse { get => _priceCourse; set => _priceCourse = value; }
        public int RewardProject { get => _rewardProject; set => _rewardProject = value; }
        public int QuantityNumberMinimumCourse { get => _quantityNumberMinimumCourse; set => _quantityNumberMinimumCourse = value; }
        public int QuantityNumberMaximumCourse { get => _quantityNumberMaximumCourse; set => _quantityNumberMaximumCourse = value; }
        public int DurationMaximumTurnProject { get => _durationMaximumTurnProject; set => _durationMaximumTurnProject = value; }
        public int DurationMinimumTurnProject { get => _durationMinimumTurnProject; set => _durationMinimumTurnProject = value; }
        public int SkillDevLevel { get => _skillDevLevel; set => _skillDevLevel = value; }
        public int SkillProjectLevel { get => _skillProjectLevel; set => _skillProjectLevel = value; }
        public int MultiHiring { get => _multiHiring; set => _multiHiring = value; }
        public int MultiSalary { get => _multiSalary; set => _multiSalary = value; }
        public int MultiPriceCourse { get => _multiPriceCourse; set => _multiPriceCourse = value; }
        public int WinParameter { get => _winParameter; set => _winParameter = value; }
        public int LooseParameter { get => _looseParameter; set => _looseParameter = value; }

        #endregion


        #endregion

        Random random;
        public JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore

        };

        int actualIdClient;
        int nextClient;
        int turnActual = 1;

        private Dictionary<int, TcpClient> clients;
        public Dictionary<int, TcpClient> Clients { get => clients; set => clients = value; }

        private General startGeneral;
        public General StartGeneral { get => startGeneral; set => startGeneral = value; }

        #region ServiceStructure
        private List<Dev> devs;
        private List<Course> courses;
        private ObservableCollection<Company> companiesObservable;
        private List<Company> companies;
        private List<Project> projects;
        private List<School> schools;
        private List<Skill> skills;
        private List<string> skillsNames;
        private List<string> abilityNames;


        public List<Course> Courses { get => courses; set => courses = value; }
        public ObservableCollection<Company> CompaniesObservable { get => companiesObservable; set => companiesObservable = value; }
        public List<Company> Companies { get => companies; set => companies = value; }
        public List<Project> Projects { get => projects; set => projects = value; }
        public List<School> Schools { get => schools; set => schools = value; }
        public List<Skill> Skills { get => skills; set => skills = value; }
        public List<string> SkillsNames { get => skillsNames; set => skillsNames = value; }
        public List<Dev> Devs { get => devs; set => devs = value; }
        public List<string> AbilityNames { get => abilityNames; set => abilityNames = value; }

        public Service()
        {
            random = new Random((int)DateTime.Now.Ticks);
            Devs = new List<Dev>();
            Courses = new List<Course>();
            CompaniesObservable = new ObservableCollection<Company>();
            Companies = new List<Company>();
            Projects = new List<Project>();
            Skills = new List<Skill>();
            Schools = new List<School>();
            SkillsNames = new List<string>();
            AbilityNames = new List<string>();
            StartGame();
        }

        public Service(Dictionary<int, TcpClient> clients)
        {
            random = new Random((int)DateTime.Now.Ticks);
            Clients = clients;
            Devs = new List<Dev>();
            Courses = new List<Course>();
            CompaniesObservable = new ObservableCollection<Company>();
            Companies = new List<Company>();
            Projects = new List<Project>();
            Skills = new List<Skill>();
            Schools = new List<School>();
            SkillsNames = new List<string>();
            AbilityNames = new List<string>();
            StartGame();

        }

        #endregion



        /// <summary>
        /// extract list of name/skills/school from the json file at the game's start
        /// </summary>
        public void StartGame()
        {

            var result = Encoding.UTF8.GetString(resourceFile1.randomValue);//extract json from the resource files
            JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(result);
            SkillsNames = jsonData.SkillNames;
            AbilityNames = jsonData.AbilitySkillNames;
            Skills = generateListOfSkillBlank();
            jsonData.DevNames.ForEach(d => this.Devs.Add(CreateDev(d))); //create devs with  random skills
            jsonData.SchoolNames.ForEach(school => this.Schools.Add(new School(school)));
            this.Schools.ForEach(s => { List<Course> courses = CreateCourse(s); s.Courses = courses; Courses.AddRange(courses); }); //generate course for each school
            Projects = CreateProject(5);
            StartGeneral = new General(Devs, Projects, Schools);

            CompaniesObservable.CollectionChanged += Companies_CollectionChanged;
            
        }

        private async void Companies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                await Task.Delay(500);
                CheckNumberOfPlayer(Clients);
            }
        }

        public void CheckNumberOfPlayer(Dictionary<int, TcpClient> clients)
        {
            if (clients.Count == NumberPlayer)
            {
                Companies = new List<Company>(CompaniesObservable);
                foreach (KeyValuePair<int, TcpClient> client in clients)
                {
                    List<Company> theirCompany = Companies.Where(company => company.Id != client.Key).ToList();
                    byte[] outMyStream = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new Communication("AddTheirCompany", JsonConvert.SerializeObject(theirCompany, jsonSetting)), jsonSetting));
                    client.Value.GetStream().Write(outMyStream, 0, outMyStream.Length);
                    client.Value.GetStream().Flush();
                }
                CreatePlayerOrder();
            }
        }


        public List<Communication> ReceiveInfo(Communication info, int clientID)
        {

            switch (info.Action)
            {
                case "AddNewCompany":
                    return CreateCompany(info.Data, clientID);

                case "AddDevToCompany":
                    return AddDevToCompany((Companies.Where(company => company.Id == clientID).First() as Company), JsonConvert.DeserializeObject<Dev>(info.Data, jsonSetting));

                case "AddProjectToCompany":
                    return AddProjectToCompany((Companies.Where(company => company.Id == clientID).First() as Company), JsonConvert.DeserializeObject<DevToProject>(info.Data, jsonSetting));

                case "AddDevToCourse":
                    DevToCourse devToCourse = JsonConvert.DeserializeObject<DevToCourse>(info.Data);
                    return AddDevToCourse((Companies.Where(company => company.Id == clientID).First() as Company), devToCourse.Course, devToCourse.Dev);

                case "WhoStart":
                    actualIdClient = keysList.First();
                    SendToClient(Clients[clientID].GetStream(), Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new Communication("WhoStart", actualIdClient.ToString()))));
                    return null;

                case "WhoNext":
                    actualIdClient = nextClient;
                    SendToClient(Clients[clientID].GetStream(), Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new Communication("WhoNext", actualIdClient.ToString()))));
                    return null;

                case "SetupGame":
                    SetupGame(clientID, info);
                    return null;

                case "NextTurn":
                    return NextTurn(info.Data);
                default:
                    return null;
            }

        }

        private void SetupGame(int clientId, Communication info)
        {
            Clients.Remove(clientId);
            GameParameters gameParameters = JsonConvert.DeserializeObject<GameParameters>(info.Data);
            AffectValue(gameParameters);
            StartGeneral.NbPlayerMax = NumberPlayer;
            StartGeneral.MoneyStart = Money;
            StartGeneral.NbTurn = NumberTurn;
            SendToOther(new Communication("SetupGame", JsonConvert.SerializeObject(StartGeneral, jsonSetting)));


        }

        public void PrepareNextTurn(Company company)
        {
            //It would have been better to not create any new method (because they are called onlyy once), and so avoiding the repetition of service.companies.where(c=>c.id = idClient)
            //-reduce the value on the projects and courses turn counter
            ReduceProjectTurnCounter(company);
            ReduceCourseTurnCounter(company);
            //-gain money from projects ( from project where remaining turn = 0) and take dev back from project
            EarnCompleteProjectReward(company);
            RecoverDevFromProject(company);
            //- improve dev skill and take them back from their courses
            RecoverDevFromCourse(company);//will improve dev skills
            // -pay dev salary
            PayDevSalary(company);


        }
        private List<Communication> NextTurn(string info)
        {
            Company company = JsonConvert.DeserializeObject<Company>(info);
            if (keysList.Count < keysList.IndexOf(actualIdClient) + 1)
            {  
                nextClient = keysList.IndexOf(actualIdClient) + 1;
                if (turnActual > 1)
                {
                    PrepareNextTurn(company);
                }
            } else
            {
                turnActual += 1;
                nextClient = keysList.First();
                PrepareNextTurn(company);

            }
            return new List<Communication>() { new Communication("MyNextTurn", JsonConvert.SerializeObject(company, jsonSetting)), new Communication("HisNextTurn", JsonConvert.SerializeObject(company, jsonSetting)) }; // Attente de l'id du prochain joueur 

        }
        private void AffectValue(GameParameters gameParameters)
        {
            Money = gameParameters.Money;
            SkillDevLevel = gameParameters.SkillDev;
            StartNumberDev = gameParameters.NumberDev;
            NumberTurn = gameParameters.NumberTurn;
            SkillProjectLevel = gameParameters.SkillProject;
            NumberPlayer = gameParameters.NumberPlayer;
            PriceCourse = gameParameters.PriceCourse;
            DurationMinimumTurnProject = gameParameters.MinCourse;
            DurationMinimumTurnProject = gameParameters.MaxCourse;
            Salary = gameParameters.SalaryDev;
            HiringCost = gameParameters.HiringDev;
            RewardProject = gameParameters.Rewards;
            DurationMinimumTurnProject = gameParameters.MinDurationProject;
            DurationMaximumTurnProject = gameParameters.MaxDurationProject;
            MultiHiring = gameParameters.MultiplierHiringDev;
            MultiPriceCourse = gameParameters.MultiplierCourse;
            MultiSalary = gameParameters.MultiplierSalaryDev;
            WinParameter = gameParameters.WinParameter;
            LooseParameter = gameParameters.LooseParameter;
        }



    }
}
