using System.Text.Json.Serialization;

namespace BlockChainTools.DataTransferObjects.Alchemy;

internal class GasPriceRequest : BaseRequest
{
    [JsonPropertyName("method")]
    internal readonly string Method = "eth_gasPrice";
}