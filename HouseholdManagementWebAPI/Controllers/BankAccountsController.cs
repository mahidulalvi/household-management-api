using AutoMapper;
using AutoMapper.QueryableExtensions;
using HouseholdManagementWebAPI.Models;
using HouseholdManagementWebAPI.Models.BindingModels;
using HouseholdManagementWebAPI.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdManagementWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/BankAccounts")]
    public class BankAccountsController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }

        public BankAccountsController()
        {
            DbContext = new ApplicationDbContext();
        }


        // GET: api/BankAccounts
        [HttpGet]
        [Route("{householdId}")]
        public IHttpActionResult Get(string householdId)
        {
            if(householdId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var result = DbContext.BankAccounts
                .Where(p => p.HouseholdId == householdId && p.Household.HouseholdMembers.Any(r => r.Id == currentUserId))
                .ProjectTo<BankAccountBindingModel>()
                .ToList();
            if(result == null)
            {
                return NotFound();
            }


            return Ok(result);
        }

        // GET: api/BankAccounts/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/BankAccounts
        [HttpPost]
        public IHttpActionResult Post(string householdId, BindingModelForCreatingBankAccount formdata)
        {
            if (householdId == null || !ModelState.Any() || !ModelState.IsValid)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdOwnerId == currentUserId);
            if (household == null)
            {
                return NotFound();
            }


            var bankAccount = Mapper.Map<BankAccount>(formdata);
            bankAccount.HouseholdId = household.Id;            

            household.BankAccounts.Add(bankAccount);

            DbContext.BankAccounts.Add(bankAccount);

            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountBindingModel>(bankAccount);

            var link = "abc/abc/abc";

            return Created(link, model);
        }

        // PUT: api/BankAccounts/5
        [HttpPut]
        [Route("{bankAccountId}")]
        public IHttpActionResult Put(string bankAccountId, BindingModelForCreatingBankAccount formdata)
        {
            if (bankAccountId == null || !ModelState.Any() || !ModelState.IsValid)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Household.HouseholdOwnerId == currentUserId && p.Id == bankAccountId);
            if (bankAccount == null)
            {
                return NotFound();
            }

            bankAccount = Mapper.Map<BankAccount>(formdata);

            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountBindingModel>(bankAccount);

            return Ok();
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete]
        [Route("{bankAccountId}")]
        public IHttpActionResult Delete(string bankAccountId)
        {
            if (bankAccountId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == bankAccountId && p.Household.HouseholdOwnerId == currentUserId);
            if (bankAccount == null)
            {
                return NotFound();
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == bankAccount.HouseholdId);
            if(household == null)
            {
                return NotFound();
            }

            DbContext.Transactions.RemoveRange(bankAccount.Transactions);

            DbContext.BankAccounts.Remove(bankAccount);

            DbContext.SaveChanges();

            return Ok();
        }
    }
}
