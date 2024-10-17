using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Handler.Abstraction
{
    public interface ICompanyService
    {
        Task<bool> IsCorporateAccountExist(string name);
        Task<bool> IsUserNameExist(string userName);
        Task<bool> InsertRecordAsync(CompanyProfile companyProfile, ContactPerson contactPerson, CorporateUserAccess corporateUserAccess);
    }
}
