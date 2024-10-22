namespace BdJobsCorporate_Corporate_Insert.DTO.DTOs
{
    public class RegisterCompanyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long CorporateAccountID { get; set; }  
        public string UserName { get; set; }         
        public string ContactEmail { get; set; }
        public string Passwrd { get; set; }
    }
}
