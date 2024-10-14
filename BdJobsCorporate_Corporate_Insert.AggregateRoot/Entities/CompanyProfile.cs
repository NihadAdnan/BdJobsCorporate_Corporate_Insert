using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class CompanyProfile
    {
        public long CP_ID { get; set; }
        public string Name { get; set; }
        public string NameBng { get; set; }
        public string Business { get; set; }
        public string Address { get; set; }
        public string AddressBng { get; set; }
        public string BillContact { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Web { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Area { get; set; }
        public string IDcode { get; set; }
        public int OfflineCom { get; set; }
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
        public int? ContactId { get; set; }
    }
}
