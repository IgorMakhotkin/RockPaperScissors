using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;

namespace RockPaperScissors.Db;

public class GameDbContext : DbContext
{
    public DbSet<User?> Users { get; set; }
    
    public DbSet<Match?> Matches { get; set; }
    public DbSet<MatchHistory?> MatchHistories { get; set; }
    public DbSet<GameTransaction> GameTransactions { get; set; }
    
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Balance).IsRequired();
            
            entity.HasIndex(e => e.Username).IsUnique();
            
            entity.HasMany(e => e.GameTransactionsSent)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.FromUserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasMany(e => e.GameTransactionsReceived)
                .WithOne(e => e.Receiver)
                .HasForeignKey(e => e.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        
        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("Matches");
            entity.HasKey(e => e.MatchId);
            entity.Property(e => e.MatchBet);
            entity.Property(e => e.Status);
            
            entity.HasIndex(e => e.MatchId).IsUnique();
        });

        modelBuilder.Entity<MatchHistory>(entity =>
        {
            entity.ToTable("MatchHistory");
            entity.HasKey(e => e.MatchId);
            entity.Property(e => e.PlayerOneId).IsRequired(false);
            entity.Property(e => e.PlayerTwoId).IsRequired(false);
            entity.Property(e => e.AmountBet);
            entity.Property(e => e.Status);
            entity.Property(e => e.PlayerOneMove);
            entity.Property(e => e.PlayerTwoMove);
            entity.Property(e => e.CreatedAt);
            
            entity.HasIndex(e => e.PlayerOneId);
            entity.HasIndex(e => e.PlayerTwoId);
            
            entity.HasOne(m => m.PlayerOne)
                .WithMany()
                .HasForeignKey(m => m.PlayerOneId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(m => m.PlayerTwo)
                .WithMany()
                .HasForeignKey(m => m.PlayerTwoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            
            entity.HasMany(m => m.Transactions)
                .WithOne(t => t.Match)
                .HasForeignKey(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });

        modelBuilder.Entity<GameTransaction>(entity =>
        {
            entity.ToTable("GameTransactions");
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.GameId);
            entity.Property(e => e.FromUserId);
            entity.Property(e => e.ToUserId);
            entity.Property(e => e.Amount);
            entity.Property(e => e.TransactionType);
            
            entity.HasIndex(e => new {e.FromUserId, e.ToUserId});
            
            entity.HasOne(e => e.Sender)
                .WithMany(e => e.GameTransactionsSent)
                .HasForeignKey(e => e.FromUserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            entity.HasOne(e => e.Receiver)
                .WithMany(e => e.GameTransactionsReceived)
                .HasForeignKey(e => e.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            entity.HasOne(e => e.Match)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}