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
    [RoutePrefix("api/Households")]
    public class HouseholdsController : ApiController
    {

        public ApplicationDbContext DbContext { get; set; }

        public HouseholdsController()
        {
            DbContext = new ApplicationDbContext();
        }        

        // GET: api/Households/5
        [HttpGet]
        [Route("{id}", Name = "GetHouseholdById")]
        public IHttpActionResult Get(string id)
        {
            if(id == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();

            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.HouseholdMembers.Any(r => r.Id == currentUserId));
            if(household == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<HouseholdViewModelForFrontEnd>(household);

            return Ok(result);
        }


        [HttpGet]
        [Route("EditHousehold/{id}")]
        public IHttpActionResult GetHouseholdForEdit(string id)
        {
            if (id == null)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();

            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.HouseholdOwnerId == currentUserId);
            if (household == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<HouseholdViewModelForFrontEnd>(household);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetHouseholds", Name = "GetAllHouseholds")]
        public IHttpActionResult Get()
        {            
            var currentUserId = User.Identity.GetUserId();            

            var households = DbContext.Households.Where(p => p.HouseholdMembers.Any(r => r.Id == currentUserId)).ProjectTo<HouseholdViewModelForFrontEnd>().ToList();
            if (households == null)
            {
                return NotFound();
            }

            return Ok(households);
        }


        // POST: api/Households
        [HttpPost]
        public IHttpActionResult Post(HouseholdBindingModel formdata)
        {
            if (formdata == null || !ModelState.IsValid)
            {
                return BadRequest("Please provide the details of the household");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = Mapper.Map<Household>(formdata);
            
            household.HouseholdOwnerId = currentUserId;
            household.HouseholdMembers.Add(currentUser);            

            currentUser.OwnedHouseholds.Add(household);
            currentUser.JoinedHouseholds.Add(household);

            DbContext.Households.Add(household);
            DbContext.SaveChanges();

            var result = new HouseholdViewModelForFrontEnd
            {
                Name = household.Name,
                DateCreated = household.DateCreated,
                Description = household.Description,
                HouseholdOwner = new HouseholdMember
                {
                    UserName = currentUser.UserName,
                    Email = currentUser.Email
                },
                HouseholdMembers = household.HouseholdMembers.Select(member => new HouseholdMember
                {
                    UserName = member.UserName,
                    Email = member.Email
                }).ToList()
            };

            var link = Url.Link("GetHouseholdById", new { Id = household.Id });

            return Created(link, result);

        }

        // PUT: api/Households/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(string id, HouseholdBindingModel formdata)
        {
            if(id == null || formdata == null || !ModelState.IsValid)
            {
                return BadRequest("Please provide all the details");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.HouseholdOwnerId == currentUserId);
            if(household == null)
            {
                return NotFound();
            }
            
            Mapper.Map(formdata, household);
            household.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var result1 = Mapper.Map<HouseholdViewModelForFrontEnd>(household);

            var result = new HouseholdViewModelForFrontEnd
            {
                Id = household.Id,
                Name = household.Name,
                DateCreated = household.DateCreated,
                DateUpdated = household.DateUpdated,
                Description = household.Description,
                HouseholdOwner = new HouseholdMember
                {
                    UserName = household.HouseholdOwner.UserName,
                    Email = household.HouseholdOwner.Email
                },
                HouseholdMembers = household.HouseholdMembers.Select(member => new HouseholdMember
                {
                    UserName = member.UserName,
                    Email = member.Email
                }).ToList()                
            };

            return Ok(result1);
        }

        // DELETE: api/Households/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            var currentUserId = User.Identity.GetUserId();

            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.HouseholdOwnerId == currentUserId);
            if(household == null)
            {
                return NotFound();
            }            

            //household.HouseholdMembers.Clear();
            DbContext.Households.Remove(household);
            DbContext.SaveChanges();

            return Ok();
        }
    }
}
