using APICatalogo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Arquivos>? Arquivos { get; set; }
}

