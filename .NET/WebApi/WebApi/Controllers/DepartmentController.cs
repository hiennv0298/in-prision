using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            var list = new List<Department>();
            SqlCommand command = new SqlCommand("select DepartmentId, DepartmentName from dbo.Department;", connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Department() { DepartmentId = (int)reader["DepartmentId"], DepartmentName = (string)reader["DepartmentName"] });
                }
            }

            connection.Close();
            return new JsonResult(list);
        }

        [HttpPost]
        public JsonResult Post(Department dep)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = new SqlCommand($"insert into dbo.Department values('{dep.DepartmentName}');", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult(dep.DepartmentName);
        }

        [HttpPut]
        public JsonResult Put(Department dep)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = new SqlCommand($"update dbo.Department set DepartmentName='{dep.DepartmentName}' where DepartmentId = '{dep.DepartmentId}';", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult(dep.DepartmentName);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = new SqlCommand($"delete from dbo.Department where DepartmentId = '{id}';", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult("Deleted");
        }
    }
}
