namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class CompanyProfile
    {
        public int CorporateAccountID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBangla { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyAddressBng { get; set; }
        public int CompanyEstablished { get; set; }
        public string CompanySize { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string OutsideCity { get; set; }
        public string IndustryType { get; set; }
        public string BusinessDescription { get; set; }  // Match this with SQL query
        public string BusinessLicenseNo { get; set; }
        public string RLNo { get; set; }
        public string WebsiteUrl { get; set; }  // Match this with SQL query
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }  // Ensure ContactMobile matches in SQL query
        public bool DisabilitiesFacility { get; set; }  // Match this with IsFacilityPWD
        public int DisabilityInclusionPolicy { get; set; }
        public List<int> DisabilityTypes { get; set; }
        public string Description { get; set; }
        public bool IsEntrepreneur { get; set; }
        public bool FacilityForDisability { get; set; }
        public bool HaveDisabilityInclusion { get; set; }
        public int ProvideTrainingForEmployee { get; set; }
        public List<int> FacilitiesCompanyHave { get; set; }
        public string ThanaId { get; set; }    // Add if needed for the query
        public string Country { get; set; }
        public string DistrictId { get; set; } // Add if needed for the query
        public string IDCode { get; set; }     // Add if needed for the query
        public bool OfflineCom { get; set; }   // Add if needed for the query

        public int MinEmployee { get; set; }  // Minimum employee count
        public int MaxEmployee { get; set; }  // Maximum employee count


    }
}
