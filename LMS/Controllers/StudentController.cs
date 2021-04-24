using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
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


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query =
                    from s in db.Students
                    join e in db.Enrollment on s.UId equals e.UId
                    join cl in db.Classes on e.ClassId equals cl.ClassId
                    join c in db.Courses on cl.CourseId equals c.CourseId
                    where s.UId == uid

                    select new
                    {
                        subject = c.Subject,
                        number = c.Number,
                        name = c.Name,
                        season = cl.Season,
                        year = cl.Year,
                        grade = e == null ? "--" : e.Grade
                    };
                return Json(query.ToArray());
            }
        }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
            
            using (Team36LMSContext db = new Team36LMSContext())
            {
                bool flag = true;
                var check =
                    from c in db.Courses
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                    join e in db.Enrollment on uid equals e.UId
                    where e.ClassId == cl.ClassId

                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId

                    join assignment in db.Assignments on ac.Acid equals assignment.Acid

                    join s in db.Submission on assignment.Aid equals s.Aid

                    select new {
                        score = s == null ? null : (uint?)s.Score
                    };

                
                if (check.ToArray().FirstOrDefault() == null) { flag = false; };
                //System.Diagnostics.Debug.WriteLine("Check :" + check.ToArray().FirstOrDefault() + "hhh" + flag);

                if (flag)
                {
                    var query =
                    from c in db.Courses
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                    join e in db.Enrollment on uid equals e.UId
                    where e.ClassId == cl.ClassId

                    join ac in db.AssignmentCategories on e.ClassId equals ac.ClassId

                    join assignment in db.Assignments on ac.Acid equals assignment.Acid

                    join s in db.Submission on assignment.Aid equals s.Aid

                    select new
                    {
                        aname = assignment.Name,
                        cname = ac.Name,
                        due = assignment.Due,
                        score = s == null ? null : (uint?)s.Score
                    };
                    return Json(query.ToArray());

                }
                else {
                    var query =
                        from c in db.Courses
                        join cl in db.Classes on c.CourseId equals cl.CourseId
                        where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                        join e in db.Enrollment on uid equals e.UId
                        where e.ClassId == cl.ClassId

                        join ac in db.AssignmentCategories on e.ClassId equals ac.ClassId

                        join assignment in db.Assignments on ac.Acid equals assignment.Acid

                        select new
                        {
                            aname = assignment.Name,
                            cname = ac.Name,
                            due = assignment.Due
                        };

                    return Json(query.ToArray());
                }
                    
                
            }
        }



    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
	/// Does *not* automatically reject late submissions.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}.</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {

            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query =
                    from c in db.Courses
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                    join e in db.Enrollment on uid equals e.UId
                    where e.ClassId == cl.ClassId

                    join ac in db.AssignmentCategories on e.ClassId equals ac.ClassId
                    where ac.Name == category

                    join assignment in db.Assignments on ac.Acid equals assignment.Acid
                    where assignment.Name == asgname

                    select assignment.Aid;

                var assignmentID = query.First();

                //System.Diagnostics.Debug.WriteLine(assignmentID);
                var query2 =
                    from s in db.Submission
                    where s.UId == uid && s.Aid == assignmentID
                    select s;

                if (query2.Count() != 0)
                {

                    //update
                    query2.First().Contents = contents;
                    query2.First().DateTime = DateTime.Now;

                    db.SaveChanges();

                    return Json(new { success = true });
                }

                Submission newS = new Submission();
                newS.UId = uid;
                newS.DateTime = DateTime.Now;
                newS.Score = 0;
                newS.Contents = contents;
                newS.Aid = assignmentID;

                db.Submission.Add(newS);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false},
	/// false if the student is already enrolled in the Class.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {

            using (Team36LMSContext db = new Team36LMSContext())
            {
                var query =
                    from c in db.Courses
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                    select cl.ClassId;

                if (query.Count() == 0)
                {
                    return Json(new { success = false });
                }
                uint classID = (uint)query.FirstOrDefault();
                System.Diagnostics.Debug.WriteLine("hwewew: "+classID);

                var currClass = (from e in db.Enrollment
                                 where e.UId == uid
                                 select e.ClassId);
                foreach (uint cid in currClass) {
                    if (cid == classID) {
                        return Json(new { success = false });
                    }
                }

                Enrollment newE = new Enrollment();
                newE.UId = uid;
                newE.ClassId = classID;
                newE.Grade = "--";

                db.Enrollment.Add(newE);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            using (Team36LMSContext db = new Team36LMSContext())
            {

                double GPA = 0.0;
                double Grades = 0.0;
                int count = 0;
                var query =
                    from e in db.Enrollment
                    where e.UId == uid
                    select e.Grade;


                foreach (string g in query)
                {
                    if (g == "A") { Grades += 4.0; count++; }
                    else if (g == "A-") { Grades += 3.7; count++; }
                    else if (g == "B+") { Grades += 3.3; count++; }
                    else if (g == "B") { Grades += 3.0; count++; }
                    else if (g == "B-") { Grades += 2.7; count++; }
                    else if (g == "C+") { Grades += 2.3; count++; }
                    else if (g == "C") { Grades += 2.0; count++; }
                    else if (g == "C-") { Grades += 1.7; count++; }
                    else if (g == "D+") { Grades += 1.3; count++; }
                    else if (g == "D") { Grades += 1.0; count++; }
                    else if (g == "D-") { Grades += 0.7; count++; }
                    else if (g == "F") { Grades += 0.0; count++; }

                }
                if (count != 0) { GPA = Grades / count; }

                //System.Diagnostics.Debug.WriteLine("GPA is: "+GPA);

                return Json(new { gpa = GPA });

            }

            //return Json(new { gpa = 0.0 });
        }

        /*******End code to modify********/

    }
    }