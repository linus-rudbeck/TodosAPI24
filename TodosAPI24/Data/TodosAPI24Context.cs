using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodosAPI24.Models;

namespace TodosAPI24.Data
{
    public class TodosAPI24Context(DbContextOptions options) : IdentityDbContext<CustomUser>(options)
    {
        public DbSet<Todo> Todos { get; set; }

    }
}
