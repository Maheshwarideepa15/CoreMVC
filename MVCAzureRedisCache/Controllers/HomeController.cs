using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCAzureRedisCache.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MVCAzureRedisCache.Controllers
{
    public class HomeController : Controller
    {

        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
        string cacheConnectionString = System.Configuration.ConfigurationManager.AppSettings["RedisCachekey"];
        EmployeeDataContext mydbcontext;

        [HttpPost]
        public ActionResult AddEmployee(Employee model)
        {
            mydbcontext = new EmployeeDataContext();
            mydbcontext.Employee.Add(model);
            mydbcontext.SaveChanges();

            return View();
        }
        [HttpGet]
        public ActionResult AddEmployee()
        {

            return View();
        }

        public ActionResult Employee()
        {
            var connect = ConnectionMultiplexer.Connect(cacheConnectionString);
            mydbcontext = new EmployeeDataContext();
            IDatabase Rediscache = connect.GetDatabase();
            if (string.IsNullOrEmpty(Rediscache.StringGet("EmployeeDetails")))
            {
                var liemp = mydbcontext.Employee.ToList();
                var emplist = JsonConvert.SerializeObject(liemp);

                Rediscache.StringSet("EmployeeDetails", emplist, TimeSpan.FromMinutes(2));
                return View(liemp);

            }
            else
            {

                var detail = JsonConvert.DeserializeObject<List<Employee>>(Rediscache.StringGet("EmployeeDetails"));
                return View(detail);

            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
           
            return View();
        }

        
    }
}