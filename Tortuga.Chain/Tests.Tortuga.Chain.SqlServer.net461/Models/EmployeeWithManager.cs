using System;
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
        public Employee Manager { get; set; }

        [Decompose]
        public AuditInfo AuditInfo { get; set; }

    }
}
