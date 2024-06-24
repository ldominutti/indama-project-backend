using System.Threading.Tasks;
using IndamaService.Models.DTOs;

namespace IndamaService.Services.Interface
{
    public interface IMailService
    {
        Task<MailDto> SendMail(MailDto mailDto);
    }
}
