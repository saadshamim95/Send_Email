using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sending_Email.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class SendingEmailController : ControllerBase
    {
        private readonly ILogger<SendingEmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public SendingEmailController(ILogger<SendingEmailController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _logger = logger;
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        public IActionResult SendEmail(SendEmailRequest request)
        {
            SendEmailResponse response = new SendEmailResponse();
            
            try
            {
                string Sender = _configuration["Sender"];
                string Password = _configuration["Password"];
                string FilePath = @"D:/EmailText.txt";
                StreamReader str = new StreamReader(FilePath);
                string mailText = str.ReadToEnd();
                mailText = mailText.Replace("[#USERFIRSTNAME#]", request.Username);
                string subject = "Forgot Password";

                MailMessage mailMsg = new MailMessage();
                mailMsg.IsBodyHtml = true;
                mailMsg.From = new MailAddress(Sender);
                mailMsg.To.Add(request.Receiver);
                mailMsg.To.Add(request.Receiver2);
                mailMsg.Subject = subject;
                mailMsg.Body = mailText;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _configuration["Host"];
                smtp.Port = Convert.ToInt32(_configuration["Port"]);
                smtp.EnableSsl = true;

                NetworkCredential network = new NetworkCredential(Sender, Password);
                smtp.Credentials = network;

                smtp.Send(mailMsg);
                response.IsSuccess = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest(response);
            }
        }

    }
}
