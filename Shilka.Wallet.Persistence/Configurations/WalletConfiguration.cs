using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shilka.Wallet.Domain.Models;

namespace Shilka.Wallet.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<FreakWallet>
{
	public void Configure(EntityTypeBuilder<FreakWallet> builder)
	{
		builder.HasKey(wallet => wallet.UserId);

		builder.Property(wallet => wallet.Amount)
			.IsRequired();
	}
}