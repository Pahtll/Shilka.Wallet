using Shilka.Wallet.Domain.Models;

namespace Shilka.Wallet.Persistence.Interfaces;

public interface IFreakWalletRepository
{
	Task<IEnumerable<FreakWallet>> GetAll();
	Task<FreakWallet> GetByUserId(long userId);
	Task<long> CreateFreakWallet(long userId);
	Task SendCoins(long fromId, long toId, decimal amount);
	Task GiveCoins(long userId, decimal amount);
	Task TakeCoins(long userId, decimal amount);
}