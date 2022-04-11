using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            var list = new List<Employee>();
            SqlCommand command = 
                new SqlCommand("select EmployeeId, EmployeeName, Department, " +
                "convert(nvarchar(max),DateofJoining,120) as DateOfJoining," +
                " PhotoFileName from dbo.Employee;", connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Employee() {
                        EmployeeId = (int)reader["EmployeeId"],
                        EmployeeName = (string)reader["EmployeeName"],
                        DateOfJoining = (string)reader["DateOfJoining"],
                        Department = (string)reader["Department"],
                        PhotoFileName = (string)reader["PhotoFileName"]
                    });
                }
            }

            connection.Close();
            return new JsonResult(list);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = 
                new SqlCommand($"insert into dbo.Employee (EmployeeName, Department, DateOfJoining, PhotoFileName)" +
                $"values(N'{emp.EmployeeName}', N'{emp.Department}',N'{emp.DateOfJoining}', N'{emp.PhotoFileName}' );", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult(emp.EmployeeName);
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = new SqlCommand($"update dbo.Employee" +
                $" set EmployeeName= N'{emp.EmployeeName}'" +
                $", Department = N'{emp.Department}'" +
                $", DateOfJoining = N'{emp.DateOfJoining}'" +
                $", PhotoFileName = N'{emp.PhotoFileName}' where EmployeeId = '{emp.EmployeeId}';", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult(emp.EmployeeName);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon"));
            connection.Open();
            SqlCommand command = new SqlCommand($"delete from dbo.Employee where EmployeeId = '{id}';", connection);
            command.ExecuteReader();
            connection.Close();
            return new JsonResult("Deleted");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.jpg");
            }
        }

        [Route("GetAllDepartmentNames")]
        public JsonResult GetAllDepartmentNames()
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
    }
}
