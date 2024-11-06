using System.Globalization;
using Grpc.Core;
using Shilka.Wallet.Persistence.Interfaces;

namespace Shilka.Wallet.GRPC.Services;

public class FreakWalletService(
	ILogger<FreakWalletService> logger,
	IFreakWalletRepository walletRepository
	) : Wallet.WalletBase
{
	public override async Task<GetAllWalletsReply> GetAllWallets(GetAllWalletsRequest request, ServerCallContext context)
	{
		try
		{
			var wallets = await walletRepository.GetAll();
			
			var reply = new GetAllWalletsReply();
			reply.Wallets.AddRange(wallets.Select(wallet => new DefaultWallet
			{
				UserId = wallet.UserId,
				Amount = wallet.Amount.ToString(CultureInfo.InvariantCulture)
			}));

			return reply;
		}
		catch (Exception e)
		{
			logger.LogError("{e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.Internal, e.Message));
		}
	}

	public override async Task<GetWalletByUserIdReply> GetWalletByUserId(GetWalletByUserIdRequest request, ServerCallContext context)
	{
		try
		{
			var wallet = await walletRepository.GetByUserId(request.UserId);

			var reply = new GetWalletByUserIdReply
			{
				Wallet = new DefaultWallet
				{
					UserId = wallet.UserId,
					Amount = wallet.Amount.ToString(CultureInfo.InvariantCulture)
				}
			};

			return reply;
		}
		catch (ArgumentException e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
		}
		catch (Exception e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.Internal, e.Message));
		}
	}

	public override async Task<DefaultResult> SendCoins(SendCoinsRequest request, ServerCallContext context)
	{
		try
		{
			var isParsed = decimal.TryParse(request.Amount, out var amount);
			if (!isParsed)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Amount can't be parsed"));

			logger.LogDebug("Waiting for repository response");

			await walletRepository.SendCoins(request.FromId, request.ToId, amount);

			return new DefaultResult { IsOk = true };
		}
		catch (ArgumentException e)
		{
			logger.LogError("error, during SendCoins: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
		}
		catch (Exception e)
		{
			logger.LogError("error, during SendCoins: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.Internal, e.Message));
		}
	}

	public override async Task<DefaultResult> GiveCoins(DefaultCoinOperations request, ServerCallContext context)
	{
		try
		{
			var isParsed = decimal.TryParse(request.Amount, out var amount);
			if (!isParsed)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Error, can't parse sent amount"));

			await walletRepository.GiveCoins(request.UserId, amount);

			return new DefaultResult { IsOk = true };
		}
		catch (ArgumentException e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
		}
		catch (Exception e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.Internal, e.Message));
		}
	}

	public override async Task<DefaultResult> TakeCoins(DefaultCoinOperations request, ServerCallContext context)
	{
		try
		{

			var isParsed = decimal.TryParse(request.Amount, out var amount);
			if (!isParsed)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Can't parse this decimal"));

			await walletRepository.TakeCoins(request.UserId, amount);

			return new DefaultResult { IsOk = true };
		}
		catch (ArgumentException e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
		}
		catch (Exception e)
		{
			logger.LogError("Error: {e.Message}", e.Message);
			throw new RpcException(new Status(StatusCode.Internal, e.Message));
		}
	}
}