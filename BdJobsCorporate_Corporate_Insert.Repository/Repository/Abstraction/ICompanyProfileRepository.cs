using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction
{
    public interface ICompanyRepository
    {
        Task<long> GetNextCompanyIdAsync();
        Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction);
        Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction);
        Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction);
        Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction);
    }
}
