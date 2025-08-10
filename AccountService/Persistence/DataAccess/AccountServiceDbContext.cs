using AccountService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Persistence.DataAccess;

public sealed class AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; init; }

    protected override void OnModelCreating(ModelBuilder b)
    {        
        base.OnModelCreating(b);
        
        ConfigureEntity(b.Entity<Account>());
        ConfigureEntity(b.Entity<Transaction>());
    }

    private static void ConfigureEntity(EntityTypeBuilder<Transaction> b)
    {
        b.HasKey(t => t.Id);
        b.HasIndex(t => t.TimeUtc)
            .HasMethod("gist");
        b.HasIndex(t => new { t.AccountId, t.TimeUtc });
        
        b.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired();
        
        b.Property(t => t.AccountId)
            .IsRequired();

        b.Property(t => t.Type)
            .HasConversion(to => (short)to, from => (TransactionType)from);
        
        b.OwnsOne(t => t.Amount, ConfigureCurrencyMapping);

        b.Property(t => t.Description)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(512);

        b.Property(t => t.TimeUtc)
            .IsRequired()
            .IsConcurrencyToken();

        b.ToTable("Transactions");
    }
    
    private static void ConfigureEntity(EntityTypeBuilder<Account> b)
    {
        b.HasKey(a => a.Id);
        b.HasIndex(a => a.Id)
            .HasMethod("hash");
        
        b.Property(a => a.Id)
            .IsRequired();

        b.Property(a => a.OwnerId)
            .IsRequired();

        b.Property(a => a.CreationTimeUtc)
            .IsRequired();

        b.Property(a => a.Type)
            .IsRequired()
            .HasConversion(to => (short)to, from => (AccountType)from);

        b.OwnsOne(a => a.Balance, ConfigureCurrencyMapping);

        b.Property(a => a.InterestRate)
            .HasConversion(
                to => to == null ? default(decimal?) : to.Value,
                from => from.HasValue ? new AccountInterestRate(from.Value) : null
            );

        b.Property(a => a.CreationTimeUtc)
            .IsRequired();
        b.Property(a => a.ModifiedAt)
            .IsRequired()
            .IsConcurrencyToken();

        b.HasMany<Transaction>(t => t.TransactionHistory)
            .WithOne()
            .HasForeignKey(t => t.AccountId);
        b.Metadata
            .FindNavigation(nameof(Account.TransactionHistory))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureCurrencyMapping<TEntity>(OwnedNavigationBuilder<TEntity, Currency> builder)
    where TEntity : class
    {
        builder.Property(c => c.Code)
            .HasColumnName(nameof(CurrencyCode))
            .IsRequired()
            .HasConversion(to => to.Value, from => new CurrencyCode(from));
        builder.Property(c => c.Amount)
            .HasColumnName(nameof(Currency.Amount))
            .IsRequired();
    }  
}