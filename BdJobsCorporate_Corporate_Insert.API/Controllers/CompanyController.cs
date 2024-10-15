using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
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
        public async Task<IActionResult> InsertCompany([FromBody] CorporateAccountRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            // Validation checks
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Name and Email are required.");
            }

            if (request.Name.Length > 100 || request.Email.Length > 100)
            {
                return BadRequest("Name and Email length should be valid.");
            }

            try
            {
                // Check if the corporate account or user already exists
                bool isCorporateAccountExist = await _companyService.IsCorporateAccountExist(request.Name);
                bool isUserNameExist = await _companyService.IsUserNameExist(request.Email);

                if (isCorporateAccountExist)
                {
                    return BadRequest("Corporate account already exists.");
                }

                if (isUserNameExist)
                {
                    return BadRequest("User with this email already exists.");
                }

                // Create the company profile
                var companyProfile = new CompanyProfile
                {
                    Name = request.Name,
                    NameBng = request.NameBng,
                    Business = request.Business,
                    Address = request.Address,
                    AddressBng = request.AddressBng,
                    City = request.City,
                    Country = request.Country,
                    Web = request.Web,
                    UpdatedDate = DateTime.Now,
                    Area = request.Area,
                    IDcode = RandomString(8),  // Generate IDcode
                    OfflineCom = 0,
                    LicenseNo = request.LicenseNo,
                    RLNo = request.RLNo,
                    ThanaId = request.ThanaId,
                    DistrictId = request.DistrictId,
                    IsFacilityPWD = request.IsFacilityPWD,
                    Established = request.Established,
                    MinEmp = request.MinEmp,
                    MaxEmp = request.MaxEmp,
                    IsEntrepreneur = request.IsEntrepreneur
                };

                // Create the contact person
                var contactPerson = new ContactPerson
                {
                    ContactName = request.ContactPerson,
                    Designation = request.Designation,
                    CurrentUser = true,
                    BillingContact = false,
                    Mobile = request.Phone,
                    Email = request.Email
                };

                // Attempt to insert the record
                bool result = await _companyService.InsertRecordAsync(companyProfile, contactPerson);

                if (result)
                {
                    return Ok(new { Message = "Company inserted successfully." });
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
            var random = new System.Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
