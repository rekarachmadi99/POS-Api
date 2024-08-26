using Microsoft.EntityFrameworkCore;
using POS_Api.Models;

namespace POS_Api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<EmployeeAccount> EmployeeAccount { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<TransactionDetail> TransactionDetail { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
    }
}
