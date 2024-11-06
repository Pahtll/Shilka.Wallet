using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shilka.Wallet.Domain.Models;
using Shilka.Wallet.Persistence.Interfaces;

namespace Shilka.Wallet.Persistence.Repositories;

public class FreakWalletRepository(
	ILogger<FreakWalletRepository> logger,
	ShilkaWalletDbContext context
	) : IFreakWalletRepository
{
	private const string LogLvl = "Shilka.Wallet.Persistence.Repositories.FreakWalletRepository";
	
	public async Task<IEnumerable<FreakWallet>> GetAll()
	{
		logger.LogDebug($"{LogLvl}.{nameof(GetAll)}");
		return await context.Wallets
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<FreakWallet> GetByUserId(long userId)
	{
		logger.LogDebug($"{LogLvl}.{nameof(GetByUserId)}");
		return await context.Wallets
			       .AsNoTracking()
			       .FirstOrDefaultAsync(w => w.UserId == userId)
		       ?? throw new ArgumentException("User with this id does not exists");
	}

	public async Task<long> CreateFreakWallet(long userId)
	{
		logger.LogDebug($"{LogLvl}.{nameof(CreateFreakWallet)}");
		
		var wallet = new FreakWallet
		{
			UserId = userId,
			Amount = 0
		};

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			await context.Wallets.AddAsync(wallet);

			await context.SaveChangesAsync();
			await transaction.CommitAsync();

			return userId;
		}
		catch (Exception e)
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task SendCoins(long fromId, long toId, decimal amount)
	{
		if (amount < 0)
			throw new ArgumentException("Can't send negative number of coins");

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var fromWallet = await context.Wallets
				                 .FirstOrDefaultAsync(w => w.UserId == fromId)
			                 ?? throw new ArgumentException("Can't send coins from non-binary person");
			var toWallet = await context.Wallets
				               .FirstOrDefaultAsync(w => w.UserId == toId)
			               ?? throw new ArgumentException("Can't send coins to a non-binary person");

			if (amount > fromWallet.Amount)
				throw new ArgumentException("Can't send coins cuz user \"from\" don't have enough coins");

			fromWallet.Amount -= amount;
			toWallet.Amount += amount;

			await context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task GiveCoins(long userId, decimal amount)
	{
		if (amount < 0)
			throw new ArgumentException("Can't give user negative number of coins");

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var wallet = await context.Wallets
				             .FirstOrDefaultAsync(w => w.UserId == userId)
			             ?? throw new ArgumentException("User with this id does not exists");

			wallet.Amount += amount;

			await context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task TakeCoins(long userId, decimal amount)
	{
		if (amount < 0)
			throw new ArgumentException("Can't take negative number of coins");

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var wallet = await context.Wallets
				             .FirstOrDefaultAsync(w => w.UserId == userId)
			             ?? throw new ArgumentException("User with this Id does not exists");

			wallet.Amount -= amount;

			await context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			await transaction.RollbackAsync();
			throw;
		}
	}
}