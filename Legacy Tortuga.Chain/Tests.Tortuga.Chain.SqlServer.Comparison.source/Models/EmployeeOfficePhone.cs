using System.ComponentModel.DataAnnotations;

namespace Tests.Models
{
    public class EmployeeOfficePhone
    {
        [Key]
        public int EmployeeKey { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [StringLength(15)]
        public string OfficePhone { get; set; }
    }
}
