using System;
using System.Collections.Generic;

namespace Sonneville.Utilities.Test.Persistence.v1
{
    class SampleConfig
    {
        public string A { get; set; }

        public int B { get; set; }

        public long C { get; set; }

        public double D { get; set; }

        public decimal E { get; set; }

        public TimeSpan F { get; set; }

        public HashSet<string> G { get; set; }

        public Dictionary<Type, object> H { get; set; }
            
        public List<string> I { get; set; }
    }
}