# BlockChainTools

A .NET 9 library that provides comprehensive utilities and services for working with blockchain technology, built on top of Nethereum. This library simplifies common blockchain operations including wallet management, balance queries, transactions, gas estimation, and token transfers.

## Features

- **HD Wallet Management**: Generate and restore hierarchical deterministic wallets
- **Balance Services**: Query native token and ERC20/ERC721 token balances
- **Transaction Services**: Send and track blockchain transactions
- **Gas Estimation**: Estimate gas costs for various transaction types
- **Transfer Services**: Simplified token transfer operations (native, ERC20, ERC721)
- **Block Services**: Query blockchain block information
- **Web3 Provider**: Easy Web3 instance creation with various configurations

## Prerequisites

- .NET SDK 9.0 or later
- Redis (if using distributed nonce features via Tricksfor.DistributedNonce)

## Installation

Install the package via NuGet (when published):
```bash
dotnet add package Tricksfor.BlockChainTools
```

Or reference it directly in your project:
```xml
<PackageReference Include="Tricksfor.BlockChainTools" Version="9.0.0" />
```

## Getting Started

### 1. Register Services in Dependency Injection

In your `Program.cs` or startup configuration:

```csharp
using BlockChainTools;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register all BlockChainTools services
services.AddBlockChainTools();

var serviceProvider = services.BuildServiceProvider();
```

**Important**: You **must** register `HttpClient` in your DI container for `GasService` to work properly. The `GasService` requires an `HttpClient` instance to communicate with RPC endpoints for gas price queries.

### 2. Basic Usage Examples

#### Web3 Provider Service

Create Web3 instances for querying or transactions:

```csharp
using BlockChainTools.Interfaces;

var web3Provider = serviceProvider.GetRequiredService<IWeb3ProviderService>();

// For read-only operations (queries, balance checks)
var web3ReadOnly = web3Provider.CreateWeb3("https://polygon-rpc.com");

// For transactions (requires private key)
var web3WithAccount = web3Provider.CreateWeb3(
    privateKey: "0x...", 
    chain: Nethereum.Signer.Chain.Polygon, 
    rpcUrl: "https://polygon-rpc.com"
);
```

#### Balance Service

Query native and token balances:

```csharp
using BlockChainTools.Interfaces;

var balanceService = serviceProvider.GetRequiredService<IBalanceService>();

// Get native token balance (e.g., MATIC on Polygon)
var nativeBalance = await balanceService.GetBalanceOfAsync(
    web3, 
    "0xYourAddress...", 
    cancellationToken
);

// Get ERC20 token balance (e.g., USDC)
var usdcBalance = await balanceService.GetErc20BalanceAsync(
    web3, 
    "0xUSDCContractAddress...", 
    "0xYourAddress...", 
    cancellationToken
);

// Get ERC721 token balance (NFTs)
var nftBalance = await balanceService.GetErc721BalanceAsync(
    web3, 
    "0xNFTContractAddress...", 
    "0xYourAddress...", 
    cancellationToken
);
```

#### HD Wallet Service

Generate and manage hierarchical deterministic wallets:

```csharp
using BlockChainTools.Interfaces;
using NBitcoin;

var hdWalletService = serviceProvider.GetRequiredService<IHdWalletService>();

// Generate a new wallet
var wallet = hdWalletService.GenerateWallet(
    seedPassword: "your-secure-password", 
    wordCount: WordCount.Twelve
);

// Restore wallet from mnemonic
var restoredWallet = hdWalletService.RestoreWallet(
    words: new List<string> { "word1", "word2", /* ... */ }, 
    seedPassword: "your-secure-password"
);

// Get account by index
var account = hdWalletService.GetAccount(wallet, index: 0);

// Get account for specific chain
var polygonAccount = hdWalletService.GetAccount(
    wallet, 
    Nethereum.Signer.Chain.Polygon, 
    index: 0
);

// Get Web3 instance with account
var web3 = hdWalletService.GetWeb3(
    wallet, 
    Nethereum.Signer.Chain.Polygon, 
    index: 0, 
    rpcUrl: "https://polygon-rpc.com"
);
```

#### Gas Service

Estimate gas and query gas prices:

```csharp
using BlockChainTools.Interfaces;

var gasService = serviceProvider.GetRequiredService<IGasService>();

// Get current gas price from RPC
var gasPrice = await gasService.GetGasPriceAsync(
    rpcUrl: "https://polygon-rpc.com", 
    cancellationToken
);

// Estimate gas for native token transfer
var transferGas = await gasService.EstimateTransferGasAsync(
    web3, 
    toAddress: "0xRecipient...", 
    value: 1.5m, 
    cancellationToken
);

// Estimate gas for ERC20 transfer
var erc20Gas = await gasService.EstimateErc20TransferGasAsync(
    web3, 
    erc20ContractAddress: "0xUSDC...", 
    toAddress: "0xRecipient...", 
    value: BigInteger.Parse("1000000"), // 1 USDC (6 decimals)
    cancellationToken
);

// Estimate gas for ERC721 (NFT) transfer
var nftGas = await gasService.EstimateErc721TransferGasAsync(
    web3, 
    erc721ContractAddress: "0xNFT...", 
    toAddress: "0xRecipient...", 
    tokenId: 42, 
    cancellationToken
);

// Calculate total amount to transfer whole balance
var totalAmount = await gasService.CalculateTotalAmountToTransferWholeBalanceAsync(
    web3, 
    fromAddress: "0xYourAddress...", 
    cancellationToken
);
```

#### Transfer Service

Send tokens and NFTs:

