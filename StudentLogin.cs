using System.ComponentModel.DataAnnotations;

namespace CUETSchool.Models
{
    public class StudentLogin
    {
        [Required]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int Class { get; set; }

        [Required]
        [Display(Name = "Student ID")]
        public int StudentId { get; set; }
    }
}
