using Developoly.Server.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Developoly.Server.Services
{
    public partial class Service
    {
        /// <summary>
        ///  Create courses for a school
        /// </summary>
        /// <param name="school"></param>
        /// <returns>List of Course</returns>
        public List<Course> CreateCourse(School school)
        {
            List<Course> coursesSchool = new List<Course>();
            var quantity = random.Next(QuantityNumberMinimumCourse, QuantityNumberMaximumCourse);
            while (quantity > 0)
            {
                int idSkillCourse = random.Next(1, this.SkillsNames.Count);
                Skill skillCourse = generateOneSkill(generateListOfSkillBlank(), idSkillCourse);
                Course course = new Course(coursesSchool.Count + 1, school, PriceCourse + (skillCourse.Level * MultiPriceCourse), (skillCourse.Level < SkillDevLevel) ? 1 : (skillCourse.Level < SkillDevLevel + 20) ? 2 : 3);
                course.Skill = skillCourse;
                coursesSchool.Add(course);
                
                quantity--;
            }
            return coursesSchool;
        }

        /// <summary>
        /// Reduce the number of turn remaining of a project
        /// </summary>
        /// <param name="clientId"></param>
       public void ReduceCourseTurnCounter(Company company) {
            
            company.Devs.Where(d=>d.Course != null).ToList().ForEach(d => d.Course.Duration--);

        }

        /// <summary>
        /// Remove, Developer from a course, when the course is finished.
        /// </summary>
        /// <param name="clientId"></param>
        public void RecoverDevFromCourse(Company company) {
            company.Devs.Where(d => d.Course != null).Where(d=>d.Course.Duration == 0).ToList().ForEach(d=> RemoveDevFromCourse(d));
            company.Projects.RemoveAll(p => p.Duration == 0);
        }

        
    }
}
