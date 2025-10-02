// Контекст базы данных

using GameShopModel;
using Microsoft.EntityFrameworkCore;
public class ApplicationContext : DbContext
{
    // Создание таблиц
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<DLC> DLCs { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<TransactionInfo> TransactionInfos { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<GameToDLC> GamesToDLCs { get; set; } = null!;
    public DbSet<GameToGenre> GamesToGenres { get; set; } = null!;
    public DbSet<GameToImage> GamesToImages { get; set; } = null!;
    public DbSet<DLCToImage> DLCsToImages { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ShopDB;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}