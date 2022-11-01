using Developoly.Server.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Developoly.Server.Services
{
    public partial class Service
    {

        public List<Skill> SumSkills(List<Dev> devs)
        {
            List<Skill> skills = new List<Skill>();
            if (devs.Count > 0)
            {
                if (devs.Count > 1)
                {
                    devs.First().Skills.ForEach(skillFirstDev =>
                    {
                        int valeurMax = 0;
                        devs.ForEach(developper => developper.Skills.Where(dS => dS.Id == skillFirstDev.Id && dS.Level >= valeurMax).ToList().ForEach(skillUp => valeurMax = skillUp.Level));
                        skills.Add(new Skill(skillFirstDev.Id, skillFirstDev.Name, valeurMax, skillFirstDev.Technical));
                    });
                } else
                {
                    return devs.First().Skills;
                }
            }
            
            return skills;
        }

        public List<Skill> generateListOfSkill(int minSkillValue, bool onlyTechnical)
        {
            List<Skill> theSkills = new List<Skill>();

            this.SkillsNames.ForEach(b => theSkills.Add(
                new Skill
                {
                    Id = theSkills.Count + 1,
                    Name = b,
                    Level = (random.Next(1, 100) < minSkillValue ? 0 : random.Next(minSkillValue, 100)),
                    Technical = true
                }
            ));
            if (onlyTechnical)
            {
                this.AbilityNames.ForEach(b => theSkills.Add(
                    new Skill
                    {
                        Id = theSkills.Count + 1,
                        Name = b,
                        Level = 0,
                        Technical = false
                    }
                ));
            } else
            {
                this.AbilityNames.ForEach(b => theSkills.Add(
                   new Skill
                   {
                       Id = theSkills.Count + 1,
                       Name = b,
                       Level = (random.Next(1, 100) < minSkillValue ? 0 : random.Next(minSkillValue, 100)),
                       Technical = false
                   }
               ));
            }

            return theSkills;
        }

       

        public List<Skill> generateListOfSkillBlank()
        {
            List<Skill> theSkills = new List<Skill>();
            this.SkillsNames.ForEach(b => theSkills.Add(
                new Skill
                {
                    Id = theSkills.Count + 1,
                    Name = b,
                    Level = 0,
                    Technical = true
                }
                ));
            this.AbilityNames.ForEach(b => theSkills.Add(
                new Skill
                {
                    Id = theSkills.Count + 1,
                    Name = b,
                    Level = 0,
                    Technical = false
                }
            ));
            return theSkills;
        }


        public Skill generateOneSkill(List<Skill> skills, int idSkill)
        {
            Skill skill = skills.Where(s => s.Id == idSkill).First();
            while (skill.Level == 0)//the while allow th ecourse to always deliver a skill with a value (and note a course with a skill level of zero)
            {
                skill.Level = (random.Next(1, 100) < SkillDevLevel ? 0 : random.Next(SkillDevLevel, 100));

            }
            return skill;
        }

        public List<Skill> CompareSkills(List<Skill> projectSkill, List<Skill> devSkill)
        {

            List<Skill> skills = devSkill.Where(developperS => developperS.Level < projectSkill.Where(project => project.Id == developperS.Id).FirstOrDefault().Level).ToList();
            if (skills.Count != 0)
            {
                return skills;
            }
            return null;

        }
    }
}
