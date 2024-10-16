namespace BdJobsCorporate_Corporate_Insert.DTO.DTOs
{
    public class RegisterCompanyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long CorporateAccountID { get; set; }  // ID of the newly created company (set to long if required)
        public string UserName { get; set; }         // Username of the created account
        public string ContactEmail { get; set; }
    }
}
