# BlockChainTools

A .NET 9 library built on Nethereum that provides ready-to-use services for common blockchain operations including HD wallet management, balance queries, gas estimation, and token transfers.

## Features

- **HD Wallet Management** - Generate and restore hierarchical deterministic wallets
- **Balance Services** - Query native, ERC20, and ERC721 token balances
- **Gas Estimation** - Estimate gas costs for various transaction types
- **Transfer Services** - Simplified transfers for native tokens, ERC20, and ERC721 (NFTs)
- **Transaction & Block Services** - Query blockchain data
- **Web3 Provider** - Easy Web3 instance creation with distributed nonce support

## Installation

```bash
dotnet add package Tricksfor.BlockChainTools
```

## Quick Start

### 1. Register Services

```csharp
using BlockChainTools;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpClient();              // Required for GasService
services.AddBlockChainTools();

var provider = services.BuildServiceProvider();
```

### 2. Create Web3 Instance

```csharp
using BlockChainTools.Interfaces;

var web3Provider = provider.GetRequiredService<IWeb3ProviderService>();

// Read-only Web3
var web3 = web3Provider.CreateWeb3("https://polygon-rpc.com");

// Web3 with account (for transactions)
var web3Tx = web3Provider.CreateWeb3(
    privateKey: "0x...",
    chain: Nethereum.Signer.Chain.Polygon,
    rpcUrl: "https://polygon-rpc.com"
);
```

### 3. Common Operations

```csharp
var balanceService = provider.GetRequiredService<IBalanceService>();
var gasService = provider.GetRequiredService<IGasService>();
var transferService = provider.GetRequiredService<ITransferService>();

// Get native balance
var balance = await balanceService.GetBalanceOfAsync(web3, "0xAddress...");

// Get ERC20 balance
var usdcBalance = await balanceService.GetErc20BalanceAsync(
    web3, "0xUSDCContract...", "0xAddress...");

// Estimate gas
var gasPrice = await gasService.GetGasPriceAsync("https://polygon-rpc.com");
var gasLimit = await gasService.EstimateTransferGasAsync(web3, "0xTo...", 1.5m);

// Transfer native tokens
var txInput = new Nethereum.RPC.Eth.DTOs.TransactionInput
{
    From = "0xFrom...",
    To = "0xTo...",
    Value = new Nethereum.Hex.HexTypes.HexBigInteger(
        Nethereum.Web3.Web3.Convert.ToWei(1.5m))
};
var txHash = await transferService.TransferAsync(web3Tx, txInput);
```

## Available Services

- **IWeb3ProviderService** - Create Web3 instances (read-only or with account)
- **IBalanceService** - Query native, ERC20, and ERC721 balances
- **IGasService** - Gas price queries and estimation (requires HttpClient)
- **ITransferService** - Transfer native tokens, ERC20, and ERC721 (NFTs)
- **IHdWalletService** - Generate/restore HD wallets and derive accounts
- **ITransactionService** - Query transaction details
- **IBlockService** - Query blockchain block information

## Distributed Nonce Management

For transaction-enabled Web3 instances, the library integrates with `Tricksfor.DistributedNonce` to prevent nonce conflicts across multiple instances. To use this feature:

```csharp
using DistributedLockManager;
using DistributedNonce;

services.AddHttpClient();
services.AddDistributedLockManager();
services.AddSingleton<IConnectionMultiplexer>(
    await ConnectionMultiplexer.ConnectAsync("localhost:6379"));
services.AddDistributedNonce();
services.AddBlockChainTools();
```

## Documentation

For detailed documentation, examples, and API reference, visit the [GitHub repository](https://github.com/Tricksfor-Organization/BlockChainTools).

## License

MIT License - see [LICENSE](https://github.com/Tricksfor-Organization/BlockChainTools/blob/main/LICENSE) for details.
