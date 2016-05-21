using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AntiCSRFSamples.Filters
{
    public class CustomValidateAntiForgeryToken : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ValidateAntiForgeryToken(filterContext.HttpContext.Request.Headers["VerificationToken"]);
            }
            catch (HttpAntiForgeryException ex)
            {
                throw new HttpAntiForgeryException("注意！您現在正在操作一個偽造的網頁！");
            }
        }

        private static void ValidateAntiForgeryToken(string verificationToken)
        {
            string cookieToken = "";
            string formToken = "";

            if (!string.IsNullOrEmpty(verificationToken))
            {
                string[] tokens = verificationToken.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            // 因為我用 AntiForgery Helper 幫我產生 Token，這邊一樣用 AntiForgery 來做 Validation。
            // 如果 Token 是自己 Hash 的，就自己另外寫檢查的邏輯。
            AntiForgery.Validate(cookieToken, formToken);
        }
    }
}