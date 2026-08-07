using HamperStore.Web.Models;

namespace HamperStore.Core.Interfaces
{
    public interface IInquiryService
    {
        Task<string> SubmitAsync(InquiryFormModel model);
    }
}
