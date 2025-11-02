using BlockChainTools.Helpers;
using System.Text.Json.Serialization;

namespace BlockChainTools.DataTransferObjects.Alchemy;

internal class BaseRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = AlchemyParameters.DefaultId;

    [JsonPropertyName("jsonrpc")]
    public readonly string JsonRpc = AlchemyParameters.DefaultJsonRpc;
}