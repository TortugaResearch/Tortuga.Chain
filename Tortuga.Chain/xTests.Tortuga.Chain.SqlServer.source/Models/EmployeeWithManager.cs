using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
    public class EmployeeWithManager
    {
        public int? EmployeeKey { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }


        [Decompose("Manager")]
        public Manager Manager { get; set; }

        [Decompose]
        public AuditInfo AuditInfo { get; set; }

    }

    public class Manager
    {
        public int? EmployeeKey { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }


        [Decompose]
        public AuditInfo AuditInfo { get; set; }

    }
}
