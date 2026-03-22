using System.Numerics;
using BlockChainTools.DataTransferObjects;
using BlockChainTools.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.Web3;

namespace BlockChainTools.Services;

public class NetworkHealthService : INetworkHealthService
{
    public async Task<NetworkHealthInfo> CheckNetworkHealthAsync(Web3 web3, Chain? expectedChain = null, int maxBlockAgeSeconds = 120, CancellationToken cancellationToken = default)
    {
        var issues = new List<string>();
        BigInteger? latestBlockNumber = null;
        DateTime? latestBlockTimestamp = null;
        bool isBlockTimestampFresh = false;
        BigInteger? gasPrice = null;
        bool isGasPriceAvailable = false;
        BigInteger? chainId = null;
        bool chainIdMatches = true;

        // 1. Check latest block number and timestamp
        try
        {
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(
                Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

            if (block is not null)
            {
                latestBlockNumber = block.Number?.Value;
                (latestBlockTimestamp, isBlockTimestampFresh) = EvaluateBlockFreshness(block.Timestamp, maxBlockAgeSeconds);

                if (latestBlockTimestamp is null)
                    issues.Add("Latest block has no timestamp.");
                else if (!isBlockTimestampFresh)
                    issues.Add($"Latest block timestamp is {(DateTime.UtcNow - latestBlockTimestamp.Value).TotalSeconds:F0}s old, exceeding the {maxBlockAgeSeconds}s threshold.");
            }
            else
            {
                issues.Add("Unable to retrieve the latest block.");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"Failed to retrieve latest block: {ex.Message}");
        }

        // 2. Check gas price
        try
        {
            var gasPriceResult = await web3.Eth.GasPrice.SendRequestAsync();
            gasPrice = gasPriceResult?.Value;
            isGasPriceAvailable = gasPrice is not null && gasPrice > 0;

            if (!isGasPriceAvailable)
            {
                issues.Add("Gas price is unavailable or zero.");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"Failed to retrieve gas price: {ex.Message}");
        }

        // 3. Check chain ID
        try
        {
            var chainIdResult = await web3.Eth.ChainId.SendRequestAsync();
            chainId = chainIdResult?.Value;

            if (expectedChain is not null)
            {
                chainIdMatches = chainId == (int)expectedChain;
                if (!chainIdMatches)
                {
                    issues.Add($"Chain ID mismatch: expected {(int)expectedChain}, got {chainId}.");
                }
            }
        }
        catch (Exception ex)
        {
            chainIdMatches = false;
            issues.Add($"Failed to retrieve chain ID: {ex.Message}");
        }

        var isHealthy = issues.Count == 0;

        return new NetworkHealthInfo
        {
            IsHealthy = isHealthy,
            LatestBlockNumber = latestBlockNumber,
            LatestBlockTimestamp = latestBlockTimestamp,
            IsBlockTimestampFresh = isBlockTimestampFresh,
            GasPrice = gasPrice,
            IsGasPriceAvailable = isGasPriceAvailable,
            ChainId = chainId,
            ChainIdMatches = chainIdMatches,
            Issues = issues.AsReadOnly()
        };
    }

    private static (DateTime? Timestamp, bool IsFresh) EvaluateBlockFreshness(HexBigInteger? blockTimestamp, int maxBlockAgeSeconds)
    {
        if (blockTimestamp?.Value is null)
            return (null, false);

        var timestamp = DateTimeOffset.FromUnixTimeSeconds((long)blockTimestamp.Value).UtcDateTime;
        var isFresh = (DateTime.UtcNow - timestamp).TotalSeconds <= maxBlockAgeSeconds;
        return (timestamp, isFresh);
    }
}
