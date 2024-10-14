using BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities;
using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> InsertCompany([FromBody] CompanyInsertRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var companyProfile = new CompanyProfile
                {
                    Name = request.Name,
                    NameBng = request.NameBng,
                    Business = request.Business,
                    Address = request.Address,
                    AddressBng = request.AddressBng,
                    BillContact = request.BillContact,
                    City = request.City,
                    Country = request.Country,
                    Web = request.Web,
                    UpdatedDate = DateTime.Now,
                    Area = request.Area,
                    IDcode = RandomString(8),
                    OfflineCom = 0,
                    LicenseNo = request.LicenseNo,
                    RLNo = request.RLNo,
                    ThanaId = request.ThanaId,
                    DistrictId = request.DistrictId,
                    ContactPerson = request.ContactPerson,
                    Designation = request.Designation,
                    Phone = request.Phone,
                    Email = request.Email,
                    IsFacilityPWD = request.IsFacilityPWD,
                    Established = request.Established,
                    MinEmp = request.MinEmp,
                    MaxEmp = request.MaxEmp,
                    IsEntrepreneur = request.IsEntrepreneur
                };

                var contactPerson = new ContactPerson
                {
                    ContactName = request.ContactPerson,
                    Designation = request.Designation,
                    CurrentUser = true,
                    BillingContact = false,
                    Mobile = request.Phone,
                    Email = request.Email
                };

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
            catch (System.Exception ex)
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
    public class CompanyInsertRequest
    {
        public string Name { get; set; }
        public string NameBng { get; set; }
        public string Business { get; set; }
        public string Address { get; set; }
        public string AddressBng { get; set; }
        public string BillContact { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Web { get; set; }
        public string Area { get; set; }
        public string LicenseNo { get; set; }
        public string RLNo { get; set; }
        public int? ThanaId { get; set; }
        public int? DistrictId { get; set; }
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int IsFacilityPWD { get; set; }
        public int? Established { get; set; }
        public int? MinEmp { get; set; }
        public int? MaxEmp { get; set; }
        public int? IsEntrepreneur { get; set; }
    }
}
