

using Microsoft.AspNetCore.Mvc;
using CUETSchool.Data;
using CUETSchool.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace CUETSchool.Controllers
{
    public class AdminResultController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminResultController(ApplicationDbContext db)
        {
            _db = db;
        }
       
        public IActionResult InputClass()
        {
            var results = _db.Results.ToList();
            var classes = results.Select(r => r.Class).Distinct().ToList();

            var notifications = classes.ToDictionary(
                c => c,
                c => _db.Results.Where(r => r.Class == c).Select(r => r.StudentId).Distinct().Count()
            );

          
            ViewBag.Notifications = notifications;

            return View(classes);
        }

        [HttpPost]
        public IActionResult Index(int selectedClass)
        {
           
            var results = _db.Results.Where(r => r.Class == selectedClass).ToList();

            if (!results.Any())
            {
                ViewBag.SelectedClass = selectedClass;
                ViewBag.Subjects = new List<string>();
                return View(new List<Dictionary<string, object>>()); 
            }

          
            var studentIds = results.Select(r => r.StudentId).Distinct().ToList();
            var subjects = results.Select(r => r.SubjectName).Distinct().ToList();

            var studentResults = new List<Dictionary<string, object>>();

            foreach (var studentId in studentIds)
            {
                var studentData = new Dictionary<string, object>
        {
            { "StudentId", studentId }
        };

                int totalMarks = 0;
                double grades_Marks = 0.00;

                foreach (var subject in subjects)
                {
                    var subjectResult = results
                        .FirstOrDefault(r => r.StudentId == studentId && r.SubjectName == subject);

                    int marks = subjectResult?.TotalMarks ?? 0;
                    studentData[subject] = marks;
                    totalMarks += marks;
                    string gpa = subjectResult?.Grade ?? "F";

                    double grades = gpa switch
                    {
                        "A+" => 5.0,
                        "A" => 4.0,
                        "A-" => 3.5,
                        "B+" => 3.25,
                        "B-" => 3.0,
                        "C" => 2.5,
                        "D" => 2.0,
                        _ => 0.0,
                    };

                    grades_Marks += grades;
                }

                double grade = grades_Marks / subjects.Count;

                studentData["TotalMarks"] = totalMarks;
                studentData["Grade"] = grade;
                studentResults.Add(studentData);
            }

            ViewBag.SelectedClass = selectedClass;
            ViewBag.Subjects = subjects;
            return View(studentResults);
        }

        

    }
}


