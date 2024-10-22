using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
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

        //Fixed it
        public async Task InsertCompanyProfileAsync(CompanyProfile company, IDbTransaction transaction)
        {
            var query = @"
    INSERT INTO Dbo_Company_Profiles 
    (CP_ID, Name, NameBng, Business, Address, AddressBng, bill_Contact, City, Country, Web, Updated_date, Area, IDcode, OfflineCom, LicenseNo, 
     RLNo, ThanaId, DistrictId, Contact_Person, Designation, Phone, E_Mail, IsFacilityPWD, Established, MinEmp, MaxEmp, IsEntrepreneur)
    VALUES
    (@CorporateAccountID, @CompanyName, @CompanyBangla, @BusinessDescription, @Address, @CompanyAddressBng, @BillContact, @City, @Country, 
     @WebsiteUrl, @UpdatedDate, @Area, @IDCode, 0, @BusinessLicenseNo, @RecruitingRLNO, @ThanaId, @DistrictId, @ContactPerson, @Designation, 
     @Phone, @Email, @IsFacilityPWD, @Established, @MinEmp, @MaxEmp, @IsEntrepreneur)";

            var connection = transaction.Connection;

            string address = company.Country == "Bangladesh"
                             ? company.CompanyAddress ?? string.Empty
                             : company.OutsideBdAddress ?? string.Empty;

            string billContact = address.Substring(0, Math.Min(150, address.Length));

            string facilityForDisability = company.FacilityForDisability ? "1" : "0";

            await connection.ExecuteAsync(query, new
            {
                company.CorporateAccountID,
                company.CompanyName,
                company.CompanyBangla,
                company.BusinessDescription,
                Address = address,
                company.CompanyAddressBng,
                BillContact = billContact,
                City = company.Country == "Bangladesh" ? company.CityName : company.OutsideBdCity,
                company.Country,
                company.WebsiteUrl,
                UpdatedDate = DateTime.Now,
                company.Area,
                company.IDCode,
                company.BusinessLicenseNo,
                RecruitingRLNO = string.IsNullOrEmpty(company.RecruitingRLNO) ? null : company.RecruitingRLNO,
                company.ThanaId,
                company.DistrictId,
                company.ContactPerson,
                company.Designation,
                company.Phone,
                company.Email,
                IsFacilityPWD = facilityForDisability,
                company.Established,
                company.MinEmp,
                company.MaxEmp,
                company.IsEntrepreneur
            }, transaction);
        }


        //Fixed it
        public async Task InsertContactPersonAsync(ContactPerson contactPerson, IDbTransaction transaction)
        {
            var query = @"
        INSERT INTO ContactPersons 
        (CP_ID, ContactName, Designation, CurrentUser, BillingContact, Mobile, Email) 
        VALUES 
        (@CorporateAccountID, @ContactPerson, @Designation, 1, 0, @ContactMobile, @ContactEmail)";

            var connection = transaction.Connection;

            await connection.ExecuteAsync(query, new
            {
                CorporateAccountID = contactPerson.CorporateAccountID, 
                ContactPerson = contactPerson.ContactName,              
                Designation = contactPerson.Designation,
                ContactMobile = contactPerson.Mobile,
                ContactEmail = contactPerson.Email
            }, transaction);
        }

        //fixed it
        public async Task<int> GetContactIdAsync(long cpId, IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction.Connection == null) throw new ArgumentNullException(nameof(transaction.Connection));

            var query = "SELECT ContactId FROM ContactPersons WHERE CP_ID = @CP_ID AND CurrentUser = 1";
            var connection = transaction.Connection;

            try
            {
                return await connection.ExecuteScalarAsync<int>(query, new { CP_ID = cpId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the ContactId.", ex);
            }
        }

        //fixed it
        public async Task UpdateCompanyProfileContactIdAsync(long cpId, int contactId, IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction.Connection == null) throw new ArgumentNullException(nameof(transaction.Connection));

            if (cpId == 0)
            {
                throw new ArgumentException("CP_ID cannot be null or zero.", nameof(cpId));
            }

            var query = "UPDATE Dbo_Company_Profiles SET ContactId = @ContactId WHERE CP_ID = @CP_ID";
            var connection = transaction.Connection;

            try
            {
                await connection.ExecuteAsync(query, new { ContactId = contactId, CP_ID = cpId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the company profile's ContactId.", ex);
            }
        }


        //New code added
        public async Task ProcessNewIndustryAsync(string strNewIndustry, string industryTypeIds, IDbTransaction transaction)
        {
            //if (!string.IsNullOrEmpty(strNewIndustry))
            //{
            //    strNewIndustry = strNewIndustry.Substring(0, strNewIndustry.Length - 3);

            //    var arrNewIndustry = strNewIndustry.Split('#');
            //    long intOrgId;

            //    foreach (var element in arrNewIndustry)
            //    {
            //        var arrSingleNewIndustryData = element.Split('_');

            //        if (industryTypeIds.Contains($",{arrSingleNewIndustryData[0]},"))
            //        {
            //            var isOrgExistSql = "SELECT ORG_TYPE_ID FROM bdjDataset..ORG_TYPES WHERE ORG_TYPE_NAME = @OrgTypeName";
            //            var orgTypeName = arrSingleNewIndustryData[1];

            //            var intOrgIdResult = await RetrieveDataAsync(isOrgExistSql, new { OrgTypeName = orgTypeName }, transaction);

            //            if (intOrgIdResult != null)
            //            {
            //                intOrgId = Convert.ToInt64(intOrgIdResult);
            //            }
            //            else
            //            {
            //                var strMaxOrgIdSQL = "SELECT MAX(ORG_TYPE_ID) FROM bdjDataset..ORG_TYPES";
            //                var maxOrgIdResult = await RetrieveDataAsync(strMaxOrgIdSQL, null, transaction);

            //                if (maxOrgIdResult == null)
            //                {
            //                    intOrgId = 1;
            //                }
            //                else
            //                {
            //                    intOrgId = Convert.ToInt64(maxOrgIdResult) + 1;
            //                }

            //                var strInsertOrg = @"
            //            INSERT INTO bdjDataset..ORG_TYPES (ORG_TYPE_ID, ORG_TYPE_NAME, IndustryId, UserDefined)
            //            VALUES (@OrgTypeId, @OrgTypeName, @IndustryId, 1)";

            //                var industryId = Convert.ToInt32(arrSingleNewIndustryData[2]);  

            //                await ExecuteQueryAsync(strInsertOrg, new
            //                {
            //                    OrgTypeId = intOrgId,
            //                    OrgTypeName = arrSingleNewIndustryData[1],
            //                    IndustryId = industryId
            //                }, transaction);
            //            }

            //            industryTypeIds = industryTypeIds.Replace($",{arrSingleNewIndustryData[0]},", $",{intOrgId},");
            //        }
            //    }
            //}
        }

        private async Task<object> RetrieveDataAsync(string query, object parameters, IDbTransaction transaction)
        {
            var connection = transaction.Connection;
            return await connection.ExecuteScalarAsync(query, parameters, transaction);
        }

        private async Task ExecuteQueryAsync(string query, object parameters, IDbTransaction transaction)
        {
            var connection = transaction.Connection;
            await connection.ExecuteAsync(query, parameters, transaction);
        }

        //Fixed it
        public async Task InsertIndustryTypesAsync(long companyId, List<int> industryTypeIds, IDbTransaction transaction)
        {
            if (industryTypeIds == null || industryTypeIds.Count == 0)
            {
                return; 
            }

            StringBuilder values = new StringBuilder();

            for (int i = 0; i < industryTypeIds.Count; i++)
            {
                if (values.Length > 0)
                {
                    values.Append(",");
                }

                values.Append($"({companyId}, {industryTypeIds[i]})");
            }

            string query = $"INSERT INTO IndustryWiseCompanies (CP_ID, Org_Type_ID) VALUES {values}";

            var connection = transaction.Connection;

            try
            {
                await connection.ExecuteAsync(query, null, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting industry types into IndustryWiseCompanies.", ex);
            }
        }


        //fixed it
        public async Task InsertUserAccessAsync(long companyId, int contactId, string userName, string password, IDbTransaction transaction)
        {
            var connection = transaction.Connection;

            string hashedPassword = GenerateSHA256Hash(password);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new Exception("Unable to generate hashed password.");
            }

            var query = @"
        INSERT INTO CorporateUserAccess (CP_ID, ContactId, AdminUser, CreatedOn, User_Name, Passwrd)
        VALUES(@CompanyId, @ContactId, 1, GETDATE(), @UserName, @Passwrd)";

            try
            {
                await connection.ExecuteAsync(query, new
                {
                    CompanyId = companyId,
                    ContactId = contactId,
                    UserName = userName,
                    Passwrd = hashedPassword
                }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting data into CorporateUserAccess", ex);
            }
        }

        private string GenerateSHA256Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var sb = new System.Text.StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("X2")); 
                }
                return sb.ToString();
            }
        }


        //Fixed it
        public async Task InsertBusinessDetailsAsync(long companyId, string businessName, string businessDetail, int postedBy, IDbTransaction transaction)
        {
            if (!string.IsNullOrEmpty(businessDetail))
            {
                var query = @"
            INSERT INTO COMPANY_BUSINESS (CP_ID, BusinessName, BusinessDetail, PostedBy, CreatedOn, IsAlternet)
            VALUES (@CP_ID, @BusinessName, @BusinessDetail, @PostedBy, @CreatedOn, 0)";

                var connection = transaction.Connection;

                await connection.ExecuteAsync(query, new
                {
                    CP_ID = companyId,
                    BusinessName = businessName,
                    BusinessDetail = businessDetail,
                    PostedBy = postedBy,
                    CreatedOn = DateTime.Now 
                }, transaction);
            }
        }

        //Fixed it
        public async Task InsertEntrepreneurshipAsync(long corporateAccountId, int contactId, bool isEntrepreneur, IDbTransaction transaction)
        {
            var connection = transaction.Connection;

            if (isEntrepreneur)
            {
                string isEntpQuery = "SELECT COUNT(1) FROM edp.Entrepreneurship WHERE CP_ID = @CorporateAccountId";
                var entrepreneurExists = await connection.ExecuteScalarAsync<int>(isEntpQuery, new { CorporateAccountId = corporateAccountId }, transaction);

                if (entrepreneurExists == 0)
                {
                    string sqlUserIdQuery = "SELECT UserId FROM CorporateUserAccess WHERE ContactId = @ContactId";
                    var userId = await connection.ExecuteScalarAsync<int?>(sqlUserIdQuery, new { ContactId = contactId }, transaction) ?? 0;

                    var validityDate = DateTime.Now.AddMonths(2);

                    string sqlInsertEntrepreneurship = @"
                INSERT INTO edp.Entrepreneurship (CP_ID, ValidityDate, CreatedOn, UserID)
                VALUES (@CorporateAccountId, @ValidityDate, GETDATE(), @UserId)";

                    await connection.ExecuteAsync(sqlInsertEntrepreneurship, new
                    {
                        CorporateAccountId = corporateAccountId,
                        ValidityDate = validityDate,
                        UserId = userId
                    }, transaction);
                }
            }
        }


    }
}
