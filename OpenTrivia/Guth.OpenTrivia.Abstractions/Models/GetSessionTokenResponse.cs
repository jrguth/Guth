using Newtonsoft.Json;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public record GetSessionTokenResponse(
        [JsonProperty("response_code")] int ResponseCode, 
        [JsonProperty("response_message")] string ResponseMessage,
        [JsonProperty("token")] string Token);
}
