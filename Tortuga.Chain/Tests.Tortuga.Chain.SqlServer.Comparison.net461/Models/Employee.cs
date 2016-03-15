namespace Tests.Models
{
    using global::Tortuga.Anchor.Modeling;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HR.Employee")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            Employee1 = new HashSet<Employee>();
        }

        [Key]
        public int EmployeeKey { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public int? ManagerKey { get; set; }

        [StringLength(15)]
        public string OfficePhone { get; set; }

        [StringLength(15)]
        public string CellPhone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employee1 { get; set; }

        public virtual Employee Employee2 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] //Needed by EF
        [IgnoreOnInsert, IgnoreOnUpdate] //Needed by Chain
        public DateTime? CreatedDate { get; set; }


    }
}
