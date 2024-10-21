using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Handler.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly DapperDbContext _context;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(DapperDbContext context, ICompanyRepository companyRepository)
        {
            _context = context;
            _companyRepository = companyRepository;
        }

        public async Task<bool> IsCorporateAccountExist(string name)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return await _companyRepository.IsCorporateAccountExistAsync(name, transaction);
                }
            }
        }

        public async Task<bool> IsUserNameExist(string userName)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return await _companyRepository.IsUserNameExistAsync(userName, transaction);
                }
            }
        }

        public async Task InsertRecordAsync(CompanyProfile companyProfile, ContactPerson contactPerson, CorporateUserAccess corporateUserAccess)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (await _companyRepository.IsCorporateAccountExistAsync(companyProfile.CompanyName, transaction))
                        {
                            throw new Exception("Corporate account already exists.");
                        }

                        long companyId = await _companyRepository.GetNextCompanyIdAsync();

                        companyProfile.CorporateAccountID = (int)companyId; 
                        await _companyRepository.InsertCompanyProfileAsync(companyProfile, transaction);

                        contactPerson.CorporateAccountID = (int)companyId; 
                        await _companyRepository.InsertContactPersonAsync(contactPerson, transaction);

                        int contactId = await _companyRepository.GetContactIdAsync(companyId, transaction);

                        await _companyRepository.InsertUserAccessAsync(companyId, contactId, corporateUserAccess.UserName, corporateUserAccess.Password, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("An error occurred while inserting the record.", ex);
                    }
                }
            }
        }

    }
}




