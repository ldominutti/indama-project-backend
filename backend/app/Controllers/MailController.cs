using System.Threading.Tasks;
using IndamaService.Models.DTOs;
using IndamaService.Services.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("mail")]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost]
    [EnableCors("AllowSpecificOrigin")]
    public Task<MailDto> SendMail(MailDto mailDto)
    {
        return _mailService.SendMail(mailDto);
    }
}
