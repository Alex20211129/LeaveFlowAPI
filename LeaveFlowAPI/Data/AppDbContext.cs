using LeaveFlowAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 自我關聯（員工 → 主管）
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.Subordinates)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Form → User
            modelBuilder.Entity<Form>()
                .HasOne(f => f.User)
                .WithMany(u => u.Forms)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApprovalRecord → Form
            modelBuilder.Entity<ApprovalRecord>()
                .HasOne(a => a.Form)
                .WithMany(f => f.ApprovalRecords)
                .HasForeignKey(a => a.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApprovalRecord → Manager（User）
            modelBuilder.Entity<ApprovalRecord>()
                .HasOne(a => a.Manager)
                .WithMany(u => u.ApprovalRecords)
                .HasForeignKey(a => a.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}