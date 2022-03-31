using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication.Models
{
    public class ContactsContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public ContactsContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ADCLG1;Database=4281-15;Trusted_Connection=True;");
        }
    }

}
