using AutoMapper;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.BranchData.DTOs;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.NotificationData.Entities;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Database.UserData.Entities;
using BankManagement.Services.AccountServices.DTOs;
using BankManagement.Services.BranchServices.DTOs;
using BankManagement.Services.NotificationService.DTOs;
using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Services.UserServices.DTOs;
using BankManagement.Utilities.Enums;


namespace BankManagement.Services.Mappers;

public class EntityMapper : Profile
{
    public EntityMapper()
    {
        CreateMap<UserDTO, User>().ReverseMap();
        CreateMap<ProfileDetailsDTO, UserProfile>().ReverseMap();
        CreateMap<LoginDTO, User>().ReverseMap();
        CreateMap<RegistrationDTO, User>().ReverseMap();
        CreateMap<UserRecoveryDTO, User>().ReverseMap();

        CreateMap<BranchDTO, Branch>().ReverseMap();
        CreateMap<BranchUpdationDTO, Branch>().ReverseMap();

        CreateMap<NotificationDTO, Notification>().ReverseMap();

        CreateMap<AccountCreationRequestDTO, Account>()
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.AccountTypeId, opt => opt.MapFrom(src => (int)src.AccountTypeName))
            .ReverseMap();

        CreateMap<AccountBranchTransferDTO, Account>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ReverseMap();

        CreateMap<Account, AccountResultDTO>()
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => (AccountName)src.AccountTypeId))
            .ForMember(dest => dest.BranchCode, opt => opt.MapFrom(src => src.Branch.BranchCode))
            .ForMember(dest => dest.IFSCCode, opt => opt.MapFrom(src => src.Branch.IFSCCode))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.PanNumber, opt => opt.MapFrom(src => src.User.PanNumber))
            .ForMember(dest => dest.AadharNumber, opt => opt.MapFrom(src => src.User.AadharNumber));

        CreateMap<TransactionDTO, Transaction>()
            .ForMember(dest => dest.TransactionTypeId, opt => opt.MapFrom(src => src.TransactionTypeId))
            .ForMember(dest => dest.SourceTypeId, opt => opt.MapFrom(src => src.SourceTypeId))
            .ReverseMap();

        CreateMap<TransactionDTO, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AccountId, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionTypeId, opt => opt.MapFrom(src => (int)src.TransactionType))
            .ForMember(dest => dest.SourceTypeId, opt => opt.MapFrom(src => (int)src.SourceName))
            .ForMember(dest => dest.DateOfTransaction, opt => opt.MapFrom(src => src.DateOfTransaction))
            .ReverseMap()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => (TransactionName)src.TransactionTypeId))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => (TransactionSource)src.SourceTypeId));

             CreateMap<TransactionSource, SourceType>()
            .ForMember(dest => dest.SourceTypeId, opt => opt.MapFrom(src => (int)src));

        CreateMap<TransactionDTO, TransactionResultDTO>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => src.SourceName))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.DateOfTransaction, opt => opt.MapFrom(src => src.DateOfTransaction))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<TransactionType, TransactionName>()
                .ConvertUsing(src => src.Name);
        CreateMap<SourceType, TransactionSource>()
                .ConvertUsing(src => src.SourceName);
        CreateMap<StatusType, StatusName>()
                .ConstructUsing(src => src.Status);

    }
}
