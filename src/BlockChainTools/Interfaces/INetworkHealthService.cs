using BlockChainTools.DataTransferObjects;
using Nethereum.Signer;
using Nethereum.Web3;

namespace BlockChainTools.Interfaces;

public interface INetworkHealthService
{
    /// <summary>
    /// Performs a health check on the blockchain network via the given Web3 endpoint.
    /// Verifies that blocks are being produced, timestamps are fresh, gas price is available,
    /// and the chain ID matches expectations.
    /// </summary>
    /// <param name="web3">The Web3 instance to query against.</param>
    /// <param name="expectedChain">
    /// Optional expected chain. When provided, the health check verifies the endpoint returns the correct chain ID.
    /// </param>
    /// <param name="maxBlockAgeSeconds">
    /// Maximum acceptable age (in seconds) of the latest block timestamp.
    /// Defaults to 120 seconds. If the latest block is older than this, the network is considered unhealthy.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="NetworkHealthInfo"/> describing the health status of the network.</returns>
    Task<NetworkHealthInfo> CheckNetworkHealthAsync(Web3 web3, Chain? expectedChain = null, int maxBlockAgeSeconds = 120, CancellationToken cancellationToken = default);
}
