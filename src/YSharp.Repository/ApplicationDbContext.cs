using YSharp.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<AccessLog> AccessLogs { get; set; }

        public DbSet<Image> Images { get; set; }
        public DbSet<IPAddress> IPAddress { get; set; }

        public DbSet<LoginLog> LoginLogs { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccessLog>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<AccessLog>().Property(x => x.Url).HasMaxLength(100);
            modelBuilder.Entity<AccessLog>().Property(x => x.AbsolutelyUrl).HasMaxLength(1000);
            modelBuilder.Entity<AccessLog>().Property(x => x.RequestIP).HasMaxLength(128);
            modelBuilder.Entity<AccessLog>().Property(x => x.RequestMethod).HasMaxLength(100);
            modelBuilder.Entity<AccessLog>().Property(x => x.RequestContentType).HasMaxLength(100);
            modelBuilder.Entity<AccessLog>().Property(x => x.ResponseStatusCode).HasMaxLength(100);
            modelBuilder.Entity<AccessLog>().Property(x => x.ResponseContentType).HasMaxLength(100);

            modelBuilder.Entity<ApplicationRole>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<ApplicationRole>().Property(x => x.Name).HasMaxLength(50);
            modelBuilder.Entity<ApplicationRole>().Property(x => x.NormalizedName).HasMaxLength(50);


            modelBuilder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.NormalizedEmail).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.NormalizedUserName).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Name).HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PasswordHash).HasMaxLength(256);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.SecurityStamp).HasMaxLength(256);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.ConcurrencyStamp).HasMaxLength(256);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(20);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(x => x.IsDeleted == false);


            modelBuilder.Entity<Image>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<Image>().Property(x => x.FileName).HasMaxLength(100);
            modelBuilder.Entity<Image>().Property(x => x.VirtualPath).HasMaxLength(1000);
            modelBuilder.Entity<Image>().Property(x => x.AbsolutePath).HasMaxLength(1000);
            modelBuilder.Entity<Image>().HasQueryFilter(x => x.IsDeleted == false);

            modelBuilder.Entity<IPAddress>().Property(x => x.IP).HasMaxLength(128);
            modelBuilder.Entity<IPAddress>().Property(x => x.Country).HasMaxLength(100);
            modelBuilder.Entity<IPAddress>().Property(x => x.Province).HasMaxLength(100);
            modelBuilder.Entity<IPAddress>().Property(x => x.City).HasMaxLength(100);
            modelBuilder.Entity<IPAddress>().Property(x => x.Detail).HasMaxLength(300);
            modelBuilder.Entity<IPAddress>().Property(x => x.Source).HasMaxLength(100);


            modelBuilder.Entity<LoginLog>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<LoginLog>().Property(x => x.UserName).HasMaxLength(100);
            modelBuilder.Entity<LoginLog>().Property(x => x.LoginUrl).HasMaxLength(1000);
            modelBuilder.Entity<LoginLog>().Property(x => x.IP).HasMaxLength(128);
            modelBuilder.Entity<LoginLog>().Property(x => x.Location).HasMaxLength(200);

            modelBuilder.Entity<Message>().Property(x => x.Id).HasMaxLength(32);

            modelBuilder.Entity<RolePermission>().Property(x => x.Id).HasMaxLength(32);
            modelBuilder.Entity<RolePermission>().Property(x => x.Url).HasMaxLength(100);
        }
    }
}
