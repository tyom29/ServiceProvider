using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceProvider.Controllers
{

    public class HomeController : Controller
    {
        private readonly ICustomerService _service;
        public HomeController(ICustomerService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Index(string mail)
        {
            var a = _service.GetCustomer(mail);
            return View(a);
        }

        [HttpGet]
        public ActionResult TermsAndConditions()
        {
            return View();
        }
    }
}