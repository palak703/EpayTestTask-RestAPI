using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EpayCodingTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private static List<Customer> customers = new List<Customer>();

        string rootPath = Directory.GetCurrentDirectory();

        private static string dataFilePath;


        public CustomerController()
        {
            dataFilePath = Path.Combine(rootPath, "Data", "customer.json");
            LoadCustomers();
        }

        [HttpPost]
        public IActionResult PostCustomers([FromBody] List<Customer> newCustomers)
        {
            if (newCustomers == null || newCustomers.Count == 0)
            {
                return BadRequest("No customers provided.");
            }

            foreach (var customer in newCustomers)
            {
                if (string.IsNullOrWhiteSpace(customer.FirstName) ||
                    string.IsNullOrWhiteSpace(customer.LastName) ||
                    customer.Age > 18 ||
                    customers.Any(c => c.Id == customer.Id))
                {
                    return BadRequest($"Invalid customer: {customer}");
                }

                InsertCustomer(customer);
                SaveCustomers();

            }

            SaveCustomers();

            return Ok("Customers added successfully.");
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(customers);
        }

        private void InsertCustomer(Customer newCustomer)
        {
            int index = 0;

            while (index < customers.Count &&
                   string.Compare(customers[index].LastName, newCustomer.LastName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                index++;
            }

            while (index < customers.Count &&
                   string.Compare(customers[index].LastName, newCustomer.LastName, StringComparison.OrdinalIgnoreCase) == 0 &&
                   string.Compare(customers[index].FirstName, newCustomer.FirstName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                index++;
            }

            customers.Insert(index, newCustomer);
        }

        private static void LoadCustomers()
        {
            if (System.IO.File.Exists(dataFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(dataFilePath);
                customers = JsonConvert.DeserializeObject<List<Customer>>(jsonData);
            }
        }

        private static void SaveCustomers()
        {
            var jsonData = JsonConvert.SerializeObject(customers);
            System.IO.File.WriteAllText(dataFilePath, jsonData);
        }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return $"{{ firstName: '{FirstName}', lastName: '{LastName}', age: {Age}, id: {Id} }}";
        }
    }
}
