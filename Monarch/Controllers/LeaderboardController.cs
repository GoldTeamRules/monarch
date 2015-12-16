using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Monarch.Controllers
{
    public class LeaderboardController : Controller
    {
        // GET: Leaderboard
        public ActionResult Index()
        {
            return View();
        }
    }
}