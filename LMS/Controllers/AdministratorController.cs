using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = from c in db.Courses
                            where c.Subject == subject
                            select new
                            {
                                number = c.Number,
                                name = c.Name
                            };


                return Json(query.ToArray());

            }
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query = from p in db.Professors
                        where p.Subject == subject
                        select new
                        {
                            lname = p.LastName,
                            fname = p.FirstName,
                            uid = p.UId
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = (from c in db.Courses
                            where c.Subject == subject
                            && c.Number == number
                            select c).FirstOrDefault();

                if (query != null)
                {
                    return Json(new { success = false });
                }
            }

            Courses newCourse = new Courses();
            newCourse.Subject = subject;
            newCourse.Number = (uint)number;
            newCourse.Name = name;
            db.Courses.Add(newCourse);
            db.SaveChanges();

            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            int newClassID = 1;
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query1 = (from c in db.Classes
                             join q in db.Courses on c.CourseId equals q.CourseId
                             where c.Season == season && q.Subject == subject
                             select c).FirstOrDefault();

                if (query1 != null)
                {
                    return Json(new { success = false });
                }

                var query2 = (from c in db.Classes
                              where c.Season == season 
                              && c.Location == location 
                              && ((c.Start >= start.TimeOfDay && c.Start <= end.TimeOfDay) || (c.End >= start.TimeOfDay && c.End <= end.TimeOfDay))
                              select c).FirstOrDefault();

                if (query2 != null)
                {
                    return Json(new { success = false });
                }

                var classes = (from c in db.Classes
                               select c.ClassId);
                List<int> list = new List<int>();
                foreach (int classID in classes)
                {
                    list.Add(classID);
                }

                if (list.Count() > 0)
                {
                    newClassID = list.Max() + 1;
                }


                var courseID = (from c in db.Courses
                                where c.Subject == subject && c.Number == number
                                select c.CourseId).FirstOrDefault();


                Classes newClass = new Classes();
                newClass.Year = (uint)year;
                newClass.Season = season;
                newClass.Location = location;
                newClass.Start = start.TimeOfDay;
                newClass.End = end.TimeOfDay;
                newClass.CourseId = courseID;
                newClass.UId = instructor;
                newClass.ClassId = (uint)newClassID;
                db.Classes.Add(newClass);
                db.SaveChanges();

                return Json(new { success = true });

            }
        }


        /*******End code to modify********/

    }
}