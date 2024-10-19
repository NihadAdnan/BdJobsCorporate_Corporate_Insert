using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using System;
using System.Linq;
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

        public async Task<bool> InsertRecordAsync(CompanyProfile companyProfile, ContactPerson contactPerson, CorporateUserAccess corporateUserAccess)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the corporate account already exists
                        if (await _companyRepository.IsCorporateAccountExistAsync(companyProfile.CompanyName, transaction))
                        {
                            throw new Exception("Corporate account already exists.");
                        }

                        // Check if the user email already exists
                        if (await _companyRepository.IsUserNameExistAsync(corporateUserAccess.UserName, transaction))
                        {
                            throw new Exception("User with this email already exists.");
                        }

                        // Generate new company ID and ID code
                        companyProfile.CorporateAccountID = (int)await _companyRepository.GetNextCompanyIdAsync();  // Explicit cast to int
                        companyProfile.IDCode = RandomString(8); // Generate random 8-character ID code

                        // Insert company profile
                        await _companyRepository.InsertCompanyProfileAsync(companyProfile, transaction);

                        // Insert contact person linked to the company
                        contactPerson.CP_ID = companyProfile.CorporateAccountID;
                        await _companyRepository.InsertContactPersonAsync(contactPerson, transaction);

                        // Retrieve and update the contact ID in the company profile
                        int contactId = await _companyRepository.GetContactIdAsync(companyProfile.CorporateAccountID, transaction);
                        await _companyRepository.UpdateCompanyProfileContactIdAsync(companyProfile.CorporateAccountID, contactId, transaction);

                        // Insert industry types if provided
                        if (companyProfile.DisabilityTypes != null && companyProfile.DisabilityTypes.Any())
                        {
                            await _companyRepository.InsertIndustryTypesAsync(companyProfile.CorporateAccountID, companyProfile.DisabilityTypes, transaction);
                        }

                        // Insert user access details
                        await _companyRepository.InsertUserAccessAsync(companyProfile.CorporateAccountID, contactId, corporateUserAccess.UserName, corporateUserAccess.Password, transaction);

                        // Insert business details if provided
                        //if (!string.IsNullOrEmpty(corporateUserAccess.BusinessName) && !string.IsNullOrEmpty(corporateUserAccess.BusinessDetail))
                        //{
                        //    await _companyRepository.InsertBusinessDetailsAsync(companyProfile.CorporateAccountID, corporateUserAccess.BusinessName, corporateUserAccess.BusinessDetail, corporateUserAccess.PostedBy, transaction);
                        //}

                        // Insert entrepreneurship details if required
                        if (corporateUserAccess.ContactId > 0)
                        {
                            await _companyRepository.InsertEntrepreneurshipAsync(companyProfile.CorporateAccountID, corporateUserAccess.ContactId, transaction);
                        }

                        // Commit transaction
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Generates a random alphanumeric string of the specified length
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
