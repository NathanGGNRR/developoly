using Developoly.Server.Entities;
using Developoly.Server.Services.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Developoly.Server.Services
{
    public partial class Service
    {
        
	
        /// <summary>
        /// Create a Company, Check if the company does not already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientID"></param>
        /// <returns>Communication("TheAction", Json)</returns>
        public List<Communication> CreateCompany(string name, int clientID)
        {
            if (CompaniesObservable.Count(company => company.Name == name) == 0)
            {  
                Company company = new Company(clientID, name, Money);
                CompaniesObservable.Add(company);
                General generalClient = new General(company, StartGeneral.UnemployedDevs, StartGeneral.NewProjects, StartGeneral.Schools, Clients.Count, NumberPlayer, Money, NumberTurn);
                return new List<Communication>() { new Communication("MyCompanyAddSuccess", JsonConvert.SerializeObject(generalClient, jsonSetting)), new Communication("AddNewPlayerSuccess", Clients.Count.ToString()) };            
            }
            return new List<Communication>() { new Communication("MyCompanyAlreadyExist", null) };

        }
        /// <summary>
        /// Add a Developer to a company, Check if he's not already in a company, check if the player has enough money
        /// </summary>
        /// <param name="company"></param>
        /// <param name="dev"></param>
        /// <returns>Communication("TheAction", Json)</returns>
        public List<Communication> AddDevToCompany(Company company, Dev dev)
        {
            if(dev.Company == null)
            {   
                if(AlterMoney(company.Money, -(dev.HiringCost)) != null)
                {
                    dev.Company = company;
                    company.Devs.Add(dev);
                    company.Money = Convert.ToInt32(AlterMoney(company.Money, -(dev.HiringCost)));
                    return new List<Communication>() { new Communication("MyDevAddSuccess", JsonConvert.SerializeObject(company, jsonSetting)), new Communication("HisDevAddSuccess", JsonConvert.SerializeObject(company, jsonSetting)) };
                }
                WhyAlterMoneyNull(company.Money, -(dev.HiringCost), company);
                return new List<Communication>() { new Communication("MyDevAddNotEnoughMoney", JsonConvert.SerializeObject(dev, jsonSetting)) };

            }
            return new List<Communication>() { new Communication("MyDevAddAlreadyExist", null) };

        }


        private void LooseOrWinGame(Company company)
        {
            if (company.Money >= LooseParameter)
            {
                if (company.Money >= WinParameter)
                {
                    WinGame(company);
                }
            }
            else
            {
                LoseGame(company);
            }
        }

        private void WhyAlterMoneyNull(int moneyCompany, int moneyLessOrMore, Company company)
        {
            if (moneyCompany + moneyLessOrMore >= LooseParameter)
            {
                if (moneyCompany >= WinParameter)
                {
                    WinGame(company);
                }
            }
            else
            {
                LoseGame(company);
            }
        }

        /// <summary>
        /// Add a Project to a company, Check if the Selected devs are Qualified
        /// </summary>
        /// <param name="company"></param>
        /// <param name="devToProject"></param>
        /// <returns>Communication("TheAction", Json)</returns>
        public List<Communication> AddProjectToCompany(Company company, DevToProject devToProject)
        {
            List<Communication> alreadyBusy = CheckAvailability(devToProject.Devs);
            if (alreadyBusy == null)
            {
                List<Skill> skillMissing = CompareSkills(devToProject.Project.Skills, SumSkills(devToProject.Devs));
                if (skillMissing == null)
                {
                    devToProject.Devs.ForEach(dev =>
                    {
                        dev.Company = company;
                        dev.Projet = devToProject.Project;
                        devToProject.Project.Devs.Add(dev);
                        company.Devs.Where(d => d.Id == dev.Id).FirstOrDefault().Projet = devToProject.Project;
                    });
                    devToProject.Project.Company = company;
                    
                    company.Projects.Add(devToProject.Project);
                    return new List<Communication>() { new Communication("MyProjectAddSuccess", JsonConvert.SerializeObject(company, jsonSetting)), new Communication("HisProjectAddSuccess", JsonConvert.SerializeObject(company, jsonSetting)) };
                }
                return new List<Communication>() { new Communication("MyDevelopperNotQualified", JsonConvert.SerializeObject(skillMissing, jsonSetting)) };
            }
            return alreadyBusy;
        }

        /// <summary>
        /// Create a Key List and return the first player to play 
        /// </summary>
        /// <returns>First element of a List<int></returns>
        public int CreatePlayerOrder()
        {
            List<int> keyList = new List<int>();
            List<int> rngkeyList = new List<int>();

            keyList = clients.Keys.ToList();
            rngkeyList = Shuffle(keyList);

            keysList = rngkeyList;
            return rngkeyList.First();
        }

        /// <summary>
        /// Mix all ID of Player 
        /// </summary>
        /// <param name="clientsId"></param>
        /// <returns>List<int> of DevId</returns>
        public static List<int> Shuffle(List<int> clientsId)
        {
            int nbOfPlayers = clientsId.Count;

            while (nbOfPlayers > 1)
            {
                Random rng = new Random();
                nbOfPlayers--;
                int random = rng.Next(nbOfPlayers + 1);
                int value = clientsId[random];
                clientsId[random] = clientsId[nbOfPlayers];
                clientsId[nbOfPlayers] = value;
            }
            return clientsId;
        }
        /// <summary>
        /// Check who is the next Player
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>Int of the next player</returns>
        public int NextPlayerOrder(int clientId)
        {
            int nexPlayerId;
            if (keysList.Count < keysList.IndexOf(clientId))
            {
               return nexPlayerId = keysList.IndexOf(clientId)+1;
            }
            else
            {
               return nexPlayerId = 0;
            }
        }



        /// <summary>
        /// Give or Take money from the player ,if the player has lost enable the lost method, if the player has win enable the win method 
        /// </summary>
        /// <param name="moneyCompany"></param>
        /// <param name="moneyLessOrMore"></param>
        /// <returns></returns>
        public int? AlterMoney(int moneyCompany, int moneyLessOrMore)
        {
            if (moneyCompany + moneyLessOrMore > 0)
            {
                return moneyCompany += moneyLessOrMore;
            }
            return null;
        }
        /// <summary>
        /// Method when the player has win
        /// </summary>
        /// <param name="company"></param>
        public void WinGame (Company company)
        {
            SendToOther(new Communication("Win", JsonConvert.SerializeObject(company, jsonSetting)));
        }

        /// <summary>
        /// Method when the player has lost
        /// </summary>
        /// <param name="company"></param>
        public void LoseGame (Company company)
        {
            SendToOther(company.Id, new Communication("ACompanyLost", JsonConvert.SerializeObject(company, jsonSetting)));
            SendToClient(Clients[company.Id].GetStream(), Encoding.Unicode.GetBytes("GameOver"));
        }

    }
}
