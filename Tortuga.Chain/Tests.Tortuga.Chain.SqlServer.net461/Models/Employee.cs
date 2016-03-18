
using System;
using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
    public class Employee
    {
        public int? EmployeeKey { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public int? ManagerKey { get; set; }

        [IgnoreOnInsert, IgnoreOnUpdate]
        public DateTime? CreatedDate { get; set; }

        [IgnoreOnUpdate]
        public DateTime? UpdatedDate { get; set; }
    }
}
