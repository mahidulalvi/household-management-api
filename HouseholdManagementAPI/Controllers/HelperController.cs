using HouseholdManagementAPI.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManagementAPI.Controllers
{
    public class HelperController : Controller
    {
        public ApplicationDbContext DbContext { get; set; }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public HelperController()
        {
            DbContext = new ApplicationDbContext();
        }

        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }
    }
}