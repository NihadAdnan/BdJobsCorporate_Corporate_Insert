using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Repository.Repository
{
    public class CompanyProfileRepository : ICompanyRepository
    {
        private readonly DapperDbContext _context;

        public CompanyProfileRepository(DapperDbContext context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            var connection = _context.CreateConnection();
            connection.Open();
            return connection.BeginTransaction();
        }

        public async Task<long> GetNextCompanyIdAsync()
        {
            var query = "SELECT MAX(CP_ID) AS CompanyID FROM Dbo_Company_Profiles";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var companyId = await connection.ExecuteScalarAsync<long?>(query) ?? 0;
                return companyId + 1;
            }
        }

        public async Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction)
        {
            var query = @"
    INSERT INTO Dbo_Company_Profiles 
    (CP_ID, Name, NameBng, Business, Address, AddressBng, Bill_Contact, City, Country, Web, Updated_date, Area, IDcode, OfflineCom, LicenseNo, RLNo, ThanaId, DistrictId, Contact_Person, Designation, Phone, E_Mail, IsFacilityPWD, Established, MinEmp, MaxEmp, IsEntrepreneur)
    VALUES
    (@CorporateAccountID, @CompanyName, @CompanyBangla, @BusinessDescription, @CompanyAddress, @CompanyAddressBng, @BillContact, @City, @Country, @WebsiteUrl, @UpdatedDate, @Area, @IDCode, @OfflineCom, @BusinessLicenseNo, @RLNo, @ThanaId, @DistrictId, @ContactPerson, @Designation, @ContactPhone, @ContactEmail, @DisabilitiesFacility, @CompanyEstablished, @MinEmployee, @MaxEmployee, @IsEntrepreneur)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                company.CorporateAccountID,
                company.CompanyName,
                company.CompanyBangla,
                company.BusinessDescription,       // Matching `BusinessDescription` to the class
                company.CompanyAddress,
                company.CompanyAddressBng,
                BillContact = company.CompanyAddress, // Mapping BillContact from CompanyAddress if needed
                company.City,
                company.Country,
                company.WebsiteUrl,               // Match the class `WebsiteUrl` to `Web` in SQL
                UpdatedDate = DateTime.Now,
                company.Area,
                company.IDCode,
                company.OfflineCom,
                company.BusinessLicenseNo,
                company.RLNo,
                company.ThanaId,                  // Ensure ThanaId and DistrictId are added to CompanyProfile if needed
                company.DistrictId,
                company.ContactPerson,
                company.Designation,
                ContactPhone = company.ContactMobile,  // Mapping `ContactMobile` to `Phone`
                company.ContactEmail,
                company.DisabilitiesFacility,     // Ensure `DisabilitiesFacility` is matched with IsFacilityPWD
                company.CompanyEstablished,
                MinEmployee = company.ProvideTrainingForEmployee,  // Map correct training property if needed
                MaxEmployee = company.ProvideTrainingForEmployee,  // Map correct training property if needed
                company.IsEntrepreneur
            }, transaction);
        }

        public async Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO ContactPersons
                (CP_ID, ContactName, Designation, CurrentUser, BillingContact, Mobile, Email)
                VALUES
                (@CP_ID, @ContactName, @Designation, @CurrentUser, @BillingContact, @Mobile, @Email)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                contactPerson.CP_ID,
                contactPerson.ContactName,
                contactPerson.Designation,
                contactPerson.CurrentUser,
                contactPerson.BillingContact,
                contactPerson.Mobile,
                contactPerson.Email
            }, transaction);
        }

        public async Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction)
        {
            var query = "SELECT ContactId FROM ContactPersons WHERE CP_ID=@CP_ID AND CurrentUser=1";
            var connection = transaction.Connection;
            var contactId = await connection.ExecuteScalarAsync<int?>(query, new { CP_ID = cpId }, transaction);
            return contactId ?? 0;
        }

        public async Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction)
        {
            var query = "UPDATE Dbo_Company_Profiles SET ContactId=@ContactId WHERE CP_ID=@CP_ID";
            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new { ContactId = contactId, CP_ID = cpId }, transaction);
        }

        public async Task<bool> IsCorporateAccountExistAsync(string companyName, IDbTransaction transaction)
        {
            var query = @"SELECT COUNT(1) 
                  FROM Dbo_Company_Profiles 
                  WHERE LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Name, ' Ltd.', ''), ' Limited', ''), ' Ltd', ''), ' Pvt.', ''), ' Pvt', ''), ' Co.', ''), ' Company', ''), ' ', '')) = 
                  LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@CompanyName, ' Ltd.', ''), ' Limited', ''), ' Ltd', ''), ' Pvt.', ''), ' Pvt', ''), ' Co.', ''), ' Company', ''), ' ', ''))";

            var connection = transaction.Connection;
            return await connection.ExecuteScalarAsync<int>(query, new { CompanyName = companyName }, transaction) > 0;
        }

        //public async Task<bool> IsUserNameExistAsync(string userName, IDbTransaction transaction)
        //{

        //    var query = @"SELECT COUNT(1) 
        //          FROM CorporateUserAccess 
        //          WHERE LOWER(User_Name) = LOWER(@UserName)";

        //    var connection = transaction.Connection;
        //    return await connection.ExecuteScalarAsync<int>(query, new { UserName = userName }, transaction) > 0;
        //}


        //USERNAME HAVE PROBLEM - - > NOT CHECKING
        public async Task<bool> IsUserNameExistAsync(string userName, IDbTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM CorporateUserAccess WHERE User_Name = @UserName";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { UserName = userName });
                return count > 0;
            }
        }

        public async Task InsertIndustryTypesAsync(long companyId, List<int> industryTypeIds, IDbTransaction transaction)
        {
            var query = "INSERT INTO IndustryWiseCompanies (CP_ID, Org_Type_ID) VALUES (@CompanyId, @IndustryTypeId)";
            var connection = transaction.Connection;

            foreach (var industryTypeId in industryTypeIds)
            {
                await connection.ExecuteAsync(query, new { CompanyId = companyId, IndustryTypeId = industryTypeId }, transaction);
            }
        }

 
        public async Task<bool> ValidateCaptchaAsync(string sessionValue, string captchaValue)
        {
            // Example for accessing session and comparing captcha
            var sessionCaptcha = "dffdf"; // Replace with real session access
            if (string.IsNullOrEmpty(sessionCaptcha)) return false;

            sessionCaptcha = sessionCaptcha.Replace("i", "I");
            captchaValue = captchaValue.Replace("i", "I");

            return string.Equals(sessionCaptcha, captchaValue, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsHackingAttemptAsync(string input)
        {
            string[] hackingPatterns = { "'", "%", "response.end", ">", "<", "\"" };

            foreach (var pattern in hackingPatterns)
            {
                if (input.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
