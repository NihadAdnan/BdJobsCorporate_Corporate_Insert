using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using BdJobsCorporate_Corporate_Insert.DTO.DTOs;

namespace BdJobsCorporate_Corporate_Insert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertCompany([FromBody] RegisterCompanyRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            if (string.IsNullOrEmpty(request.txtCompanyName) || string.IsNullOrEmpty(request.txtUserName))
            {
                return BadRequest("Company Name and User Name are required.");
            }

            if (request.txtCompanyName.Length > 100 || request.txtUserName.Length > 100)
            {
                return BadRequest("Company Name and Contact Email length should not exceed 100 characters.");
            }

            try
            {
                bool isCorporateAccountExist = await _companyService.IsCorporateAccountExist(request.txtCompanyName);
                bool isUserNameExist = await _companyService.IsUserNameExist(request.txtUserName);

                if (isCorporateAccountExist)
                {
                    return Conflict("Corporate account already exists.");
                }

                if (isUserNameExist)
                {
                    return Conflict("User with this user name already exists.");
                }

                var companyProfile = new CompanyProfile
                {
                    CompanyName = request.txtCompanyName,
                    CompanyBangla = request.txtCompanyBangla,
                    CompanyAddress = request.txtCompanyAddress,
                    CompanyAddressBng = request.txtCompanyAddressBng,
                    City = request.cboCity,
                    Area = request.cboArea,
                    Country = request.Country,
                    WebsiteUrl = request.website_url,
                    BusinessLicenseNo = request.business_license_no,
                    RLNo = request.rl_no,
                    CompanyEstablished = int.Parse(request.txtCompanyEstablished),
                    IsEntrepreneur = !string.IsNullOrEmpty(request.hidEntrepreneur),
                    DisabilitiesFacility = request.DisabilitiesFacility == 1,
                    FacilityForDisability = request.facilityForDisability == 1,
                    MinEmployee = ParseEmployeeRange(request.ComSize).Item1,
                    MaxEmployee = ParseEmployeeRange(request.ComSize).Item2,
                    ProvideTrainingForEmployee = request.provideTrainingForEmployee,
                    DisabilityTypes = request.whatFacilityCompanyHave.Split(',').Select(int.Parse).ToList(),
                    IDCode = RandomString(8) 
                };

                var contactPerson = new ContactPerson
                {
                    ContactName = request.txtContactPerson,
                    Designation = request.txtDesignation,
                    CurrentUser = true,
                    BillingContact = false,
                    Mobile = request.txtContactMobile,
                    Email = request.txtContactEmail
                };

                var corporateUserAccess = new CorporateUserAccess
                {
                    UserName = request.txtUserName 
                };

               
                bool result = await _companyService.InsertRecordAsync(companyProfile, contactPerson, corporateUserAccess);

                if (result)
                {
                    return Ok(new RegisterCompanyResponse
                    {
                        Success = true,
                        Message = "Company inserted successfully.",
                        CorporateAccountID = companyProfile.CorporateAccountID,
                        UserName = corporateUserAccess.UserName,
                        ContactEmail = request.txtContactEmail
                    });
                }
                else
                {
                    return StatusCode(500, "An error occurred while inserting the company.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

  
        private (int, int) ParseEmployeeRange(string employeeRange)
        {
            if (string.IsNullOrEmpty(employeeRange)) return (0, 0);

            var parts = employeeRange.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out var min) && int.TryParse(parts[1], out var max))
            {
                return (min, max);
            }

            return (0, 0);
        }
    }
}
