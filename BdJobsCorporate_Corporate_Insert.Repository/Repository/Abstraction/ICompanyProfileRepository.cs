using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public interface ICompanyRepository
{
    IDbTransaction BeginTransaction();
    Task<long> GetNextCompanyIdAsync();
    Task<bool> IsCorporateAccountExistAsync(string companyName, IDbTransaction transaction);
    Task<bool> IsUserNameExistAsync(string userName, IDbTransaction transaction);
    Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction);
    Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction);
    Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction);
    Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction);
    Task InsertIndustryTypesAsync(long companyId, List<int> industryTypeIds, IDbTransaction transaction);
    Task InsertUserAccessAsync(long companyId, int contactId, string userName, string password, IDbTransaction transaction);
    Task InsertBusinessDetailsAsync(long companyId, string businessName, string businessDetail, int postedBy, IDbTransaction transaction);
    Task InsertEntrepreneurshipAsync(long corporateAccountId, int contactId, bool isEntrepreneur, IDbTransaction transaction);
    Task ProcessNewIndustryAsync(string strNewIndustry, string industryTypeIds, IDbTransaction transaction); 
}
