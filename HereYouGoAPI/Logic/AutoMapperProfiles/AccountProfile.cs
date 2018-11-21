using AutoMapper;
using Domain.Entities;
using Domain.ViewModels;

namespace Logic.AutoMapperProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountViewModel, Account>()
                .ForMember(x => x.Id, x => x.MapFrom(a => a.Id))
                .ForMember(x => x.Login, x => x.MapFrom(a => a.Login))
                .ForMember(x => x.WalletAddress, x => x.MapFrom(a => a.WalletAddress));
        }
    }
}
