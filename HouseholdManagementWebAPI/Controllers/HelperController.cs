using HouseholdManagementWebAPI.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManagementWebAPI.Controllers
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



        //[Authorize]
        //private async Task<> SendEmail(string userId, string ticketTitle, string operation)
        //{
        //    var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);

        //    if (user == null)
        //    {
        //        RedirectToAction("All Tickets", "Tickets");
        //    }

        //    if (operation == "Invite")
        //    {
        //        await UserManager.SendEmailAsync(userId, $"Invitation from {ticketTitle}", $"Greetings! You have been invited to household: {ticketTitle}");
        //    }
        //    //else if (operation == "Remove")
        //    //{
        //    //    await UserManager.SendEmailAsync(userId, $"Unassigned from Ticket {ticketTitle}", $"You have been unassigned from ticket {ticketTitle}");
        //    //}
        //    //else if (operation == "Modify")
        //    //{
        //    //    await UserManager.SendEmailAsync(userId, $"Ticket {ticketTitle} has been modified", $"An user just modified ticket {ticketTitle}");
        //    //}
        //    else
        //    {
        //        RedirectToAction("AllTickets", "Tickets");
        //    }

        //    return RedirectToAction("AllTickets", "Tickets");
        //}
    }
}