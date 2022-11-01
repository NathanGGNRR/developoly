using Developoly.Server.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Developoly.Server.Services
{
    public partial class Service
    {

        public List<Project> CreateProject(int quantity)
        {
            List<Project> projects = new List<Project>();
            for (int i = 0; i < quantity; i++)
            {

                List<Skill> skillProjet = this.generateListOfSkill(SkillProjectLevel, true);
                projects.Add(new Project(projects.Count + 1, random.Next(DurationMinimumTurnProject, DurationMaximumTurnProject), skillProjet.Sum(s => s.Level) * RewardProject, skillProjet));
                quantity--;
            }
            return projects;
        }

        public void ReduceProjectTurnCounter(Company company)
        {
            company.Projects.ForEach(p => p.Duration--);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        public void EarnCompleteProjectReward(Company company)
        {

            company.Money += company.Projects.Where(p => p.Duration == 0).Sum(p => p.Reward);

        }

        /// <summary>
        ///  set the Project value of each participating dev to null , delete the project from the company's list of project
        /// </summary>
        /// <param name="clientId"></param>
        public void RecoverDevFromProject(Company company)
        {
            var proj = company.Projects;
            proj.Where(p => p.Duration == 0).ToList().ForEach(p => RemoveDevFromProject(p));
            proj.RemoveAll(p => p.Duration == 0);
        }
    }
}
