using Employee_Mnagement_System.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Employee_Mnagement_System.Controllers
{
    public class EmployeeManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        private string connectionString;

        public EmployeeManagementController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<EmployeeModel> employeeList = new List<EmployeeModel>();
            const string Query = "select * from employee;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        EmployeeModel employee = new EmployeeModel();

                        employee.Id = (int) reader["Id"];
                        employee.FirstName = reader["FirstName"].ToString();
                        employee.LastName = reader["LastName"].ToString();
                        employee.EmailId = reader["EmailId"].ToString();
                        employee.ContactNo = reader["ContactNo"].ToString();
                        employee.Age = (int) reader["Age"];

                        employeeList.Add(employee);
                    }
                }
            }

            return View(employeeList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeModel employee)
        {

            const string Query = "Insert into employee (FirstName, LastName, EmailId, ContactNo, Age) values (@FirstName, @LastName, @EmailId, @ContactNo, @Age);";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                    command.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    command.Parameters.AddWithValue("@Age", employee.Age);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }



    }
}
