


using Azure;
using Core.Application.Dtos.Account;
using Core.Application.Dtos.Account.Request;
using Core.Application.Dtos.Account.Response;
using Core.Application.Enums;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModel.User;
using Infrastructure.Identity.Entities;
using Infrastructure.Identity.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace Infrastructure.Identity.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplitationUser> userManager;
        private readonly SignInManager<ApplitationUser> signInManager;
        private readonly IEmailService emailService;

        public AccountServices(UserManager<ApplitationUser> userManager
            , SignInManager<ApplitationUser> signInManager
            , IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }



        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            AuthenticationResponse authenticationResponse = new();

            if (request == null || string.IsNullOrEmpty(request.Correo))
            {
                authenticationResponse.HasError = true;
                authenticationResponse.Error = "El correo electrónico es nulo o vacío.";
                return authenticationResponse;
            }

            var user = await userManager.FindByEmailAsync(request.Correo);


            if (user == null)
            {
                authenticationResponse.HasError = true;
                authenticationResponse.Error = $"NO EXISTE UN CUENSTA REGISTRADA CON EL SIGUIENTE CORREO : {request.Correo}";

                return authenticationResponse;
            }

            //AQUI HACEMOS EL LOGIN 
            //PARAMETROS A PASAR :
            //1 - EL NOMBRE DE USUARIO DEL USUARIO
            //2 - EL PASSWORD DEL USUARIO
            //3 - ES POR SI QUIERES QUE EL NAVEGAR GUARDE TU CUENTA 
            //4 - EL NOMBRE DE USUARIO DEL USUARIO
            var result = await signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                authenticationResponse.HasError = true;
                authenticationResponse.Error = $"SUS CREDENCIALES ESTAN INCORRECTAS : {request.Correo}";

                return authenticationResponse;
            }
            if (!user.EmailConfirmed)
            {
                authenticationResponse.HasError = true;
                authenticationResponse.Error = $"SU CUENTA NO HA SIDO CONFIRMADA : {request.Correo}";

                return authenticationResponse;
            }

            authenticationResponse.Id = user.Id;
            authenticationResponse.Nombre = user.Nombre;
            authenticationResponse.Apellido = user.Apellido;
            authenticationResponse.UserName = user.UserName!;
            authenticationResponse.Email = user.Email!;
            authenticationResponse.IsValified = user.EmailConfirmed;
            var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationResponse.Roles = rolesList.ToList();




            return authenticationResponse;
        }

        public async Task LogOut()
        {
            await signInManager.SignOutAsync();
        }
        public async Task<RegisterResponse> RegitroBasicAsync(RegisterRequest request, string origin)
        {
            RegisterResponse registerResponse = new();
            registerResponse.HasError = false;

            var NombreUsuarioExistente = await userManager.FindByNameAsync(request.UserName);
            if (NombreUsuarioExistente != null)
            {
                registerResponse.HasError = true;
                registerResponse.Error = $"ESTE {request.UserName} YA ESTA REGISTRADO";

                return registerResponse;
            }
            var EmailExistente = await userManager.FindByEmailAsync(request.Email);
            if (EmailExistente != null)
            {
                registerResponse.HasError = true;
                registerResponse.Error = $"ESTE {request.Email} YA REGISTRADO";

                return registerResponse;
            }

            var user = new ApplitationUser
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                UserName = request.UserName,
            };

            if (request.Rol == "Admin")
            {
                user.EmailConfirmed = true;
            }


            var result = await userManager.CreateAsync(user, request.Password);


            if (result.Succeeded)
            {
                if (request.Rol == "Admin")
                {
                    await userManager.AddToRoleAsync(user, EnumRoles.Admin.ToString());
                }
                else
                {

                    var VeficacacionUri = await EnviarVerificacionUrl(user, origin);
                    //esto es para enviar el correo de verificacion


                    await emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
                    {
                        To = user.Email,
                        Body = $@"
                            <!DOCTYPE html>
                            <html lang='es'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Confirmación de Cuenta</title>
                                <style>
                                    body {{
                                        font-family: 'Times New Roman', Times, serif;
                                        color: #333;
                                        background-color: #f4f4f4;
                                        margin: 0;
                                        padding: 0;
                                    }}
                                    .container {{
                                        width: 100%;
                                        max-width: 600px;
                                        margin: auto;
                                        padding: 20px;
                                        background-color: #ffffff;
                                        border-radius: 8px;
                                        border: 1px solid #dddddd;
                                    }}
                                    h1 {{
                                        color: #007bff;
                                        font-size: 24px;
                                        margin-top: 0;
                                    }}
                                    p {{
                                        line-height: 1.6;
                                        margin: 0 0 10px;
                                    }}
                                    a {{
                                        color: #007bff;
                                        text-decoration: none;
                                        font-weight: bold;
                                    }}
                                    a:hover {{
                                        text-decoration: underline;
                                    }}
                                    .footer {{
                                        font-size: 0.8em;
                                        color: #666666;
                                        text-align: center;
                                        margin-top: 20px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1>¡Bienvenido a la RentDrive De tu Preferencia!</h1>
                                    <p>Gracias por registrarte a tu RentDrive De tu Preferencia. Para completar el proceso de registro, por favor confirma tu cuenta haciendo clic en el siguiente enlace:</p>
                                    <p><a href='{VeficacacionUri}' target='_blank'>Confirmar mi cuenta</a></p>
                                    <p>Si no has solicitado esta cuenta, por favor ignora este correo electrónico.</p>
                                    <p>¡Gracias por unirte a nosotros!</p>
                                </div>
                            </body>
                            </html>",
                                                Subject = "Confirmar Cuenta"
                                            });


                    await userManager.AddToRoleAsync(user, EnumRoles.Usuario.ToString());

                }



            }
            else
            {
                registerResponse.HasError = true;
                registerResponse.Error = "LO SIENTO, OCURRIO UN ERROR EN EL PROCESOR DE CREAR EL USUARIO";

                return registerResponse;
            }

            return registerResponse;
        }

        public async Task<ForgotPasswordReponse> ForgotPasswor(ForgotPasswordRequest request, string origin)
        {
            ForgotPasswordReponse ForgotResponse = new();
            ForgotResponse.HasError = false;


            var result = await userManager.FindByEmailAsync(request.Email);

            if (result == null)
            {
                ForgotResponse.HasError = true;
                ForgotResponse.Error = $"LO SIENTO, NO EXISTE UNA CUENTA REGISTRADA CON EL EMAIL INTRODUCIDO";

                return ForgotResponse;
            }

            var VerificacionUri = await EnviarContraseñaUrl(result, origin);
            //esto es para enviar el cambiar la contrase;a
            await emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
            {
                To = result.Email!,
                Body = $@"<!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Restablecimiento de Contraseña</title>
                        <style>
                            body {{font-family: 'Times New Roman', Times, serif;
                                color: #333;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{width: 100%;
                                max-width: 600px;
                                margin: auto;
                                padding: 20px;
                                background-color: #ffffff;
                                border-radius: 8px;
                                border: 1px solid #dddddd;
                            }}
                            h1 {{color: #007bff;
                                font-size: 24px;
                                margin-top: 0;
                            }}
                            p {{line - height: 1.6;
                                margin: 0 0 10px;
                            }}
                            a {{color: #007bff;
                                text-decoration: none;
                                font-weight: bold;
        
                                    }}
                            a:hover {{text - decoration: underline;
                            }}
                            .footer {{font - size: 0.8em;
                                color: #666666;
                                text-align: center;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h1>Solicitud de Restablecimiento de Contraseña</h1>
                            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta. Si fuiste tú, por favor, haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                            <p><a href='{VerificacionUri}' target='_blank'>Restablecer mi contraseña</a></p>
                            <p>Si no solicitaste el restablecimiento de contraseña, por favor ignora este correo electrónico. Tu contraseña no será cambiada.</p>
                            <p>¡Gracias por usar nuestros servicios!</p>
                        </div>
                    </body>
                    </html>
                    ",
                                    Subject = "RESETEAR CONTRASEÑA !!"
                                });



            return ForgotResponse;
        }
        public async Task<ResetPasswordReponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ResetPasswordReponse resetPasswordResponse = new();
            resetPasswordResponse.HasError = false;

            var Cuenta = await userManager.FindByEmailAsync(request.Email);

            if (Cuenta == null)
            {
                resetPasswordResponse.HasError = true;
                resetPasswordResponse.Error = $"LO SIENTO, NO EXISTE UNA CUENTA REGISTRADA CON EL SIGUIENTE EMAIL {Cuenta!.Email}";

                return resetPasswordResponse;
            }
            request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var result = await userManager.ResetPasswordAsync(Cuenta, request.Token, request.Password);
            if (!result.Succeeded)
            {
                resetPasswordResponse.HasError = true;
                resetPasswordResponse.Error = $"LO SIENTO,HA OCURRIDO UN ERROR MIESTRA SE CAMBIABA LA CONTRASEÑA!! ";

                return resetPasswordResponse;
            }

            return resetPasswordResponse;

        }
        //ESTO ES PARA EL ENVIO DE CORREO DE CONFIRMACION DE LA CUENTA
        private async Task<string> EnviarVerificacionUrl(ApplitationUser applitationUser, string origin)
        {
            //primero crearemos el origen del correo

            //aqui generamos un token para el usuario a verficr , esto con el objetivo de que solo se pueda enviar desde el correo y no por la url
            var code = await userManager.GenerateEmailConfirmationTokenAsync(applitationUser);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //esto es para mandar a la accion del confirm en el controllers
            var ruta = "User/ConfirmEmail";

            var url = new Uri(string.Concat($"{origin}/", ruta));

            var verificacionUrl = QueryHelpers.AddQueryString(url.ToString(), "userId", applitationUser.Id);

            verificacionUrl = QueryHelpers.AddQueryString(verificacionUrl, "token", code);

            return verificacionUrl;
        }

        public async Task<String> ConfirmarCuenta(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "LO SIENTO , NO ENCONTRAMOS EL USUARIO";
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return $"SU CUENTA A SIDO CONFIRMADA POR {user.Email} , YA ERES USUARIO DE NUESTRA RENTCARS";
            }
            else
            {
                return "HA OCURRIDO UN ERROR MIENTRA SE EL PROCESO DE CONFIRMAR SU CUENTA";
            }

        }
        private async Task<string> EnviarContraseñaUrl(ApplitationUser applitationUser, string origin)
        {
            //primero crearemos el origen del correo

            //aqui generamos un token para el usuario a verficr , esto con el objetivo de que solo se pueda enviar desde el correo y no por la url
            var code = await userManager.GeneratePasswordResetTokenAsync(applitationUser);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //esto es para mandar a la accion del confirm en el controllers
            var ruta = "User/ResetPassword";

            var url = new Uri(string.Concat($"{origin}/", ruta));


            var verificacionUrl = QueryHelpers.AddQueryString(url.ToString(), "token", code);

            return verificacionUrl;
        }

        public async Task<UpdateResponse> EditUserAsync(string userId, UpdateRequest request)
        {
            UpdateResponse updateResponse = new();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                updateResponse.HasError = true;
                updateResponse.Error = $"Lo Siento, No Encontre la Cuenta Con el Siguiente Id {user!.Id}";

                return updateResponse;
            }

            user.Nombre = request.Nombre;
            user.Apellido = request.Apellido;
            user.UserName = request.UserName;
            user.Email = request.Email;

            var token = await userManager.GeneratePasswordResetTokenAsync(user!);

            if (request.Password != null)
            {
                var result = await userManager.ResetPasswordAsync(user, token, request.Password!);
                if (!result.Succeeded)
                {
                    updateResponse.HasError = true;
                    updateResponse.Error = $"LO SIENTO,HA OCURRIDO UN ERROR MIESTRA SE CAMBIABA LA CONTRASEÑA!! ";

                    return updateResponse;
                }
            }


            var resultUpdate = await userManager.UpdateAsync(user);
            if (!resultUpdate.Succeeded)
            {
                updateResponse.HasError = true;
                updateResponse.Error = $"LO SIENTO,HA OCURRIDO UN ERROR MIESTRA SE CAMBIABA LA CONTRASEÑA!! ";

                return updateResponse;
            }

            return updateResponse;
        }

        public async Task<UpdateUserViewModel> GetById(string userId = "", bool action = true, string UserName = "")
        {
            if (action == true)
            {

                var userById = await userManager.FindByIdAsync(userId);
                UpdateUserViewModel updateUserById = new();

                updateUserById.UserId = userById!.Id;
                updateUserById.Nombre = userById!.Nombre;
                updateUserById.Apellido = userById.Apellido;
                updateUserById.UserName = userById.UserName!;
                updateUserById.Email = userById.Email!;
                

                return updateUserById;
            }

            var user = await userManager.FindByNameAsync(UserName);

            UpdateUserViewModel updateUser = new();

            updateUser.UserId = user!.Id;
            updateUser.Nombre = user!.Nombre;
            updateUser.Apellido = user.Apellido;
            updateUser.UserName = user.UserName!;
            updateUser.Email = user.Email!;
            

            return updateUser;

        }

        public async Task<IEnumerable<UserViewModel>> GetAllUser()
        {
            var result = await userManager.Users.ToListAsync();

            var Users = new List<UserViewModel>();

            foreach (var user in result)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userViewModel = new UserViewModel
                {
                    UserId = user.Id,
                    Nombre = user.Nombre!,
                    Apellido = user.Apellido!,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Rol = string.Join(",", roles) 
                };

                Users.Add(userViewModel);
            }

            return Users;
        }


    }
}
