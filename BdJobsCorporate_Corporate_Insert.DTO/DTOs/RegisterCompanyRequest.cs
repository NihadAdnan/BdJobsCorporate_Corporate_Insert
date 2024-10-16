using System;

namespace BdJobsCorporate_Corporate_Insert.DTO.DTOs
{
    public class RegisterCompanyRequest
    {
        // User-related data
        public string txtUserName { get; set; }
        public string txtPassword { get; set; }
        public string txtConfirmPassword { get; set; }

        // Company-related data
        public string txtCompanyName { get; set; }
        public string txtCompanyBangla { get; set; }
        public string txtCompanyEstablished { get; set; }
        public string ComSize { get; set; }
        public string Country { get; set; }
        public string cboCity { get; set; }
        public string cboArea { get; set; }
        public string txtCompanyAddress { get; set; }
        public string txtCompanyAddressBng { get; set; }
        public string website_url { get; set; }

        // Contact person details
        public string txtContactPerson { get; set; }
        public string txtDesignation { get; set; }
        public string txtContactEmail { get; set; }
        public string txtContactMobile { get; set; }

        // Additional company information
        public string business_license_no { get; set; }
        public string rl_no { get; set; }
        public int DisabilitiesFacility { get; set; }
        public string hidEntrepreneur { get; set; }
        public int facilityForDisability { get; set; }
        public int provideTrainingForEmployee { get; set; }
        public string whatFacilityCompanyHave { get; set; }

        // Other metadata
        public string CaptchaCode { get; set; }
        public bool PolicyAccepted { get; set; }
    }
}
