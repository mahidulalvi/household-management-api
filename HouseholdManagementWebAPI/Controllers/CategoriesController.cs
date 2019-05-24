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
    [RoutePrefix("api/Categories")]
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext DbContext { get; set; }
        public CategoriesController()
        {
            DbContext = new ApplicationDbContext();
        }


        // GET: api/Categories
        [HttpGet]
        [Route("{householdId}/Categories", Name = "HouseholdCategories")]
        public IHttpActionResult Get(string householdId)
        {
            if(householdId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();            

            var result = DbContext.Categories
                .Where(p => p.HouseholdId == householdId && p.Household.HouseholdMembers.Any(r => r.Id == currentUserId))     
                .ProjectTo<CategoryBindingModel>()                                
                .ToList();

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/Categories
        [Route("CreateCategory/{householdId}")]
        public IHttpActionResult Post(string householdId, BindingModelForCreatingCategory formdata)
        {
            if(householdId == null || formdata == null || !ModelState.IsValid)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();

            var household = DbContext.Households.FirstOrDefault(p => p.Id == householdId && p.HouseholdOwnerId == currentUserId);
            if(household == null)
            {
                return NotFound();
            }


            var category = Mapper.Map<Category>(formdata);            

            category.HouseholdId = householdId;              

            household.Categories.Add(category);

            DbContext.Categories.Add(category);

            DbContext.SaveChanges();

            var result = Mapper.Map<CategoryBindingModel>(category);
            result.HouseholdName = household.Name;
             
            var link = Url.Link("HouseholdCategories", new { householdId = householdId });

            return Created(link, result);
        }

        // PUT: api/Categories/5
        [Route("EditCategory/{categoryId}")]
        public IHttpActionResult Put(string categoryId, BindingModelForCreatingCategory formdata)
        {
            if (categoryId == null || formdata == null || !ModelState.IsValid)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId && p.Household.HouseholdOwnerId == currentUserId);
            if(category == null)
            {
                return NotFound();
            }

            Mapper.Map(formdata, category);            
            category.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            return Ok();
        }

        // DELETE: api/Categories/5
        [Route("DeleteCategory/{categoryId}")]
        public IHttpActionResult Delete(string categoryId)
        {
            if(categoryId == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId && p.Household.HouseholdOwnerId == currentUserId);
            if(category == null)
            {
                return NotFound();
            }            

            DbContext.Categories.Remove(category);

            DbContext.SaveChanges();

            return Ok();
        }
    }
}
