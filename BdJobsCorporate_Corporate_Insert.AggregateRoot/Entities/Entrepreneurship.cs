using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class Entrepreneurship
    {
        public int CP_ID { get; set; }
        public DateTime ValidityDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserID { get; set; }
    }
}
