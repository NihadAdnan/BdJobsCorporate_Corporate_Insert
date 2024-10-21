using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class ContactPerson
    {
        public int CorporateAccountID { get; set; }
        public int CP_ID { get; set; }
        public string ContactName { get; set; }
        public string Designation { get; set; }
        public bool CurrentUser { get; set; }
        public bool BillingContact { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
