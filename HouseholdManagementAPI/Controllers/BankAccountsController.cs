using AutoMapper;
using AutoMapper.QueryableExtensions;
using HouseholdManagementAPI.Models;
using HouseholdManagementAPI.Models.BindingModels;
using HouseholdManagementAPI.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Http;

namespace HouseholdManagementAPI.Controllers
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

        [HttpGet]
        [Route("{householdId}/{bankAccountId}", Name = "GetBankAccountById")]
        public IHttpActionResult GetBankAccount(string householdId, string bankAccountId)
        {
            if (householdId == null || bankAccountId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.HouseholdId == householdId && p.Id == bankAccountId && p.Household.HouseholdMembers.Any(r => r.Id == currentUserId));    
            if (bankAccount == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<BankAccountBindingModel>(bankAccount);

            return Ok(result);
        }



        [HttpGet]
        [Route("Edit/{householdId}/{bankAccountId}", Name = "GetBankAccountForEdit")]
        public IHttpActionResult GetBankAccountForEdit(string householdId, string bankAccountId)
        {
            if (householdId == null || bankAccountId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.HouseholdId == householdId && p.Id == bankAccountId && p.Household.HouseholdOwnerId == currentUserId);
            if (bankAccount == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<BankAccountBindingModel>(bankAccount);

            return Ok(result);
        }


        // GET: api/BankAccounts/5
        [HttpPut]
        [Route("{bankAccountId}/CalculateAccountBalance")]
        public IHttpActionResult CalculateAccountBalance(string bankAccountId)
        {
            var currentUserId = User.Identity.GetUserId();

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == bankAccountId && p.Household.HouseholdOwnerId == currentUserId);
            if(bankAccount == null)
            {
                return BadRequest("No accounts on your name were found that matches this account number");
            }

            bankAccount.Balance = DbContext.Transactions
                    .Where(p => p.BankAccountId == bankAccountId && p.BankAccount.Household.HouseholdOwnerId == currentUserId && p.IsTransactionVoid == false)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

            DbContext.SaveChanges();

            var result = Mapper.Map<BankAccountBindingModel>(bankAccount);

            return Ok(result);
        }

        // POST: api/BankAccounts
        [HttpPost]
        [Route("{householdId}")]
        public IHttpActionResult Post(string householdId, BindingModelForCreatingBankAccount formdata)
        {
            if (householdId == null || formdata == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdOwnerId == currentUserId);
            if (household == null)
            {
                return NotFound();
            }


            var bankAccount = Mapper.Map<BankAccount>(formdata);
            bankAccount.HouseholdId = householdId;            

            household.BankAccounts.Add(bankAccount);

            DbContext.BankAccounts.Add(bankAccount);

            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountBindingModel>(bankAccount);

            var link = Url.Link("GetBankAccountById", new { bankAccountId = bankAccount.Id, householdId = householdId});

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
            
            Mapper.Map(formdata, bankAccount);
            bankAccount.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var model = Mapper.Map<BankAccountBindingModel>(bankAccount);

            var link = Url.Link("GetBankAccountById", new { bankAccountId = bankAccount.Id, householdId = bankAccount.HouseholdId });

            return Created(link, model);
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete]
        [Route("{bankAccountId}")]
        public IHttpActionResult Delete(string bankAccountId)
        {
            if(bankAccountId == null)
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
