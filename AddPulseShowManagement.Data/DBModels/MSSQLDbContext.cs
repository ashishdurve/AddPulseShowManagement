using AddPulseShowManagement.Data.DataTableModels;
using Microsoft.EntityFrameworkCore;


namespace AddPulseShowManagement.Data.DBModels
{
    public partial class MSSQLDbContext : DbContext
    {
        public MSSQLDbContext()
        {
        }

        public MSSQLDbContext(DbContextOptions<MSSQLDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<Teams> Teams { get; set; } = null!;
        public virtual DbSet<Shows> Shows{ get; set; } = null!;
        public virtual DbSet<Contestants> Contestants { get; set; } = null!;
        public virtual DbSet<TeamContestants> TeamContestants { get; set; } = null!;
        public virtual DbSet<DashboardConfigurations> DashboardConfigurations { get; set; } = null!;
        public virtual DbSet<CardDescription> CardDescription { get; set; } = null!;
        public virtual DbSet<PlayersSequence> PlayersSequence { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.UserID, "PK_Users").IsUnique();
                entity.Property(e => e.Email).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Teams>(entity =>
            {
                entity.HasIndex(e => e.TeamID, "PK_Teams").IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Shows>(entity =>
            {
                entity.HasIndex(e => e.ShowID, "PK_Shows").IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            //modelBuilder.Entity<UserTypes>(entity =>
            //{                
            //    entity.HasIndex(e => e.UserTypeID, "PK_UserTypes").IsUnique();
            //    entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            //});

            
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
