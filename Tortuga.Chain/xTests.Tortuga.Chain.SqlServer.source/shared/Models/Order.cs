using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
    public class Order
    {
        public int? OrderKey { get; set; }

        [IgnoreOnUpdate]
        public int CustomerKey { get; set; }
    }
}
