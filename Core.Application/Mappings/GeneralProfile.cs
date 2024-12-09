
using AutoMapper;
using Core.Application.Dtos.Account;
using Core.Application.Dtos.Account.Request;
using Core.Application.ViewModel.Reserva;
using Core.Application.ViewModel.User;
using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;

namespace Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            

            CreateMap<AuthenticationRequest, LoginViewModel>()
                .ForMember(d => d.Error, o => o.Ignore())
                .ForMember(d => d.HasError, o => o.Ignore())
                .ReverseMap();

            CreateMap<RegisterRequest, SaveUserViewModel>()
                .ForMember(d => d.Error, o => o.Ignore())
                .ForMember(d => d.HasError, o => o.Ignore())
                .ReverseMap();

            CreateMap<UpdateRequest, UpdateUserViewModel>()
                .ForMember(d => d.Error, o => o.Ignore())
                .ForMember(d => d.HasError, o => o.Ignore())
                .ReverseMap();

            CreateMap<ForgotPasswordRequest, ForgotPasswordViewModel>()
               .ForMember(d => d.Error, o => o.Ignore())
               .ForMember(d => d.HasError, o => o.Ignore())
               .ReverseMap();

            CreateMap<ResetPasswordRequest, ResetPasswordViewModel>()
               .ForMember(d => d.Error, o => o.Ignore())
               .ForMember(d => d.HasError, o => o.Ignore())
               .ReverseMap();

            CreateMap<Vehiculo, VehiculoViewModel>()
               .ReverseMap()
               .ForMember(d => d.Categoria, o => o.Ignore())
               .ForMember(d => d.Reservas, o => o.Ignore());

            CreateMap<Vehiculo, SaveVehiculoViewModel>()
                .ForMember(d => d.File, o => o.Ignore())

               .ReverseMap();

            CreateMap<Reserva, ReservaViewModel>()
                .ForMember(d => d.Vehiculos, o => o.Ignore())
               .ReverseMap()
               .ForMember(d => d.Vehiculo, o => o.Ignore());

            CreateMap<Reserva, SaveReservaViewModel>()
               .ForMember(d => d.Vehiculos, o => o.Ignore())
               .ReverseMap();
               //.ForMember(d => d.PrecioTotal, o => o.Ignore());



        }
    }
}
