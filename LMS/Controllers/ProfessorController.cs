using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query =
                    from c in db.Courses
                    where subject == c.Subject && num == c.Number
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where season == cl.Season && year == cl.Year
                    join e in db.Enrollment on cl.ClassId equals e.ClassId
                    join s in db.Students on e.UId equals s.UId
                    select new
                    {
                        fname = s.FirstName,
                        lname = s.LastName,
                        uid = s.UId,
                        dob = s.Dob,
                        grade = e == null ? "--" : e.Grade
                    };


                return Json(query.ToArray());
            }
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {

                if (category != null)
                {
                    var query =
                        from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join cl in db.Classes on c.CourseId equals cl.CourseId
                        where cl.Season == season && cl.Year == year
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        where ac.Name == category
                        join a in db.Assignments on ac.Acid equals a.Acid

                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            submissions = (from s in db.Submission
                                           where a.Aid == s.Aid
                                           select s).Count()
                        };

                    return Json(query.ToArray());
                }
                else
                {
                    var query =
                        from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join cl in db.Classes on c.CourseId equals cl.CourseId
                        where cl.Season == season && cl.Year == year
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId

                        join a in db.Assignments on ac.Acid equals a.Acid

                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            submissions = (from s in db.Submission
                                           where a.Aid == s.Aid
                                           select s).Count()
                        };
                    return Json(query.ToArray());
                }
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query =
                    from c in db.Courses
                    where c.Subject == subject && c.Number == num
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where cl.Season == season && cl.Year == year
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    select new
                    {
                        name = ac.Name,
                        weight = ac.GradingWeight
                    };
                return Json(query.ToArray());

            }
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            int newAssignmentCatID = 1;
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = (from ac in db.AssignmentCategories
                             where ac.Name == category
                             join c in db.Classes on ac.ClassId equals c.ClassId
                             select ac).FirstOrDefault();
                if (query != null)
                {
                    return Json(new { success = false });
                }


                var query2 = (from ac in db.AssignmentCategories
                              where ac.Name == category
                              join c in db.Classes on ac.ClassId equals c.ClassId
                              where c.Year == year && c.Season == season
                              join cou in db.Courses on c.CourseId equals cou.CourseId
                              where cou.Number == num && cou.Subject == subject
                              select new
                              {
                                  assiClassID = c.ClassId
                              }).FirstOrDefault();

                var assignmentIDs = (from a in db.Assignments
                                     select a.Aid);
                List<int> list = new List<int>();
                foreach (int a in assignmentIDs)
                {
                    list.Add(a);
                }

                if (list.Count() > 0)
                {
                    newAssignmentCatID = list.Max() + 1;
                }

                AssignmentCategories assiCat = new AssignmentCategories();
                assiCat.Acid = (uint)newAssignmentCatID;
                assiCat.Name = category;
                assiCat.GradingWeight = (uint)catweight;
                assiCat.ClassId = query2.assiClassID;

                db.AssignmentCategories.Add(assiCat);
                db.SaveChanges();

                return Json(new { success = true });

            }
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            int newAssignmentID = 1;
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = (from a in db.Assignments
                             where a.Name == asgname
                             join ac in db.AssignmentCategories on a.Acid equals ac.Acid
                             select a).FirstOrDefault();
                if (query != null)
                {
                    return Json(new { success = false });
                }


                var query2 = (from ac in db.AssignmentCategories
                              where ac.Name == category
                              join c in db.Classes on ac.ClassId equals c.ClassId
                              where c.Year == year && c.Season == season
                              join cou in db.Courses on c.CourseId equals cou.CourseId
                              where cou.Number == num && cou.Subject == subject
                              select new
                              {
                                  assiCat = ac.Acid
                              }).FirstOrDefault();

                var assignmentIDs = (from a in db.Assignments
                                     select a.Aid);
                List<int> list = new List<int>();
                foreach (int a in assignmentIDs)
                {
                    list.Add(a);
                }

                if (list.Count() > 0)
                {
                    newAssignmentID = list.Max() + 1;
                }

                Assignments assignment = new Assignments();
                assignment.Name = asgname;
                assignment.Contents = asgcontents;
                assignment.Due = asgdue;
                assignment.MaxPointValue = (uint)asgpoints;
                assignment.Aid = newAssignmentID;
                assignment.Acid = query2.assiCat;

                db.Assignments.Add(assignment);
                db.SaveChanges();

                return Json(new { success = true });

            }
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = from s in db.Submission
                            join st in db.Students on s.UId equals st.UId
                            where s.UId == st.UId
                            join a in db.Assignments on s.Aid equals a.Aid
                            where a.Name == asgname
                            join ac in db.AssignmentCategories on a.Acid equals ac.Acid
                            where ac.Name == category
                            join c in db.Classes on ac.ClassId equals c.ClassId
                            where c.Season == season && c.Year == year
                            join cou in db.Courses on c.CourseId equals cou.CourseId
                            where cou.Subject == subject && cou.Number == num
                            select new
                            {
                                fname = st.FirstName,
                                lname = st.LastName,
                                uid = st.UId,
                                time = s.DateTime,
                                score = s.Score
                            };

                db.SaveChanges();

                return Json(new { success = true });
            }
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = from s in db.Submission
                            where s.UId == uid
                            join a in db.Assignments on s.Aid equals a.Aid
                            where a.Name == asgname
                            join ac in db.AssignmentCategories on a.Acid equals ac.Acid
                            where ac.Name == category
                            join c in db.Classes on ac.ClassId equals c.ClassId
                            where c.Season == season && c.Year == year
                            join cou in db.Courses on c.CourseId equals cou.CourseId
                            where cou.Subject == subject && cou.Number == num
                            select s;

                foreach (Submission s in query)
                {
                    s.Score = (uint)score;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }

        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query = from c in db.Classes
                            where c.UId == uid
                            join k in db.Courses on c.CourseId equals k.CourseId
                            select new
                            {
                                subject = k.Subject,
                                number = k.Number,
                                name = k.Name,
                                season = c.Season,
                                year = c.Year
                            };

                return Json(query.ToArray());
            }
        }


        /*******End code to modify********/

    }
}