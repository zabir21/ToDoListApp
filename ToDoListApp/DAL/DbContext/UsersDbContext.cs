using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.DAL.Entity;
using ToDoListApp.DAL.Entity.Identity;

namespace ToDoList.DAL.DbContext
{
    public class UsersDbContext : IdentityDbContext<User, UserRoles, Guid>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Users)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<IdentityUserLogin<Guid>>()
                .HasNoKey();

            modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
                 b.HasKey(i => new { i.UserId, i.RoleId }));

            modelBuilder.Entity<IdentityUserToken<Guid>>()
                .HasNoKey();

        }
    }
}