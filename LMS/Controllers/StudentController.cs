﻿using System;
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
                    from t in db.Classes
                    join c in db.Courses on t.CourseId equals c.CourseId
                    into getCourse
                    from gc in getCourse.DefaultIfEmpty()
                    join e in db.Enrollment on uid equals e.UId
                    into getGrade1
                    from gg in getGrade1.DefaultIfEmpty()
                    join e in db.Enrollment on gg.ClassId equals e.ClassId
                    into getGrade2
                    from gg2 in getGrade2.DefaultIfEmpty()

                    select new
                    {
                        subject = gc.Subject,
                        number = gc.Number,
                        name = gc.Name,
                        season = t.Season,
                        year = t.Year,
                        grade = gg2 == null ? "--" : gg2.Grade
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
            bool flag = false;
            using (Team36LMSContext db = new Team36LMSContext())
            {
                var check =
                    from c in db.Courses
                    join cl in db.Classes on c.CourseId equals cl.CourseId
                    where c.Subject == subject && c.Number == num && cl.Season == season && cl.Year == year

                    join e in db.Enrollment on uid equals e.UId
                    where e.ClassId == cl.ClassId

                    join ac in db.AssignmentCategories on e.ClassId equals ac.ClassId

                    join assignment in db.Assignments on ac.Acid equals assignment.Acid

                    join s in db.Submission on assignment.Aid equals s.Aid

                    select new {
                        score = s == null ? null : (uint?)s.Score
                    };

                if (check.ToArray().FirstOrDefault() != null) { flag = true; };

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
                            due = assignment.Due,
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

                    select new
                    {
                        assignment.Aid
                    };

                var assignmentID = query.ToArray().FirstOrDefault();
                
                //System.Diagnostics.Debug.WriteLine(assignmentID);

                Submission newS = new Submission();
                newS.UId = uid;
                newS.DateTime = DateTime.Now;
                newS.Score = 0;
                newS.Contents = contents;
                newS.Aid = assignmentID.Aid;

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

                    select new
                    {
                        cl.ClassId
                    };

                var classID = query.ToArray().FirstOrDefault();

                var currClass = (from e in db.Enrollment
                                 where e.UId == uid
                                 select e.ClassId);
                foreach (uint cid in currClass) {
                    if (cid == classID.ClassId) {
                        return Json(new { success = false });
                    }
                }

                Enrollment newE = new Enrollment();
                newE.UId = uid;
                newE.ClassId = classID.ClassId;
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
                String GPA = "0.0";
                double Grades = 0.0;
                int count = 0;
                var grades =
                    from e in db.Enrollment
                    where e.UId == uid
                    select e.Grade;
                if (grades.ToArray().FirstOrDefault() != null)
                {
                    foreach (string g in grades)
                    {
                        if (g == "A") { Grades += 4.0; count++; }
                        else if (g == "A-") { Grades += 3.7; count++; }
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
                    GPA = (Grades / (double)count).ToString();
                    
                    
                }
                System.Diagnostics.Debug.WriteLine("GPA is: "+GPA);
                var gpanum = 
                    from s in db.Students
                    select new {
                        gpa = GPA
                    };
                return Json(gpanum.ToArray());
            }

            
    }

    /*******End code to modify********/

  }
}