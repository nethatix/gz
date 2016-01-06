﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace gzWeb.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        // Mario added extra profile
        [Required, StringLength(30)]
        public string FirstName { get; set; }

        [Required, StringLength(30)]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [NotMapped] /* Must get from Casino API */
        public decimal GamBalance { get; set; }

        public virtual InvBalance InvBalance { get; set; }

        public virtual ICollection<CustPortfolio> PortfWeights { get; set; }
        public virtual ICollection<Portfolio> Portfolio { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    /// <summary>
    /// Introduced them to change Id to Int identity
    /// </summary>
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole> {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim> {
        public CustomUserStore(ApplicationDbContext context)
            : base(context) {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole> {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context) {
        }
    }

    // End of custom classes for int Id

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<CustPortfolio> CustPortfolios { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<InvBalance> InvBalances { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortFund> PortFunds { get; set; }
        public DbSet<Transx> Transxes { get; set; }
        public DbSet<TransxType> TransxTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // do fluent API stuff below
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}