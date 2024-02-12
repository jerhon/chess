using Microsoft.EntityFrameworkCore;

namespace Honlsoft.Chess.Database;

public class ChessContext : DbContext
{
    /// <summary>
    /// The games in the database.
    /// </summary>
    public DbSet<Game> Games { get; set; }
    
    /// <summary>
    /// The game positions.
    /// </summary>
    public DbSet<GamePosition> GamePositions { get; set; }
   
    /// <summary>
    /// The chess positions in the database.
    /// </summary>
    public DbSet<ChessPosition> ChessPositions { get; set; }
    
    public ChessContext(DbContextOptions<ChessContext> options) : base(options) {  }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId);
            entity.Property(e => e.GameId).ValueGeneratedOnAdd();
            entity.HasMany(e => e.Positions).WithOne().HasForeignKey(e => e.GameId);
        });
        
        modelBuilder.Entity<GamePosition>(entity =>
        {
            entity.HasKey(e => e.GamePositionId);
            entity.Property(e => e.GamePositionId).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Game).WithMany(e => e.Positions).HasForeignKey(e => e.GameId);
        });

        modelBuilder.Entity<ChessPosition>(entity => {
            entity.HasKey(e => e.ChessPositionId);
            entity.Property(e => e.ChessPositionId).ValueGeneratedOnAdd();
        });
    }
}