using AutoMapper;
using BankingAPI.Dto;
using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Helper
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      CreateMap<Client, ClientDto>();
      CreateMap<Account, AccountDto>()
        .ForMember(
          dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));
      CreateMap<Movement, MovementDto>()
        .ForMember(
          dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account.Id))
        .ForMember(
          dest => dest.AccountType, opt => opt.MapFrom(src => src.Account.Type))
        .ForMember(
          dest => dest.AccountStatus, opt => opt.MapFrom(src => src.Account.Status))
        .ForMember(
          dest => dest.ClientName, opt => opt.MapFrom(src => src.Account.Client.Name));

      CreateMap<ClientCreationDto, Client>();
      CreateMap<AccountCreationDto, Account>();
      CreateMap<MovementCreationDto, Movement>();

      CreateMap<AccountUpdateDto, Account>();
      CreateMap<MovementUpdateDto, Movement>();
    }
  }
}
