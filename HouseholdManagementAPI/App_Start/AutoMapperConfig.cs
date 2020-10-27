using AutoMapper;
using HouseholdManagementAPI.Models;
using HouseholdManagementAPI.Models.BindingModels;
using HouseholdManagementAPI.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementAPI.App_Start
{
    public class AutoMapperConfig
    {
        public static void Init()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Household, HouseholdBindingModel>().ReverseMap();
                config.CreateMap<HouseholdViewModelForFrontEnd, Household>().ReverseMap();
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