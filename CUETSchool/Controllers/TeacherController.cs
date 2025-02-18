using Microsoft.AspNetCore.Mvc;
using CUETSchool.Data;
using CUETSchool.Models;
using System.Linq;
using System.Collections.Generic;

namespace CUETSchool.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewClass(string subject, string className)
        {
            // Fetch students of the specific subject and class
            var students = _context.Students
                .Where(s => s.SubjectName == subject && s.Class == className)
                .ToList();

            // Retrieve FullMarks for the subject and class from the database
            var fullMarks = _context.Students
                                    .Where(r => r.SubjectName == subject && r.Class == className)
                                    .Select(r => r.FullMarks)
                                    .FirstOrDefault();

            // If no full marks exist, set a default value
            if (fullMarks == 0)
            {
                fullMarks = 100;  // You can change the default value if necessary
            }

            // Initialize a HashSet to store unique column names (marks categories)
            HashSet<string> allColumns = new HashSet<string>();

            foreach (var student in students)
            {
                if (!string.IsNullOrEmpty(student.MarksJson))
                {
                    var marksData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(student.MarksJson);
                    foreach (var key in marksData.Keys)
                    {
                        allColumns.Add(key);
                    }
                }
            }

            // Pass the data to the View
            ViewBag.Subject = subject;
            ViewBag.Class = className;
            ViewBag.FullMarks = fullMarks;  // Pass FullMarks to the View
            ViewBag.AllColumns = allColumns; // Pass the list of columns

            return View(students);
        }


        ////[HttpPost]
        ////public IActionResult UpdateFullMarks(int fullMarks, string subject, string className)
        ////{
        ////    // Retrieve results for the specified subject and class
        ////    var results = _context.Students
        ////                          .Where(r => r.SubjectName == subject && r.Class == className)
        ////                          .ToList();

        ////    // Update the FullMarks for each result
        ////    foreach (var result in results)
        ////    {
        ////        result.FullMarks = fullMarks;
        ////    }

        ////    // Save changes to the database
        ////    _context.SaveChanges();

        ////    // Redirect to the same page (or another view)
        ////    return RedirectToAction("ViewClass", new { subject, className });
        ////}

        [HttpPost]
        public IActionResult UpdateFullMarks1(int fullMarks, string subject, string className)
        {
            // Fetch all results for the specific subject and class
            var results = _context.Students
                                  .Where(r => r.SubjectName == subject && r.Class == className)
                                  .ToList();

            // Update the FullMarks for each result
            foreach (var result in results)
            {
                result.FullMarks = fullMarks; // Update FullMarks field
            }

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the same page (or another view)
            return RedirectToAction("ViewClass", new { subject, className });
        }
        [HttpPost]
        public IActionResult UpdateFullMarks(int fullMarks, string subject, string className)
        {
            // Fetch all students for the specified subject and class
            var students = _context.Students
                .Where(s => s.SubjectName == subject && s.Class == className)
                .ToList();

            // Update the FullMarks for each student
            foreach (var student in students)
            {
                student.FullMarks = fullMarks;  // Update FullMarks field
            }

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the same page (or another view)
            return RedirectToAction("ViewClass", new { subject, className });
        }




        // Action to show students of a specific class and subject



        // Action to update the student's result
        [HttpPost]
        public IActionResult Edit([FromBody] Result updatedResult, int fullMarks)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == updatedResult.Id);
            if (student == null) return NotFound();

            // Deserialize existing marks
            var marksData = student.MarksJson != null
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(student.MarksJson)
                : new Dictionary<string, int>();

            // Deserialize new marks from request
            var updatedMarks = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(updatedResult.MarksJson);

            // Merge updated values
            foreach (var key in updatedMarks.Keys)
            {
                marksData[key] = updatedMarks[key];
            }

            // Store updated marks back to JSON
            student.MarksJson = System.Text.Json.JsonSerializer.Serialize(marksData);

            // Calculate Total Marks
            student.TotalMarks = marksData.Values.Sum();

            // Use the fullMarks value passed from the client
            double percentage = (double)student.TotalMarks / fullMarks * 100;

            // Assign Grade based on the new grading scale
            string grade;
            if (percentage >= 80) grade = "A+";
            else if (percentage >= 75) grade = "A";
            else if (percentage >= 70) grade = "A-";
            else if (percentage >= 65) grade = "B+";
            else if (percentage >= 60) grade = "B";
            else if (percentage >= 55) grade = "B-";
            else if (percentage >= 50) grade = "C+";
            else if (percentage >= 45) grade = "C";
            else if (percentage >= 40) grade = "D";
            else grade = "F";

            student.Grade = grade; // Update the grade

            // Save changes to DB
            _context.SaveChanges();

            // Return updated data
            return Json(new { success = true, totalMarks = student.TotalMarks, percentage = percentage.ToString("F2"), grade = student.Grade });
        }

       
        

        private string CalculateGrade(double percentage)
        {
            if (percentage >= 80) return "A+";
            else if (percentage >= 75) return "A";
            else if (percentage >= 70) return "A-";
            else if (percentage >= 65) return "B+";
            else if (percentage >= 60) return "B";
            else if (percentage >= 55) return "B-";
            else if (percentage >= 50) return "C+";
            else if (percentage >= 45) return "C";
            else if (percentage >= 40) return "D";
            else return "F";
        }
        public IActionResult Index()
        {
            var teachers = _context.Teachers.ToList();
            return View(teachers);
        }

        [HttpGet]
        public IActionResult AddColumn(string subject, string className)
        {
            ViewBag.Subject = subject;
            ViewBag.ClassName = className;
            return View();
        }

        [HttpPost]
        public IActionResult SaveColumn(string subject, string className, string columnName, int maxMarks, double percentage)
        {
            var students = _context.Students
                .Where(s => s.SubjectName == subject && s.Class == className)
                .ToList();

            foreach (var student in students)
            {
                var marksData = !string.IsNullOrEmpty(student.MarksJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(student.MarksJson)
                    : new Dictionary<string, int>();

                if (!marksData.ContainsKey(columnName))
                {
                    marksData[columnName] = 0; // Default value
                }

                student.MarksJson = System.Text.Json.JsonSerializer.Serialize(marksData);
            }

            _context.SaveChanges();

            // Redirect back to ViewClass after updating
            return RedirectToAction("ViewClass", new { subject, className });
        }
        
        

    }
}
