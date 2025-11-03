# GitHub Copilot Instructions for BlockChainTools

These instructions help Copilot generate high‑quality, idiomatic code and tests for this repository, aligned with how the library is actually used and wired through DI.

## Project overview

BlockChainTools is a .NET 9 library built on Nethereum that provides ready‑to‑use services for common blockchain operations:
- HD wallet generation and restoration
- Web3 provider creation (read‑only and with an account)
- Balance queries (native, ERC20, ERC721)
- Gas price and gas estimation helpers
- Token and NFT transfers (native, ERC20, ERC721)
- Block and transaction queries

The library integrates with Tricksfor.DistributedNonce to ensure safe, non‑duplicated nonces when sending transactions concurrently from multiple processes/instances.

Key points
- Runtime: .NET 9 with nullable enabled and implicit usings
- Ethereum stack: Nethereum (Web3, Accounts, HdWallet, Signer)
- Nonce management: Tricksfor.DistributedNonce (Redis‑backed)
- DI: Microsoft.Extensions.DependencyInjection via AddBlockChainTools()
- Tests: NUnit + NSubstitute (no Moq)

## Code organization

```
src/BlockChainTools/
├── Configuration.cs                   # DI extension AddBlockChainTools()
├── Interfaces/                        # Service contracts
│   ├── IBalanceService.cs
│   ├── IBlockService.cs
│   ├── IGasService.cs
│   ├── IHdWalletService.cs
│   ├── ITransactionService.cs
│   ├── ITransferService.cs
│   └── IWeb3ProviderService.cs
├── Services/                          # Implementations
│   ├── BalanceService.cs
│   ├── BlockService.cs
│   ├── GasService.cs                  # Requires HttpClient from DI
│   ├── HdWalletService.cs
│   ├── TransactionService.cs
│   ├── TransferService.cs
│   └── Web3ProviderService.cs         # Wires DistributedNonce into Account.NonceService
└── DataTransferObjects/
		└── Alchemy/                       # DTOs used for gas price RPC

tests/BlockChainTools.Tests/
├── *.cs                               # NUnit + NSubstitute unit tests
└── BlockChainTools.Tests.csproj
```

## How to register and use the library (DI)

Add the library services to your container. Some dependencies must be registered explicitly:
- HttpClient: required for GasService
- DistributedNonce: required for Web3ProviderService when creating Web3 with a private key

```csharp
using BlockChainTools;                      // AddBlockChainTools()
using DistributedLockManager;               // AddDistributedLockManager()
using DistributedNonce;                     // AddDistributedNonce()
using DistributedNonce.Services;            // DistributedNonceService
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var services = new ServiceCollection();

// 1) HttpClient for GasService
services.AddHttpClient();

// 2) Redis‑backed distributed lock + nonce services (required for account Web3)
services.AddDistributedLockManager();
var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379,abortConnect=false");
services.AddSingleton<IConnectionMultiplexer>(redis);
services.AddDistributedNonce();

// 3) BlockChainTools services
services.AddBlockChainTools();

var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

// Resolve services
var web3Provider = scope.ServiceProvider.GetRequiredService<BlockChainTools.Interfaces.IWeb3ProviderService>();
var gas = scope.ServiceProvider.GetRequiredService<BlockChainTools.Interfaces.IGasService>();
```

### Web3 examples

```csharp
// Read‑only Web3
var web3Read = web3Provider.CreateWeb3("https://polygon-rpc.com");

// Web3 with account (DistributedNonce is automatically attached to Account.NonceService)
var web3Tx = web3Provider.CreateWeb3(
		privateKey: "0x...",
		chain: Nethereum.Signer.Chain.Polygon,
		rpcUrl: "https://polygon-rpc.com");
```

### Gas price and estimation

```csharp
var gasPrice = await gas.GetGasPriceAsync("https://polygon-rpc.com");
var estimateNative = await gas.EstimateTransferGasAsync(web3Read, "0xRecipient...", 1.5m);
```

For more usage examples (balances, transfers, wallets, blocks, transactions), see README.md in the repo root.

## Using DistributedNonce correctly (no duplicate nonces)

