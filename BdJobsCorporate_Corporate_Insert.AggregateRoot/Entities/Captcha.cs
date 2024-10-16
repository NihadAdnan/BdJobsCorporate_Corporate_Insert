using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class Captcha
    {
        public string SessionValue { get; set; }
        public string CaptchaValue { get; set; }
    }
}
