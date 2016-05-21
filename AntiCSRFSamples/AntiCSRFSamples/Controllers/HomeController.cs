using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using AntiCSRFSamples.Filters;
using AntiCSRFSamples.Models;

namespace AntiCSRFSamples.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // 這邊我用 AntiForgery 這個 Helper 來幫我產生 Token。
            // 我們可以自己撰寫自己想要的邏輯，用 MD5、SHAxxx…等方式產生一個 Hash 過的 Token。
            ViewBag.VerificationToken = GetAntiForgeryToken();

            return View();
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
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

        private static string GetAntiForgeryToken()
        {
            string cookieToken;
            string formToken;

            // 這邊我用 AntiForgery 這個 Helper 來幫我產生 Token。
            // 我們也可以自己撰寫自己想要的邏輯，用 MD5、SHAxxx…等方式產生一個 Hash 過的 Token。
            AntiForgery.GetTokens(null, out cookieToken, out formToken);

            return string.Concat(cookieToken, ":", formToken);
        }
    }
}