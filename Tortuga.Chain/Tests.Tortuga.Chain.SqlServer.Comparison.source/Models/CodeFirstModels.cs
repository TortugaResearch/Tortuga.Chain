namespace Tests.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CodeFirstModels : DbContext
    {
        public CodeFirstModels()
            : base("name=CodeFirstModels")
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Employee1)
                .WithOptional(e => e.Employee2)
                .HasForeignKey(e => e.ManagerKey);

            modelBuilder.Entity<Customer>()
                .Property(e => e.FullName)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .Property(e => e.State)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);
        }
    }
}
