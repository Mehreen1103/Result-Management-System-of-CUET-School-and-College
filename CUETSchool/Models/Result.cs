using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CUETSchool.Models
{
    public class Result
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string SubjectName { get; set; }
        public string Class { get; set; }

        // Store all marks dynamically as JSON (CQ, MCQ, Practical, etc.)
        public string MarksJson { get; set; } // Stores {"CQ": 10, "MCQ": 15, "Practical": 20}

        public int FullMarks { get; set; }
        public int? TotalMarks { get; set; }
        public string? Grade { get; set; }
    }
}
