using Microsoft.EntityFrameworkCore;
using Shilka.Wallet.Domain.Models;
using Shilka.Wallet.Persistence.Configurations;

namespace Shilka.Wallet.Persistence;

public class ShilkaWalletDbContext(DbContextOptions<ShilkaWalletDbContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new WalletConfiguration());
		
		base.OnModelCreating(modelBuilder);
	}
	
	public DbSet<FreakWallet> Wallets { get; set; }
}