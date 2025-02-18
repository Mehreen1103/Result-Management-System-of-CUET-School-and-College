namespace CUETSchool.Models
{
    public class Dashboard
    {

        public StudentLogin Student { get; set; }
        public List<Dictionary<string, object>> Results { get; set; } // Allow dynamic columns



    }
}
