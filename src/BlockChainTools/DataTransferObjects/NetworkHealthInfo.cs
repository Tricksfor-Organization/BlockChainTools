using System.Numerics;

namespace BlockChainTools.DataTransferObjects;

/// <summary>
/// Contains the results of a blockchain network health check.
/// </summary>
public class NetworkHealthInfo
{
    /// <summary>
    /// Whether the network is considered healthy overall.
    /// True when blocks are advancing, timestamps are fresh, gas estimates succeed, and chain ID matches.
    /// </summary>
    public required bool IsHealthy { get; init; }

    /// <summary>
    /// The latest block number reported by the RPC endpoint.
    /// </summary>
    public BigInteger? LatestBlockNumber { get; init; }

    /// <summary>
    /// The timestamp of the latest block as a UTC DateTime, if available.
    /// </summary>
    public DateTime? LatestBlockTimestamp { get; init; }

    /// <summary>
    /// Whether the latest block timestamp is within the acceptable freshness threshold.
    /// </summary>
    public bool IsBlockTimestampFresh { get; init; }

    /// <summary>
    /// The current gas price returned by the node, if available.
    /// </summary>
    public BigInteger? GasPrice { get; init; }

    /// <summary>
    /// Whether a gas price was successfully retrieved and is non-zero.
    /// </summary>
    public bool IsGasPriceAvailable { get; init; }

    /// <summary>
    /// The chain ID returned by the RPC endpoint, if available.
    /// </summary>
    public BigInteger? ChainId { get; init; }

    /// <summary>
    /// Whether the returned chain ID matches the expected chain ID (when an expected value was provided).
    /// </summary>
    public bool ChainIdMatches { get; init; }

    /// <summary>
    /// Human-readable summary of any issues detected during the health check.
    /// Empty when fully healthy.
    /// </summary>
    public required IReadOnlyList<string> Issues { get; init; }
}
