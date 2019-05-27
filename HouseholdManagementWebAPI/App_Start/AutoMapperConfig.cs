using AutoMapper;
using HouseholdManagementWebAPI.Models;
using HouseholdManagementWebAPI.Models.BindingModels;
using HouseholdManagementWebAPI.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.App_Start
{
    public class AutoMapperConfig
    {
        public static void Init()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Household, HouseholdBindingModel>().ReverseMap();
                config.CreateMap<HouseholdViewModelForFrontEnd, Household>()
                    /*.ForMember(t => t.HouseholdOwner, source => source.MapFrom(property => property.HouseholdOwner))*/.ReverseMap();
                config.CreateMap<ApplicationUser, HouseholdMember>().ReverseMap();
                config.CreateMap<InviteBindingModel, Invite>().ReverseMap();
                config.CreateMap<Category, CategoryBindingModel>().ReverseMap();
                config.CreateMap<BindingModelForCreatingCategory, Category>().ReverseMap();
                config.CreateMap<BankAccount, BindingModelForCreatingBankAccount>().ReverseMap();
                config.CreateMap<BankAccountBindingModel, BankAccount>().ReverseMap();
                config.CreateMap<Transaction, BindingModelForCreatingTransaction>().ReverseMap();
                config.CreateMap<Transaction, AlternativeBindingModelForCreatingTransaction>().ReverseMap();
                config.CreateMap<TransactionViewModel, Transaction>().ReverseMap();
            });
        }
    }
}