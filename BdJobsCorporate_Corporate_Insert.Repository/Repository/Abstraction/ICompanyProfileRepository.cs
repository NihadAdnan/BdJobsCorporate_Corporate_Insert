using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction
{
    public interface ICompanyRepository
    {
        IDbTransaction BeginTransaction();
        Task<long> GetNextCompanyIdAsync();
        Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction);
        Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction);
        Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction);
        Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction);
        Task<bool> IsCorporateAccountExistAsync(string companyName, IDbTransaction transaction);
        Task<bool> IsUserNameExistAsync(string userName, IDbTransaction transaction);
        Task InsertIndustryTypesAsync(long companyId, List<int> industryTypeIds, IDbTransaction transaction);
        Task<bool> ValidateCaptchaAsync(string sessionValue, string captchaValue);
        Task<bool> IsHackingAttemptAsync(string input);

    }

 
}
