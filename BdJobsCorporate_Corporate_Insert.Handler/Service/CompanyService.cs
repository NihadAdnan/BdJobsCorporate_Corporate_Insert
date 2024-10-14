using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Handler.Service
{
    public class CompanyService:ICompanyService
    {
        private readonly DapperDbContext _context;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(DapperDbContext context, ICompanyRepository companyRepository)
        {
            _context = context;
            _companyRepository = companyRepository;
        }

        public async Task<bool> InsertRecordAsync(CompanyProfile companyProfile, ContactPerson contactPerson)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        companyProfile.CP_ID = await _companyRepository.GetNextCompanyIdAsync();

                        await _companyRepository.InsertCompanyProfileAsync(companyProfile, transaction);

                        contactPerson.CP_ID = companyProfile.CP_ID;
                        await _companyRepository.InsertContactPersonAsync(contactPerson, transaction);

                        int contactId = await _companyRepository.GetContactIdAsync(companyProfile.CP_ID, transaction);

                        await _companyRepository.UpdateCompanyProfileContactIdAsync(companyProfile.CP_ID, contactId, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
