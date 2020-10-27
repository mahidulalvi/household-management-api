using AutoMapper;
using AutoMapper.QueryableExtensions;
using HouseholdManagementAPI.Models;
using HouseholdManagementAPI.Models.BindingModels;
using HouseholdManagementAPI.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdManagementAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Transactions")]
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }

        public TransactionsController()
        {
            DbContext = new ApplicationDbContext();
        }


        [HttpGet]
        [Route("IsUserTransactionOwner/{transactionId}")]
        public IHttpActionResult IsUserTransactionOwner(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("transactionId required");
            }

            var currentUserId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == transactionId && (p.TransactionOwnerId == currentUserId || p.BankAccount.Household.HouseholdOwnerId == currentUserId));
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok();
        }


        // GET: api/Transactions
        [HttpGet]
        [Route("{householdId}/{bankAccountId}")]
        public IHttpActionResult Get(string householdId, string bankAccountId)
        {
            if(householdId == null)
            {
                return BadRequest("householdId is required");
            }
            if (bankAccountId == null)
            {
                return BadRequest("bankAccountId is required");
            }

            var currentUserId = User.Identity.GetUserId();            

            var result = DbContext.Transactions
                    .Where(p => p.BankAccountId == bankAccountId && p.BankAccount.HouseholdId == householdId && p.BankAccount.Household.HouseholdMembers.Any(r => r.Id == currentUserId))
                    .ProjectTo<TransactionViewModel>()
                    .ToList();
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{transactionId}", Name = "GetTransactionById")]
        public IHttpActionResult GetATransaction(string transactionId)
        {
            if(transactionId == null)
            {
                return BadRequest("transactionId is required");
            }

            var currentUserId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions
                    .FirstOrDefault(p => p.Id == transactionId && p.BankAccount.Household.HouseholdMembers.Any(r => r.Id == currentUserId));
            if(transaction == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<TransactionViewModel>(transaction);

            return Ok(result);
        }





        //Get transaction for edit
        [HttpGet]
        [Route("Edit/{transactionId}", Name = "GetTransactionForEdit")]
        public IHttpActionResult GetTransactionForEdit(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("transactionId is required");
            }

            var currentUserId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions
                    .FirstOrDefault(p => p.Id == transactionId && (p.BankAccount.Household.HouseholdOwnerId == currentUserId || p.TransactionOwnerId == currentUserId));
            if (transaction == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<BindingModelForCreatingTransaction>(transaction);

            return Ok(result);
        }

        // POST: api/Transactions
        [HttpPost]
        [Route("{householdId}/{bankAccountId}")]
        public IHttpActionResult Post(string householdId, string bankAccountId, BindingModelForCreatingTransaction formdata)
        {
            if (householdId == null)
            {
                return BadRequest("householdId is required");
            }
            if (bankAccountId == null)
            {
                return BadRequest("bankAccountId is required");
            }
            if (formdata == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdMembers.Any(r => r.Id == currentUserId));
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == bankAccountId && p.HouseholdId == householdId);
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formdata.CategoryId && p.HouseholdId == householdId);

            if (household == null || bankAccount == null || category == null)
            {
                return NotFound();
            }

            var transaction = Mapper.Map<Transaction>(formdata);
            transaction.BankAccountId = bankAccountId;
            transaction.IsTransactionVoid = false;
            transaction.TransactionOwnerId = currentUserId;

            bankAccount.Balance += transaction.Amount;
            bankAccount.Transactions.Add(transaction);
            category.Transactions.Add(transaction);
            currentUser.Transactions.Add(transaction);

            DbContext.Transactions.Add(transaction);

            DbContext.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);
            var link = Url.Link("GetTransactionById", new { transactionId = transaction.Id });

            return Created(link, result);
        }


        // POST: api/Transactions
        [HttpPost]
        [Route("{householdId}")]
        public IHttpActionResult Post(string householdId, AlternativeBindingModelForCreatingTransaction formdata)
        {
            if (householdId == null)
            {
                return BadRequest("householdId is required");
            }
            if (formdata == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdMembers.Any(r => r.Id == currentUserId));
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formdata.BankAccountId && p.HouseholdId == householdId);
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formdata.CategoryId && p.HouseholdId == householdId);

            if (household == null || bankAccount == null || category == null)
            {
                return NotFound();
            }

            var transaction = Mapper.Map<Transaction>(formdata);
            transaction.IsTransactionVoid = false;
            transaction.TransactionOwnerId = currentUserId;

            bankAccount.Balance += transaction.Amount;
            bankAccount.Transactions.Add(transaction);
            category.Transactions.Add(transaction);
            currentUser.Transactions.Add(transaction);

            DbContext.Transactions.Add(transaction);

            DbContext.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);
            var link = Url.Link("GetTransactionById", new { transactionId = transaction.Id });

            return Created(link, result);
        }



        // PUT: api/Transactions/5
        [HttpPut]
        [Route("{transactionId}")]
        public IHttpActionResult Put(string transactionId, BindingModelForCreatingTransaction formdata)
        {
            if (transactionId == null)
            {
                return BadRequest("transactionId is required");
            }
            if (formdata == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == transactionId && ((p.TransactionOwnerId == currentUserId && p.BankAccount.Household.HouseholdMembers.Any(r => r.Id == currentUserId)) || p.BankAccount.Household.HouseholdOwnerId == currentUserId));
            if (transaction == null)
            {
                return NotFound();
            }

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formdata.CategoryId && p.HouseholdId == transaction.BankAccount.HouseholdId);
            if(category == null)
            {
                return NotFound();
            }

            if (!transaction.IsTransactionVoid)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
                transaction.BankAccount.Balance += formdata.Amount;
            }

            Mapper.Map(formdata, transaction);
            transaction.DateUpdated = DateTime.Now;            

            DbContext.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);

            var link = Url.Link("GetTransactionById", new { transactionId = transaction.Id });

            return Created(link, result);
        }


        [HttpPut]
        [Route("ToggleVoidTransaction/{transactionId}")]
        public IHttpActionResult ToggleVoidTransaction(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == transactionId && ((p.TransactionOwnerId == currentUserId && p.BankAccount.Household.HouseholdMembers.Any(r => r.Id == currentUserId)) || p.BankAccount.Household.HouseholdOwnerId == currentUserId));
            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.IsTransactionVoid == false)
            {
                transaction.IsTransactionVoid = true;
                transaction.BankAccount.Balance -= transaction.Amount;
            }
            else
            {
                transaction.IsTransactionVoid = false;
                transaction.BankAccount.Balance += transaction.Amount;
            }
            transaction.DateUpdated = DateTime.Now;
            

            DbContext.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);

            var link = Url.Link("GetTransactionById", new { transactionId = transaction.Id });

            return Created(link, result);
        }

        // DELETE: api/Transactions/5
        [HttpDelete]
        [Route("{transactionId}")]
        public IHttpActionResult Delete(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == transactionId && ((p.TransactionOwnerId == currentUserId && p.BankAccount.Household.HouseholdMembers.Any(r => r.Id == currentUserId)) || p.BankAccount.Household.HouseholdOwnerId == currentUserId));
            if (transaction == null)
            {
                return NotFound();
            }

            if(transaction.IsTransactionVoid == false)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
            }

            DbContext.Transactions.Remove(transaction);

            DbContext.SaveChanges();

            return Ok();
        }
    }
}