Web3ProviderService sets Account.NonceService using DistributedNonceService. To ensure correctness across multiple instances/pods:
- Use a shared Redis instance/cluster reachable by all app instances
- Register IConnectionMultiplexer as a singleton for the app lifetime (do not dispose per request)
- Keep the lock key stable by using the exact same address formatting for the account
- Prefer resilient Redis options (examples): AbortOnConnectFail=false, ConnectRetry=5, ExponentialReconnect, SyncTimeout=10000

Example configuration snippet
```csharp
var options = ConfigurationOptions.Parse("redis:6379");
options.AbortOnConnectFail = false;
options.ConnectRetry = 5;
options.ReconnectRetryPolicy = new ExponentialRetry(5000);
var mux = await ConnectionMultiplexer.ConnectAsync(options);
services.AddSingleton<IConnectionMultiplexer>(mux);
```

## Behavioral contracts (quick reference)

- IWeb3ProviderService
	- Input: rpcUrl or (privateKey, Chain, rpcUrl)
	- Output: Nethereum.Web3.Web3 instance (with Account.NonceService wired when using private key)
- IGasService
	- Requires HttpClient from DI
	- GetGasPriceAsync(rpcUrl) -> BigInteger gas price
	- EstimateTransferGasAsync / EstimateErc20TransferGasAsync / EstimateErc721TransferGasAsync -> BigInteger gas limits
- IBalanceService, ITransferService, ITransactionService, IBlockService, IHdWalletService
	- Mirror the signatures described in README.md

## Testing patterns and expectations

Preferred stack
- NUnit for tests, NSubstitute for mocks

Unit test tips
- Mock interfaces (e.g., IGasService, IWeb3ProviderService) directly with NSubstitute for fast unit tests
- When testing GasService behavior, provide an HttpClient using a fake HttpMessageHandler to stub JSON‑RPC responses
- When testing Web3 integration, mock Nethereum.JsonRpc.Client.IClient where feasible; avoid real network calls in unit tests

Examples
```csharp
// Mocking an interface
var gas = Substitute.For<BlockChainTools.Interfaces.IGasService>();
gas.GetGasPriceAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
	 .Returns(new System.Numerics.BigInteger(100));

// Mocking IClient (Nethereum) for Web3 construction
var client = Substitute.For<Nethereum.JsonRpc.Client.IClient>();
var web3 = new Nethereum.Web3.Web3(client);
```

## Copilot guidance: do / don’t in this repo

Do
- Use AddBlockChainTools() for DI wiring; also register AddHttpClient()
- Register AddDistributedLockManager(), a singleton IConnectionMultiplexer, and AddDistributedNonce() when creating Web3 with a private key
- Prefer NUnit + NSubstitute for tests (do not introduce Moq)
- Keep assertions minimal and clear; follow Arrange/Act/Assert
- Reuse the service interfaces from BlockChainTools.Interfaces

Don’t
- Don’t create HttpClient with new HttpClient() inside services or examples; rely on DI
- Don’t bypass DistributedNonce for transaction nonces
- Don’t hardcode shell commands into production code
- Don’t introduce alternate test frameworks

## Common helpers Copilot may generate

- Resolve services via a small helper
```csharp
public static class ServiceProviderExtensions
{
		public static T Get<T>(this IServiceProvider sp) where T : notnull
				=> sp.GetRequiredService<T>();
}
```

- Create Web3 from mnemonic (HdWalletService + Web3ProviderService)
```csharp
public static Nethereum.Web3.Web3 CreateWeb3FromMnemonic(
		this BlockChainTools.Interfaces.IHdWalletService hd,
		BlockChainTools.Interfaces.IWeb3ProviderService web3Provider,
		IEnumerable<string> words,
		string seedPassword,
		Nethereum.Signer.Chain chain,
		string rpcUrl,
		int index = 0)
{
		var wallet = hd.RestoreWallet(words.ToList(), seedPassword);
		var account = hd.GetAccount(wallet, chain, index);
		return web3Provider.CreateWeb3(account.PrivateKey, chain, rpcUrl);
}
```

## Acceptance checklist Copilot should satisfy
- Build compiles on .NET 9 with nullable enabled and implicit usings
- New tests use NUnit + NSubstitute and pass locally
- DI examples include AddHttpClient(), AddBlockChainTools(), and (when sending transactions) AddDistributedLockManager() + singleton IConnectionMultiplexer + AddDistributedNonce()
- Examples avoid duplicate nonces when creating Web3 with a private key
