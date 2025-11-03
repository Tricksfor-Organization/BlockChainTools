using BlockChainTools.Interfaces;
using BlockChainTools.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockChainTools;

public static class ConfigureServices
{
    public static void AddBlockChainTools(this IServiceCollection services)
    {
        services.AddScoped<IWeb3ProviderService, Web3ProviderService>();
        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<IGasService, GasService>();
        services.AddScoped<IHdWalletService, HdWalletService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IBlockService, BlockService>();
        services.AddScoped<ITransferService, TransferService>();
    }
}