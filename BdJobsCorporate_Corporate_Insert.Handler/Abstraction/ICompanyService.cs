using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Handler.Abstraction
{
    public interface ICompanyService
    {
        Task<bool> InsertRecordAsync(CompanyProfile companyProfile, ContactPerson contactPerson);
    }
}
