
namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class CompanyProfile
    {
        public int CorporateAccountID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBangla { get; set; }
        public string CompanyAddress { get; set; }
        public string OutsideBdAddress { get; set; }
        public string CompanyAddressBng { get; set; }
        public int Established { get; set; }  
        public string Size { get; set; }  
        public string CityName { get; set; } 
        public string Area { get; set; }
        public string OutsideBdCity { get; set; } 
        public string IndustryType { get; set; }
        public string BusinessDescription { get; set; }  
        public string BusinessLicenseNo { get; set; }
        public string RecruitingRLNO { get; set; }
        public string WebsiteUrl { get; set; }  
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; } 
        public bool IsFacilityPWD { get; set; } 
        public bool FacilityForDisability { get; set; } 
        public int DisabilityInclusionPolicy { get; set; }
        public List<int> DisabilityTypes { get; set; }
        public string Description { get; set; }
        public bool IsEntrepreneur { get; set; }
        public int ProvideTrainingForEmployee { get; set; }
        public List<int> FacilitiesCompanyHave { get; set; }
        public string ThanaId { get; set; }    
        public string Country { get; set; }
        public string DistrictId { get; set; }
        public string IDCode { get; set; }     
        public bool OfflineCom { get; set; }  

        public int MinEmp { get; set; } 
        public int MaxEmp { get; set; }

    }
}
