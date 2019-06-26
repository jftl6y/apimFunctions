using System;
using System.Collections.Generic;
using System.Text;

namespace apimFunctions
{
    public class ApimUser
        {
            public string id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public Properties properties { get; set; }
        }

        public class Properties
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string state { get; set; }
            public DateTime registrationDate { get; set; }
            public object note { get; set; }
            public Identity[] identities { get; set; }
        }

        public class Identity
        {
            public string provider { get; set; }
            public string id { get; set; }
        }

    }

