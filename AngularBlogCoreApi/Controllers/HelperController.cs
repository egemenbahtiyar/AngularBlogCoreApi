using AngularBlogCoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AngularBlogCoreApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendContactEmail(Contact contact)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                NetworkCredential kullanicibilgi = new NetworkCredential("cyptometa@gmail.com", "kripto10");
                smtp.Credentials = kullanicibilgi;

                MailAddress gonderen = new MailAddress("cyptometa@gmail.com");

                MailAddress alici = new MailAddress("bahtiyaregemen51@gmail.com");

                MailMessage mail = new MailMessage(gonderen, alici);
                mail.Subject = contact.Subject;
                mail.Body = contact.Message;
                mail.IsBodyHtml = true;
                smtp.Send(mail);

                return Ok();


            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }


        }
    }
}
