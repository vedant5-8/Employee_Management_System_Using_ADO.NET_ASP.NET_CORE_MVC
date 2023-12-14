using System.ComponentModel.DataAnnotations;

namespace Employee_Mnagement_System.Models
{
    public class EmployeeModel
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailId { get; set; }

        public string ContactNo { get; set; }

        public string Age { get; set; }

        public string ProfileImage { get; set; }

        public IFormFile imageFile { get; set; }

    }
}
