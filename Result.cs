namespace CUETSchool.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        // Foreign key to StudentLogin
        public int Class { get; set; }
        public string SubjectName { get; set; }
        public int CQ { get; set; } // Creative Questions Marks
        public int MCQ { get; set; } // MCQ Marks
        public int Practical { get; set; } // Practical Marks
        public int TotalMarks { get; set; }
        public string Grade { get; set; }

        public StudentLogin Student { get; set; } // Navigation property
    }
}
