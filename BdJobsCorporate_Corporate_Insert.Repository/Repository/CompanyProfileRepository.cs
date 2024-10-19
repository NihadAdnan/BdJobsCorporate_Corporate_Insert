using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            var query = "SELECT ISNULL(MAX(CP_ID), 0) + 1 FROM Dbo_Company_Profiles";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<long>(query);
            }
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

        public async Task<bool> IsUserNameExistAsync(string userName, IDbTransaction transaction)
        {
            var query = @"SELECT COUNT(1) 
                  FROM CorporateUserAccess 
                  WHERE LOWER(User_Name) = LOWER(@UserName)";

            var connection = transaction.Connection;

            int userCount = await connection.ExecuteScalarAsync<int>(query, new { UserName = userName }, transaction);
            return userCount > 0;
        }

        public async Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO Dbo_Company_Profiles 
                (CP_ID, Name, NameBng, Business, Address, AddressBng, Bill_Contact, City, Country, Web, Updated_date, Area, IDcode, OfflineCom, LicenseNo, RLNo, ThanaId, DistrictId, Contact_Person, Designation, Phone, E_Mail, IsFacilityPWD, Established, MinEmp, MaxEmp, IsEntrepreneur)
                VALUES
                (@CorporateAccountID, @CompanyName, @CompanyBangla, @BusinessDescription, @CompanyAddress, @CompanyAddressBng, @BillContact, @City, @Country, @WebsiteUrl, @UpdatedDate, @Area, @IDCode, 0, @BusinessLicenseNo, @RLNo, @ThanaId, @DistrictId, @ContactPerson, @Designation, @ContactPhone, @ContactEmail, @DisabilitiesFacility, @Established, @MinEmployee, @MaxEmployee, @IsEntrepreneur)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                company.CorporateAccountID,
                company.CompanyName,
                company.CompanyBangla,
                company.BusinessDescription,
                Address = company.Country == "Bangladesh",
                company.Country,
                company.WebsiteUrl,
                company.Area,
                company.IDCode,
                company.BusinessLicenseNo,
                company.RLNo,
                company.ThanaId,
                company.DistrictId,
                company.ContactPerson,
                company.Designation,
                company.ContactMobile,
                company.ContactEmail,
                company.DisabilitiesFacility,
                Established = DateTime.Now,
                company.MinEmployee,
                company.MaxEmployee,
                IsEntrepreneur = company.IsEntrepreneur ? 1 : 0
            }, transaction);
        }

        public async Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO ContactPersons
                (CP_ID, ContactName, Designation, CurrentUser, BillingContact, Mobile, Email)
                VALUES
                (@CP_ID, @ContactName, @Designation, 1, 0, @Mobile, @Email)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                contactPerson.CP_ID,
                contactPerson.ContactName,
                contactPerson.Designation,
                contactPerson.Mobile,
                contactPerson.Email
            }, transaction);
        }

        public async Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction)
        {
            var query = "SELECT ContactId FROM ContactPersons WHERE CP_ID=@CP_ID AND CurrentUser=1";
            var connection = transaction.Connection;
            return await connection.ExecuteScalarAsync<int>(query, new { CP_ID = cpId }, transaction);
        }

        public async Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction)
        {
            var query = "UPDATE Dbo_Company_Profiles SET ContactId=@ContactId WHERE CP_ID=@CP_ID";
            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new { ContactId = contactId, CP_ID = cpId }, transaction);
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

        public async Task InsertUserAccessAsync(long companyId, int contactId, string userName, string password, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO CorporateUserAccess (CP_ID, ContactId, AdminUser, CreatedOn, User_Name, Passwrd)
                VALUES(@CompanyId, @ContactId, 1, GETDATE(), @UserName, @Passwrd)";
            var connection = transaction.Connection;
            var hashedPassword = GenerateSHA256Hash(password);

            await connection.ExecuteAsync(query, new
            {
                CompanyId = companyId,
                ContactId = contactId,
                UserName = userName,
                Passwrd = hashedPassword
            }, transaction);
        }

        public async Task InsertBusinessDetailsAsync(long companyId, string businessName, string businessDetail, int postedBy, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO COMPANY_BUSINESS (CP_ID, BusinessName, BusinessDetail, PostedBy, CreatedOn, IsAlternet)
                VALUES(@CP_ID, @BusinessName, @BusinessDetail, @PostedBy, GETDATE(), 0)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                CP_ID = companyId,
                BusinessName = businessName,
                BusinessDetail = businessDetail,
                PostedBy = postedBy
            }, transaction);
        }

        public async Task InsertEntrepreneurshipAsync(long companyId, int userId, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO edp.Entrepreneurship (CP_ID, ValidityDate, CreatedOn, UserID)
                VALUES(@CP_ID, @ValidityDate, GETDATE(), @UserID)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, new
            {
                CP_ID = companyId,
                ValidityDate = DateTime.Now.AddMonths(2),
                UserID = userId
            }, transaction);
        }

        // Helper function for password hashing
        public string GenerateSHA256Hash(string password) // Made public
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
