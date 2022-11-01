using Developoly.Server.Entities;
using Developoly.Server.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace Developoly.Server.Services
{
    public partial class Service
    {
        /// <summary>
        ///  Create developer, check if he does not already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Developer</returns>
        public Dev CreateDev(string name)
        {
            if (Devs.Count == 0 || Devs.Count(d => d != null && d.Name == name) == 0)
            {
                Dev dev = new Dev(Devs.Count + 1, name, _salary, _hiringCost);
                dev.Skills = this.generateListOfSkill(SkillDevLevel, false);
                dev.HiringCost =  HiringCost + (dev.Skills.Where(s => s.Level > SkillDevLevel).Sum(s => s.Level) * MultiHiring);
                dev.Salary = Salary + (dev.Skills.Sum(s => s.Level) * MultiSalary); 
                return dev;
            }
            return null;
        }


<<<<<<< HEAD
        /// <summary>
        /// Add a Developer to a project, Check if he has the necessary skills and if he's not already busy
        /// </summary>
        /// <param name="company"></param>
        /// <param name="project"></param>
        /// /// <param name="devs"></param>
        /// <returns>Communication("TheAction", Json)</returns>
        public List<Communication> AddDevToProject(Company company, Project project, List<Dev> devs)
        {
            List<Communication> alreadyBusy = CheckAvailability(devs);
            if (alreadyBusy == null && CompareSkills(project.Skills, SumSkills(devs)))
            {
                devs.ForEach(dev =>
                {
                    dev.Projet = project;
                    project.Devs.Add(dev);
                    company.Projects.Add(project);
                    // Initialize turn system
                });
                return new List<Communication>() { new Communication("MyDevToProjectSuccess", JsonConvert.SerializeObject(new DevToProject(project, devs), jsonSetting)), new Communication("HisDevToProjectSuccess", JsonConvert.SerializeObject(new DevToProject(project, devs), jsonSetting)) };
            }
            return alreadyBusy;
        }

=======
        
>>>>>>> 97f0eef5be4e806ba4d5c6e5b8d2cb08c60b60c6
        /// <summary>
        /// Remove a developer From a project
        /// </summary>
        /// <param name="project"></param>
        public void RemoveDevFromProject(Project project)
        {
            project.Devs.ForEach(d => d.Projet = null);
            project.Devs.RemoveRange(0, project.Devs.Count);
        }

        /// <summary>
        /// Add a Developer to a course, Check if he's not already busy, check if the player has enough money to pay the course
        /// </summary>
        /// <param name="company"></param>
        /// <param name="project"></param>
        /// /// <param name="devs"></param>
        /// <returns>Communication("TheAction", Json)</returns>
        public List<Communication> AddDevToCourse(Company company, Course course, Dev dev)
        {
            List<Communication> alreadyBusy = CheckAvailability(dev);
            if (alreadyBusy == null)
            { 
                if (AlterMoney(company.Money, -(course.Price)) != null)
                {
                    dev.Course = course;
                    course.Dev = dev;
                    company.Money = Convert.ToInt32(AlterMoney(company.Money, -(course.Price)));
                    return new List<Communication>() { new Communication("MyDevToCourseSuccess", JsonConvert.SerializeObject(new DevToCourse(course, dev), jsonSetting)), new Communication("HisDevToCourseSuccess", JsonConvert.SerializeObject(new DevToCourse(course, dev), jsonSetting)) };

                }
                WhyAlterMoneyNull(company.Money, -(course.Price), company);
                return new List<Communication>() { new Communication("MyDevToCourseNotEnoughMoney", null) };
                // Initialize turn system and win at every turn level/duration

            }
            return alreadyBusy;
        }

        /// <summary>
        /// Remove Dev from a Course, and give him exeperience
        /// </summary>
        /// <param name="dev"></param>
        public void RemoveDevFromCourse(Dev dev)
        {
            var theCourse = dev.Course;
            dev.Skills.Where(skill => skill.Id == dev.Course.Skill.Id).First().Level += dev.Course.Skill.Level;
            dev.Course.Dev = null;
            dev.Course = null;
            dev.Skills.ForEach(s => { if (s.Level > 100) { s.Level = 100; } });
        }


        /// <summary>
        /// Check if the developers are not busy (not in a course or a project)
        /// </summary>
        /// <param name="devs"></param>
        /// <returns>null if they are avaliable or a Communication("TheAction", Json) if they are busy</returns>
        private List<Communication> CheckAvailability(List<Dev> devs)
        {
            List<Dev> devNotAvailable = devs.Where(dev => dev.Course != null || dev.Projet != null).ToList();
            if (devNotAvailable.Count > 0)
            {
                return new List<Communication>() { new Communication("MyDevsAlreadyBusy", JsonConvert.SerializeObject(devNotAvailable, jsonSetting)) };
            }
            return null;
        }

        private List<Communication> CheckAvailability(Dev dev)
        {
            if (dev.Course != null || dev.Projet != null)
            {
                return new List<Communication>() { new Communication("MyDevsAlreadyBusy", JsonConvert.SerializeObject(dev, jsonSetting)) };
            }
            return null;
        }

        public void PayDevSalary(Company company)
        {
            company.Devs.ForEach(d =>d.Company.Money = (int)this.AlterMoney(d.Company.Money,- d.Salary));
            LooseOrWinGame(company);
        }
    }
}
