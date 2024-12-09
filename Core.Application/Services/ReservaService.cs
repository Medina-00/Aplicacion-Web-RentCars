

using AutoMapper;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModel.Reserva;
using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;
using InternetBanking.Core.Application.Services;
using System.Security.Claims;

namespace Core.Application.Services
{
    public class ReservaService : GenericService<SaveReservaViewModel, ReservaViewModel, Reserva>,IReservaService 
    {
        private readonly IReservaRepository reservaRepository;
        private readonly IVehiculoRepository vehiculoRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly IUserService userService;

        public ReservaService(IReservaRepository reservaRepository 
            ,IVehiculoRepository vehiculoRepository, 
            IMapper mapper, IEmailService emailService,
            IUserService userService) : base(reservaRepository, mapper)
        {

            this.reservaRepository = reservaRepository;
            this.vehiculoRepository = vehiculoRepository;
            this.mapper = mapper;
            this.emailService = emailService;
            this.userService = userService;
        }

        public override async Task<List<ReservaViewModel>> GetAllViewModel()
        {
            var entityList = await reservaRepository.GetAllAsync();
          
            var reservasViewModel = mapper.Map<List<ReservaViewModel>>(entityList);

            // Llena la información del vehículo
            foreach (var reserva in reservasViewModel)
            {
                var vehiculo = await vehiculoRepository.GetByIdAsync(reserva.VehiculoId);
                reserva.Vehiculos = mapper.Map<VehiculoViewModel>(vehiculo);
            }

            return reservasViewModel;
            
        }

        public async Task<SaveReservaViewModel> Add(SaveReservaViewModel vm, string UserId)
        {
            // Obtener el precio por día del vehículo
            var vehiculo = await vehiculoRepository.GetByIdAsync(vm.VehiculoId);
            var preciopordia = vehiculo.PrecioPorDia; // Asumir que PrecioPorDia es una propiedad del Vehiculo

            // Calcular la cantidad de días de la reserva
            var cantidadDias = (vm.FechaFin - vm.FechaInicio).Days;

            // Asegurarse de que la cantidad de días sea positiva
            if (cantidadDias <= 0)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            // Calcular el precio total
            vm.PrecioTotal = cantidadDias * preciopordia;

            // Mapear y agregar la reserva
            Reserva entity = mapper.Map<Reserva>(vm);
            entity = await reservaRepository.AddAsync(entity);

            SaveReservaViewModel entityVm = mapper.Map<SaveReservaViewModel>(entity);

            var user = await userService.GetById(UserId);


            await emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
            {
                To = user.Email,
                        Body = $@"<!DOCTYPE html>
        <html lang=""es"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Notificación de Reserva</title>
            <style>
                body {{
                    font-family: 'Times New Roman', Times, serif;
                    background-color: #e9ecef;
                    margin: 0;
                    padding: 20px;
                    text-align: center;
                }}
                .container {{
                    background-color: #ffffff;
                    padding: 30px;
                    border-radius: 8px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
                    max-width: 600px;
                    margin: auto;
                }}
                .header {{
                    text-align: center;
                    padding: 10px;
                    background-color: #007bff;
                    color: white;
                    border-radius: 8px 8px 0 0;
                }}
                h1 {{
                    margin: 0;
                    font-size: 24px;
                }}
                p {{
                    color: #555;
                    line-height: 1.6;
                }}
                .vehicle-img {{
                    max-width: 100%;
                    height: auto;
                    border-radius: 5px;
                    margin: 15px 0;
                }}
                .details {{
                    border-top: 1px solid #e9ecef;
                    padding-top: 15px;
                    margin-top: 15px;
                }}
                .details li {{
                    margin: 5px 0;
                }}
                .btn {{
                    display: inline-block;
                    padding: 12px 20px;
                    color: #ffffff;
                    background-color: #28a745;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 15px;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                    text-align: center;
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <h1>Confirmación de Reserva</h1>
                </div>
                <p>Estimado/a <strong>'{user.Nombre} {user.Apellido}'</strong>,</p>
                <p>Nos complace informarte que tu reserva para el vehículo ha sido confirmada.</p>
                <div class=""details"">
                    <h2>Detalles de la Reserva</h2>
                    <p><strong>Vehículo:</strong> '{vehiculo.Marca} {vehiculo.Modelo} {vehiculo.Año}'</p>
                    <p><strong>Fecha de Inicio:</strong> '{entity.FechaInicio}'</p>
                    <p><strong>Fecha de Fin:</strong> '{entity.FechaFin}'</p>
                    <p><strong>Precio Total:</strong> '{entity.PrecioTotal}'</p>
                </div>
                <p>Si necesitas realizar algún cambio o tienes preguntas, no dudes en ponerte en contacto con nosotros.</p>
                <div class=""footer"">
                    <p>Gracias por elegir <strong>RentDrive</strong>.</p>
                    <p>Este es un correo automático, por favor no respondas.</p>
                </div>
            </div>
        </body>
        </html>
        ",
                Subject = "Detalle de la reserva"
            });
            return entityVm;
        }

    }
}