```csharp
using BlockChainTools.Interfaces;
using Nethereum.RPC.Eth.DTOs;

var transferService = serviceProvider.GetRequiredService<ITransferService>();

// Transfer native token (e.g., MATIC)
var txInput = new TransactionInput
{
    From = "0xYourAddress...",
    To = "0xRecipient...",
    Value = new Nethereum.Hex.HexTypes.HexBigInteger(
        Nethereum.Web3.Web3.Convert.ToWei(1.5m)
    )
};
var txHash = await transferService.TransferAsync(web3, txInput);

// Transfer ERC20 token
var erc20Transfer = new Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction
{
    To = "0xRecipient...",
    AmountToSend = BigInteger.Parse("1000000") // 1 USDC (6 decimals)
};
var erc20TxHash = await transferService.TransferAsync(
    web3, 
    "0xUSDCContract...", 
    erc20Transfer
);

// Transfer ERC721 token (NFT)
var nftTransfer = new Nethereum.Contracts.Standards.ERC721.ContractDefinition.TransferFromFunction
{
    From = "0xYourAddress...",
    To = "0xRecipient...",
    TokenId = 42
};
var nftTxHash = await transferService.TransferAsync(
    web3, 
    "0xNFTContract...", 
    nftTransfer
);

// Transfer and wait for receipt
var receipt = await transferService.TransferAndWaitForReceiptAsync(web3, txInput);
```

#### Transaction Service

Query transaction details:

```csharp
using BlockChainTools.Interfaces;

var transactionService = serviceProvider.GetRequiredService<ITransactionService>();

var transaction = await transactionService.GetTransactionByHashAsync(
    web3, 
    hash: "0xTransactionHash...", 
    cancellationToken
);
```

#### Block Service

Query blockchain information:

```csharp
using BlockChainTools.Interfaces;

var blockService = serviceProvider.GetRequiredService<IBlockService>();

var latestBlock = await blockService.GetLastBlockAsync(web3, cancellationToken);
```

## Available Services

### IWeb3ProviderService
- `CreateWeb3(string rpcUrl)` - Create Web3 for read-only operations
- `CreateWeb3(string privateKey, Chain chain, string rpcUrl)` - Create Web3 for transactions

### IBalanceService
- `GetBalanceOfAsync(Web3, string address)` - Get native token balance
- `GetErc20BalanceAsync(Web3, string contractAddress, string address)` - Get ERC20 balance
- `GetErc721BalanceAsync(Web3, string contractAddress, string address)` - Get ERC721 balance

### IHdWalletService
- `GenerateWallet(string seedPassword, WordCount)` - Generate new HD wallet
- `RestoreWallet(List<string> words, string seedPassword)` - Restore from mnemonic
- `GetAccount(Wallet, int index)` - Get account by index
- `GetAccount(Wallet, Chain, int index)` - Get account for specific chain
- `GetWeb3(Wallet, Chain, int index, string rpcUrl)` - Get Web3 with account

### IGasService
**Note**: Requires `HttpClient` to be registered in DI container
- `GetGasPriceAsync(string rpcUrl)` - Get current gas price
- `EstimateTransferGasAsync(Web3, string to, decimal value)` - Estimate native transfer gas
- `EstimateErc20TransferGasAsync(Web3, string contract, string to, BigInteger value)` - Estimate ERC20 gas
- `EstimateErc721TransferGasAsync(Web3, string contract, string to, int tokenId)` - Estimate NFT gas
- `SuggestFeeToTransferWholeBalanceAsync(Web3, string from)` - Suggest fee for full balance transfer
- `CalculateTotalAmountToTransferWholeBalanceAsync(Web3, string from)` - Calculate transferable amount

### ITransferService
- `TransferAsync(Web3, TransactionInput)` - Transfer native tokens
- `TransferAsync(Web3, string contract, TransferFunction)` - Transfer ERC20
- `TransferAsync(Web3, string contract, TransferFromFunction)` - Transfer ERC721
- `TransferAndWaitForReceiptAsync(...)` - Transfer and wait for confirmation

### ITransactionService
- `GetTransactionByHashAsync(Web3, string hash)` - Get transaction details

### IBlockService
- `GetLastBlockAsync(Web3)` - Get latest block number

## Project Structure

```
BlockChainTools/
├── src/
│   └── BlockChainTools/
│       ├── Configuration.cs              # DI registration
│       ├── Services/                     # Service implementations
│       │   ├── BalanceService.cs
│       │   ├── BlockService.cs
│       │   ├── GasService.cs
│       │   ├── HdWalletService.cs
│       │   ├── TransactionService.cs
│       │   ├── TransferService.cs
│       │   └── Web3ProviderService.cs
│       ├── Interfaces/                   # Service contracts
│       └── DataTransferObjects/          # DTOs for external APIs
└── tests/
    └── BlockChainTools.Tests/            # Unit tests
```

## Dependencies

- **Nethereum.Web3** (5.0.0) - Ethereum Web3 implementation
- **Nethereum.Accounts** (5.0.0) - Account management
- **Nethereum.Signer** (5.0.0) - Transaction signing
- **Nethereum.HdWallet** (5.0.0) - HD wallet support
- **Microsoft.Extensions.DependencyInjection** (9.0.10) - DI support
- **Tricksfor.DistributedNonce** (9.0.0) - Distributed nonce management

## Testing

Run the unit tests:

```bash
dotnet test tests/BlockChainTools.Tests/BlockChainTools.Tests.csproj
```

The test suite uses:
- **NUnit** for test framework
- **NSubstitute** for mocking
- **DotNet.Testcontainers** for integration tests

## Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or contributions, please visit the GitHub repository.