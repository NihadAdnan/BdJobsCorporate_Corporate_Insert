using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public async Task<bool> IsUserNameExist(string email)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return await _companyRepository.IsUserNameExistAsync(email, transaction);
                }
            }
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
                        // Validate if the corporate account or user exists
                        if (await _companyRepository.IsCorporateAccountExistAsync(companyProfile.Name, transaction))
                        {
                            throw new Exception("Corporate account already exists.");
                        }

                        if (await _companyRepository.IsUserNameExistAsync(contactPerson.Email, transaction))
                        {
                            throw new Exception("User with this email already exists.");
                        }

                        // Generate the next company ID and a random ID code
                        companyProfile.CP_ID = await _companyRepository.GetNextCompanyIdAsync();
                        companyProfile.IDcode = RandomString(8); // Set the IDcode

                        // Insert the company profile
                        await _companyRepository.InsertCompanyProfileAsync(companyProfile, transaction);

                        // Prepare the contact person
                        contactPerson.CP_ID = companyProfile.CP_ID;
                        await _companyRepository.InsertContactPersonAsync(contactPerson, transaction);

                        // Get the contact ID for the inserted contact person
                        int contactId = await _companyRepository.GetContactIdAsync(companyProfile.CP_ID, transaction);

                        // Update the company profile with the contact ID
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

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
