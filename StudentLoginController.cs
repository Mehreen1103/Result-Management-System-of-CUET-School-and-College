using Microsoft.AspNetCore.Mvc;
using CUETSchool.Models;
using CUETSchool.Data;

namespace CUETSchool.Controllers
{
    public class StudentLoginController : Controller
    {
        private readonly SchoolContext _context;

        public StudentLoginController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Shared/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(StudentLogin model)
        {
            if (ModelState.IsValid)
            {
                // Check if the student already exists in the database using Class and StudentId
                var student = _context.StudentLogins.FirstOrDefault(s => s.Class == model.Class && s.StudentId == model.StudentId);

                if (student == null)
                {
                    // Add the student to the database if not already present
                    _context.StudentLogins.Add(model);
                    await _context.SaveChangesAsync();
                    student = model; // Set the added student as the current one
                    ViewBag.Message = "New student added and logged in!";
                }
                else
                {
                    ViewBag.Message = "Login successful!";
                }

                // Store the student's Class and StudentId in TempData
                TempData["StudentClass"] = student.Class;
                TempData["StudentId"] = student.StudentId;

                return RedirectToAction("Dashboard"); // Redirect to Dashboard
            }

            // If ModelState is invalid
            ViewBag.ErrorMessage = "Invalid input. Please check the entered data.";
            return View(model);
        }

        public IActionResult Dashboard()
        {
            // Check if TempData contains the required keys
            if (TempData["StudentClass"] == null || TempData["StudentId"] == null)
            {
                return RedirectToAction("Login");
            }

            // Retrieve Class and StudentId from TempData
            int studentClass = (int)TempData["StudentClass"];
            int studentId = (int)TempData["StudentId"];
            TempData.Keep("StudentClass");
            TempData.Keep("StudentId");

            // Fetch student details using Class and StudentId
            var student = _context.StudentLogins.FirstOrDefault(s => s.Class == studentClass && s.StudentId == studentId);

            if (student == null)
            {
                return RedirectToAction("Login");
            }

            // Fetch results for the student
            //var results = _context.Results
            //    .Where(r => r.StudentId == studentId && r.Student.Class == studentClass)
            //    .Select(r => new Result
            //    {
            //        SubjectName = r.SubjectName,
            //        CQ = r.CQ,
            //        MCQ = r.MCQ,
            //        Practical = r.Practical,
            //        TotalMarks = r.TotalMarks,
            //        Grade = r.Grade
            //    }).ToList();
            var results = _context.Results
.Where(r => r.StudentId == studentId && r.Class == studentClass)
.Select(r => new Result
{
  SubjectName = r.SubjectName,
  CQ = r.CQ,
  MCQ = r.MCQ,
  Practical = r.Practical,
  TotalMarks = r.TotalMarks,
  Grade = r.Grade
}).ToList();


            var dashboardData = new Dashboard
            {
                Student = student,
                Results = results
            };

            return View("~/Views/Student/Dashboard.cshtml", dashboardData);
        }
    }
}
