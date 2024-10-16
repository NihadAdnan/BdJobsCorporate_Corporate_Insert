using System.Collections.Generic;

namespace BdJobsCorporate_Corporate_Insert.DTO.DTOs
{
    public class CorporateAccountRequest
    {
        public string Name { get; set; }
        public string NameBng { get; set; }
        public string Business { get; set; }
        public string Address { get; set; }
        public string AddressBng { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Web { get; set; } 
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
        public string Area { get; set; } 
        public List<int> IndustryTypeIds { get; set; } 
    }
}
