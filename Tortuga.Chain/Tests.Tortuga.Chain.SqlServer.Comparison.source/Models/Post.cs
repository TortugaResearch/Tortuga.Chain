using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int OwnerId { get; set; }
        [Decompose("Owner")]
        public User Owner { get; set; }
    }
}



