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

        public IActionResult Edit(int? id)
        {

            EmployeeModel employeeModel = null;

            const string queryString = "SELECT * FROM employee WHERE Id = @Id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        employeeModel = new EmployeeModel();

                        // Id, FirstName, LastName, ContactNo, Age

                        employeeModel.Id = (int)reader["Id"];
                        employeeModel.FirstName = reader["FirstName"].ToString();
                        employeeModel.LastName = reader["LastName"].ToString();
                        employeeModel.ContactNo = reader["ContactNo"].ToString();
                        employeeModel.EmailId = reader["EmailId"].ToString();
                        employeeModel.Age = (int)reader["Age"];

                    }
                }
            }

            return View(employeeModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, EmployeeModel employee)
        {

            string queryString = "UPDATE employee SET FirstName = @FirstName, LastName = @LastName, ContactNo = @ContactNo, EmailId = @EmailId, Age = @Age WHERE Id = @Id;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(queryString, connection))
                {

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                    command.Parameters.AddWithValue("@Age", employee.Age);

                    connection.Open();

                    command.ExecuteNonQuery();

                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            string queryString = "DELETE FROM employee WHERE Id = @Id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Search(string Search)
        {
            List<EmployeeModel> employeeList = new List<EmployeeModel>();
            string Query = "SELECT * FROM employee WHERE Id LIKE @search OR FirstName LIKE @search OR LastName LIKE @search OR EmailId LIKE @search OR ContactNo LIKE @search OR Age LIKE @search;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@search", "%" + Search + "%");
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EmployeeModel employee = new EmployeeModel();

                        employee.Id = (int)reader["Id"];
                        employee.FirstName = reader["FirstName"].ToString();
                        employee.LastName = reader["LastName"].ToString();
                        employee.EmailId = reader["EmailId"].ToString();
                        employee.ContactNo = reader["ContactNo"].ToString();
                        employee.Age = (int)reader["Age"];

                        employeeList.Add(employee);
                    }
                }
            }
            return View("Search", employeeList);
        }

    }
}
