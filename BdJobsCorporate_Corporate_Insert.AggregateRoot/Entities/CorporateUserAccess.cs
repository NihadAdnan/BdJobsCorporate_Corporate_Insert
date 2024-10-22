using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class CorporateUserAccess
    {
        public long CompanyId { get; set; }        
        public int ContactId { get; set; }
        public string UserName { get; set; }
        public string Passwrd { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
