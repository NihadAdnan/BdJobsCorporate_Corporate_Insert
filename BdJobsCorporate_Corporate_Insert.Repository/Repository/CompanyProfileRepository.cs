using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using BdJobsCorporate_Corporate_Insert.Repository.Repository.Abstraction;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.Repository.Repository
{
    public class CompanyProfileRepository: ICompanyRepository
    {
        private readonly DapperDbContext _context;

        public CompanyProfileRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<long> GetNextCompanyIdAsync()
        {
            var query = "SELECT MAX(CP_ID) AS CompanyID FROM Dbo_Company_Profiles";
            using (var connection = _context.CreateConnection())
            {
                var companyId = await connection.ExecuteScalarAsync<long?>(query) ?? 0;
                return companyId + 1;
            }
        }

        public async Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO Dbo_Company_Profiles 
                (CP_ID, Name, NameBng, Business, Address, AddressBng, bill_contact, City, Country, Web, Updated_date, Area, IDcode, OfflineCom, LicenseNo, RLNo, ThanaId, DistrictId, CONTACT_PERSON, DESIGNATION, PHONE, E_MAIL, IsFacilityPWD, Established, MinEmp, MaxEmp, IsEntrepreneur)
                VALUES
                (@CP_ID, @Name, @NameBng, @Business, @Address, @AddressBng, @BillContact, @City, @Country, @Web, @UpdatedDate, @Area, @IDcode, @OfflineCom, @LicenseNo, @RLNo, @ThanaId, @DistrictId, @ContactPerson, @Designation, @Phone, @Email, @IsFacilityPWD, @Established, @MinEmp, @MaxEmp, @IsEntrepreneur)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, company, transaction);
        }

        public async Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction)
        {
            var query = @"
                INSERT INTO ContactPersons
                (CP_ID, ContactName, Designation, CurrentUser, BillingContact, Mobile, Email)
                VALUES
                (@CP_ID, @ContactName, @Designation, @CurrentUser, @BillingContact, @Mobile, @Email)";

            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, contactPerson, transaction);
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
    }
}
