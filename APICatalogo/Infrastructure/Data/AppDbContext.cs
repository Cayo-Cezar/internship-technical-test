using APICatalogo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace APICatalogo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Arquivos>? Arquivos { get; set; }
}

