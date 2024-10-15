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
        public string Web { get; set; } // Added to match CompanyProfile
        public string LicenseNo { get; set; } // Added to match CompanyProfile
        public string RLNo { get; set; } // Added to match CompanyProfile
        public int? ThanaId { get; set; } // Added to match CompanyProfile
        public int? DistrictId { get; set; } // Added to match CompanyProfile
        public string ContactPerson { get; set; } // Added for ContactPerson
        public string Designation { get; set; } // Added for ContactPerson
        public string Phone { get; set; } // Added for ContactPerson
        public string Email { get; set; } // Added for ContactPerson
        public int IsFacilityPWD { get; set; } // Added to match CompanyProfile
        public int? Established { get; set; } // Added to match CompanyProfile
        public int? MinEmp { get; set; } // Added to match CompanyProfile
        public int? MaxEmp { get; set; } // Added to match CompanyProfile
        public int? IsEntrepreneur { get; set; } // Added to match CompanyProfile
        public string Area { get; set; } // Added to match CompanyProfile
        public List<int> IndustryTypeIds { get; set; } // Optional for your needs
    }
}
