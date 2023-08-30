using System;
using Masterdev.Update.RestAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace Masterdev.Update.RestAPI.Properties
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
		{

		}

		public DbSet<Files> files { get; set; }
		public DbSet<Data> data { get; set; }
		public DbSet<Users> users { get; set; }
		public DbSet<Apps> apps { get; set; }
	}
}

