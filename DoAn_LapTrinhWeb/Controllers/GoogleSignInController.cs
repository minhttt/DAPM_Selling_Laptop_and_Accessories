using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GoogleAuthentication.Services;
namespace DoAn_LapTrinhWeb.Controllers
{
    public class GoogleSignInController : Controller
    {
        // GET: GoogleSignIn
        public ActionResult Index()
        {
            return View();
        }

    }
}