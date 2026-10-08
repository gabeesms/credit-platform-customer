using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Persistence;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.BirthDate)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            entity.Property(c => c.UpdatedAt)
                .IsRequired();

            entity.Property(c => c.CPF)
                .HasConversion(
                    cpf => cpf.Value,
                    value => CustomerService.Domain.ValueObjects.Cpf.Create(value))
                .HasMaxLength(11)
                .IsRequired();

            entity.HasIndex(c => c.CPF)
                .IsUnique();
        });
    }
}