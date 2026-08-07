using System.Threading.Tasks;

namespace HamperStore.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
