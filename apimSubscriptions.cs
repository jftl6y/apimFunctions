
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.FastTrack
{ 
     public class ApimSubscriptions
        {
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public Properties properties { get; set; }
        }

        public class Properties
        {
            public string ownerId { get; set; }
            public string scope { get; set; }
            public string displayName { get; set; }
            public string state { get; set; }
            public DateTime createdDate { get; set; }
            public DateTime? startDate { get; set; }
            public object expirationDate { get; set; }
            public object endDate { get; set; }
            public object notificationDate { get; set; }
            public string primaryKey { get; set; }
            public string secondaryKey { get; set; }
            public object stateComment { get; set; }
            public bool allowTracing { get; set; }
        }

    }

