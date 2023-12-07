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

        public string UploadImage(IFormFile imageFile)
        {
            try
            {
                string uniqueFileName = null;

                if (imageFile != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                }
                else
                {
                    Console.WriteLine("Image File is Null");
                }
                return uniqueFileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeModel employee)
        {
            employee.ProfileImage = UploadImage(employee.imageFile);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                const string Query = "Insert into employee (FirstName, LastName, EmailId, ContactNo, Age, ProfileImage) values (@FirstName, @LastName, @EmailId, @ContactNo, @Age, @ProfileImage);";

                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                    command.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    command.Parameters.AddWithValue("@Age", employee.Age);
                    command.Parameters.AddWithValue("@ProfileImage", employee.ProfileImage);

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
                        employeeModel.ProfileImage = reader["ProfileImage"].ToString();

                        // Add the image file path to the model
                        employeeModel.ProfileImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", employeeModel.ProfileImage);

                    }
                }
            }

            return View(employeeModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, EmployeeModel employee)
        {

            string existingImage = null;

            // fetch the old image name
            using (MySqlConnection connection1 = new MySqlConnection(connectionString))
            {
                const string queryString1 = "SELECT ProfileImage FROM employee WHERE Id = @Id";

                using (MySqlCommand command = new MySqlCommand(queryString1, connection1))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection1.Open();
                    existingImage = command.ExecuteScalar() as string;
                }
            }

            // Upload the new image and get the new image name
            string newImage = UploadImage(employee.imageFile);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                string queryString = "UPDATE employee SET FirstName = @FirstName, LastName = @LastName, ContactNo = @ContactNo, EmailId = @EmailId, Age = @Age, ProfileImage = @ProfileImage WHERE Id = @Id;";

                using (MySqlCommand command = new MySqlCommand(queryString, connection))
                {

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                    command.Parameters.AddWithValue("@Age", employee.Age);
                    command.Parameters.AddWithValue("@ProfileImage", employee.ProfileImage);

                    connection.Open();

                    command.ExecuteNonQuery();

                }
            }

            // Delete the old image file if it exists and is different from the new image

            if (existingImage != null && existingImage != newImage)
            {
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", existingImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
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

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string Search)
        {
            List<EmployeeModel> employeeList = new List<EmployeeModel>();
            string Query = "SELECT * FROM employee WHERE Id = @search OR FirstName LIKE @search OR LastName LIKE @search OR EmailId LIKE @search OR ContactNo = @search OR Age LIKE @search;";

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
            return View("Index", employeeList);
        }

    }
}
