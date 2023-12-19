using Application.BankAccounts;
using AutoMapper;
using Domain;
namespace Application.Core.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bank,Bank>();
            CreateMap<BankAccount,BankAccount>();
            CreateMap<BankAccountType,BankAccountType>();
            CreateMap<Category,Category>();
            CreateMap<Envelope,Envelope>();
            CreateMap<Payee,Payee>();
            CreateMap<Transaction,Transaction>();
        }
    }
}
