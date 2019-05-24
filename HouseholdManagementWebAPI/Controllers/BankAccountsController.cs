using AutoMapper;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            bankAccount.OwnerId = currentUserId;

            household.BankAccounts.Add(bankAccount);
            currentUser.BankAccounts.Add(bankAccount);

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

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.OwnerId == currentUserId && p.Household.HouseholdOwnerId == currentUserId && p.Id == bankAccountId);
            if (bankAccount == null)
            {
                return NotFound();
            }

            bankAccount = Mapper.Map<BankAccount>(formdata);

            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountBindingModel>(bankAccount);

            return Ok(model);
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete]
        [Route("bankAccountId")]
        public IHttpActionResult Delete(string bankAccountId)
        {
            if(bankAccountId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == bankAccountId);
            if(bankAccount == null)
            {
                return NotFound();
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == bankAccount.HouseholdId);
            if(household == null)
            {
                return NotFound();
            }

            bankAccount.Transactions.Clear();

            var allTransactions = DbContext.Transactions.Where(p => p.BankAccountId == bankAccountId);
            DbContext.Transactions.RemoveRange(bankAccount.Transactions);
            foreach(var transaction in allTransactions)
            {                
                DbContext.Transactions.Remove(transaction);
            }

            return Ok();
        }
    }
}
