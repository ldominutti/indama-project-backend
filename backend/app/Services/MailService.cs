using System;
using System.Net.Mail;
using System.Net;
using IndamaService.Models.DTOs;
using IndamaService.Services.Interface;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace IndamaService.Services
{
    public class MailService : IMailService
    {
        private readonly string recaptchaSecretKey = "6LfSWPspAAAAAHPh9C6Cwvl8RBpxdvkoPQvfhIo0";

        public async Task<MailDto> SendMail(MailDto mailDto)
        {
            if (mailDto.Name == null || mailDto.Name.Length > 25 || mailDto.Email == null || mailDto.Message == null || mailDto.Message.Length > 1000)
            {
                throw new Exception("Mail is not valid");
            }

            if (!IsValidEmail(mailDto.Email))
            {
                throw new FormatException("Invalid email address format");
            }

            if (!await IsRecaptchaValid(mailDto.Recaptcha))
            {
                throw new Exception("Invalid reCAPTCHA");
            }

            string fromEmail = "indamabot@gmail.com";
            string password = "lmml xkja vzut vzau";
            string toEmail = "lucianodominutti@gmail.com";

            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = true;

                string emailBody = $@"
                    <html>
                    <body>
                        <p>Hola,</p>
                        <p>Has recibido un nuevo mensaje de un cliente a través del bot de <strong>INDAMA</strong>.</p>

                        <p><strong>Detalles del mensaje:</strong></p>
                        <ul>
                            <li><b>Nombre del cliente:</b> {mailDto.Name}</li>
                            <li><b>Email del cliente:</b> {mailDto.Email}</li>
                            <li><b>Mensaje:</b><br>{mailDto.Message}</li>
                        </ul>

                        <p>Puedes responder al cliente directamente utilizando su correo electrónico proporcionado arriba.</p>

                        <p>Este mensaje fue enviado automáticamente por el bot de INDAMA. <strong>No respondas a este correo directamente.</strong></p>

                        <p>Gracias,<br>El equipo de INDAMA</p>
                    </body>
                    </html>";

                using (MailMessage mailMessage = new MailMessage(fromEmail, toEmail))
                {
                    mailMessage.Subject = "Nuevo mensaje de cliente - Consulta INDAMA";
                    mailMessage.Body = emailBody;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        smtpClient.Send(mailMessage);
                        return mailDto;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error sending mail: " + ex.Message, ex);
                    }
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private async Task<bool> IsRecaptchaValid(string recaptchaResponse)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={recaptchaResponse}", null);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JObject.Parse(jsonResponse);
                return result.success == true;
            }
        }
    }
}
