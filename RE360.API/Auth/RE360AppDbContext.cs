using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RE360.API.DBModels;

namespace RE360.API.Auth
{
	public class RE360AppDbContext : IdentityDbContext<ApplicationUser>
	{
        public RE360AppDbContext()
        {
        }

        public RE360AppDbContext(DbContextOptions<RE360AppDbContext> options) : base(options)
		{
		}

        public DbSet<ListingAddress> ListingAddress { get; set; }
        public DbSet<ClientDetail> ClientDetail { get; set; }
        public DbSet<SolicitorDetail> SolicitorDetail { get; set; }
        public DbSet<ParticularDetail> ParticularDetail { get; set; }
        public DbSet<ContractDetail> ContractDetail { get; set; }
        public DbSet<ContractRate> ContractRate { get; set; }
        public DbSet<LegalDetail> LegalDetail { get; set; }
        public DbSet<MethodOfSale> MethodOfSale { get; set; }
        public DbSet<TenancyDetail> TenancyDetail { get; set; }
        public DbSet<PriorAgencyMarketing> PriorAgencyMarketing { get; set; }
        public DbSet<Estimates> Estimates { get; set; }
        public DbSet<Execution> Execution { get; set; }
        public DbSet<PropertyInformation> PropertyInformation { get; set; }
        public DbSet<PropertyInformationDetail> PropertyInformationDetail { get; set; }
        public DbSet<PropertyAttributeType> PropertyAttributeType { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
            /* Comment For Live Start  */

            //builder.Entity<ApplicationUser>(entity =>
            //{
            //    entity.ToTable(name: "dbo.aspNetUsers");
            //    entity.Property(e => e.Id).HasColumnName("UserId");
            //});

            /* Comment For Live END  */
        }
    }
}
