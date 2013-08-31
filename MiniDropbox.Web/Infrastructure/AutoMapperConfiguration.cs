using System.Collections.Generic;
using AutoMapper;
using MiniDropbox.Domain;
using MiniDropbox.Web.Models;
using NHibernate.Hql.Ast.ANTLR;
using Ninject.Modules;

namespace MiniDropbox.Web.Infrastructure
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<AccountInputModel, Account>();
            Mapper.CreateMap<Account, AccountInputModel>();
            Mapper.CreateMap<Account, UpdatePerfilModel>();
            Mapper.CreateMap<UpdatePerfilModel,Account>();
            Mapper.CreateMap<ListAccountModel, Account>();
            Mapper.CreateMap<Account, ListAccountModel>();
            Mapper.CreateMap<PaquetesPremiumModel, PaquetesPremium>();
            Mapper.CreateMap<PaquetesPremium, PaquetesPremiumModel>();
        
        }
    }
}