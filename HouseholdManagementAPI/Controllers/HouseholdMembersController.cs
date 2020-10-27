using AutoMapper;
using HouseholdManagementAPI.Models;
using HouseholdManagementAPI.Models.BindingModels;
using HouseholdManagementAPI.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HouseholdManagementAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/HouseholdMembers")]
    public class HouseholdMembersController : ApiController
    {
        public ApplicationDbContext DbContext { get; set; }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public HouseholdMembersController()
        {
            DbContext = new ApplicationDbContext();
        }

        
        [HttpGet]
        [Route("{householdId}")]
        public IHttpActionResult Get(string householdId)
        {
            if (householdId == null)
            {
                return BadRequest("Please provide required information");
            }

            var currentUserId = User.Identity.GetUserId();

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdMembers.Any(q => q.Id == currentUserId));
            if (household == null)
            {
                return NotFound();
            }

            var result = household.HouseholdMembers
                .Select(member => Mapper.Map<HouseholdMember>(member))
                .ToList();                        

            return Ok(result);
        }                        

        [HttpPost]
        [Route("{householdId}/InviteMember")]
        public async Task<IHttpActionResult> InviteMemberByEmail(string householdId, InviteHouseholdMemberBindingModel formdata)
        {
            if (householdId == null || formdata == null)
            {
                return BadRequest("Please provide all the details!");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdOwnerId == currentUserId);
            if (household == null)
            {
                return NotFound();
            }

            var userToBeEmailed = DbContext.Users.FirstOrDefault(p => p.Email == formdata.Email && p.Id !=  currentUserId && !p.JoinedHouseholds.Any(r => r.Id == householdId));
            if (userToBeEmailed == null)
            {
                return NotFound();
            }

            var invite = new Invite();
            invite.HouseholdId = householdId;
            invite.InviteeId = currentUserId;
            invite.InvitedId = userToBeEmailed.Id;

            currentUser.SentHouseholdInvites.Add(invite);
            userToBeEmailed.ReceivedHouseholdInvites.Add(invite);
            household.ActiveInvites.Add(invite);

            DbContext.Invites.Add(invite);

            DbContext.SaveChanges();

            await SendEmail(userToBeEmailed.Id, household.Name, currentUser.UserName, "Invite", invite.Id);

            return Ok();
        }


        [HttpPut]
        [Route("JoinHousehold/{inviteId}")]
        public IHttpActionResult JoinHouseholdFromInvitation(string inviteId)
        {
            if(inviteId == null)
            {
                return BadRequest("Please provide all the details");
            }
            
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);            

            var invite = DbContext.Invites.FirstOrDefault(p => p.Id == inviteId && p.InvitedId ==  currentUserId);
            if (invite == null)
            {
                return NotFound();
            }            

            var household = DbContext.Households.FirstOrDefault(p => p.Id == invite.HouseholdId);
            if(household == null)
            {
                return NotFound();
            }

            if(invite.IsInvitationValid(DateTime.Now))
            {
                household.HouseholdMembers.Add(currentUser);
                currentUser.JoinedHouseholds.Add(household);
                DbContext.Invites.Remove(invite);

                DbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
        }


        [Authorize]
        private async Task<IHttpActionResult> SendEmail(string userId, string householdName, string invitee, string operation, string inviteId)
        {

            if (operation == "Invite")
            {                
                var encodedInviteId = HttpUtility.UrlEncode(inviteId);
                await UserManager.SendEmailAsync(userId, $"Invitation from {invitee}", $"Greetings! {invitee} invited you to the household: {householdName}. Click <a href='http://localhost:50948/HouseholdApiHouseholdMembers/JoinHousehold?inviteId={encodedInviteId}&householdName={householdName}'>Here</a> to accept/decline. This is the id of the invitation {inviteId}");
            }
            else
            {
                return BadRequest("Unknown parameter");
            }

            return Ok();
        }





        [HttpPut]
        [Route("LeaveHousehold/{householdId}")]
        public IHttpActionResult LeaveHouseHold(string householdId)
        {
            if(householdId == null)
            {
                return BadRequest("Please provide all the details!");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdMembers.Any(q => q.Id == currentUserId));
            if(household == null)
            {
                return NotFound();
            }
            if(household.HouseholdOwnerId == currentUserId)
            {
                return BadRequest("Owner of the household cannot leave! Deletion is necessary");
            }

            household.HouseholdMembers.Remove(currentUser);

            DbContext.SaveChanges();

            return Ok("Left household successfully");
        }
    }
}
