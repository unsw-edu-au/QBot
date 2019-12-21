using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Teams.Apps.QBot.Bot.utility;

namespace Microsoft.Teams.Apps.QBot.Bot.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("selectanswer")]
        public ActionResult SelectAnswer()
        {
            var payload = Request.QueryString["json"];

            ViewBag.Payload = HttpUtility.HtmlDecode(payload);
            return View();
        }
    }
}