using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using AntiCSRFSamples.Models;

namespace AntiCSRFSamples.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(string source, int money, string target)
        {
            XDocument xdoc = XDocument.Load(Path.Combine(Server.MapPath("~/"), "Money.xml"));

            var targetElement = xdoc.Root.Elements().Where(e => e.Attribute("Name").Value.Equals(target)).Single();
            targetElement.Value = (int.Parse(targetElement.Value) + money).ToString();

            var sourceElement = xdoc.Root.Elements().Where(e => e.Attribute("Name").Value.Equals(source)).Single();
            sourceElement.Value = (int.Parse(sourceElement.Value) - money).ToString();

            xdoc.Save(Path.Combine(Server.MapPath("~/"), "Money.xml"));

            var balances = xdoc.Root.Elements().Select(e => new Deposit() { Name = e.Attribute("Name").Value, Balance = int.Parse(e.Value) });

            return Json(balances);
        }
    }
}