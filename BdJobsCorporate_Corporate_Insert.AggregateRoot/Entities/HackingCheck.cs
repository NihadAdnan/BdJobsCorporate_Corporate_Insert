using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Entities
{
    public class HackingCheck
    {
        public string InputString { get; set; }
        public string[] HackingPatterns { get; set; } = { "'", "%", "response.end", ">", "<", "\"" };

        public bool IsHackingAttempt()
        {
            foreach (var pattern in HackingPatterns)
            {
                if (InputString.Contains(pattern))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
