using System;
using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
    public class Dog
    {
        public int? Age { get; set; }
        [IgnoreOnInsert, IgnoreOnUpdate]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float? Weight { get; set; }

        public int IgnoredProperty { get { return 1; } }
    }
}



