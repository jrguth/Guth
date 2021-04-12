using Newtonsoft.Json;
namespace Guth.OpenTrivia.Abstractions.Models
{
    public class GetSessionTokenResponse
    {
        [JsonProperty("response_code")]
        public int ResponseCode { get; set; }
        [JsonProperty("response_message")] 
        public string ResponseMessage { get; set; }
        [JsonProperty("token")] 
        public string Token { get; set; }
    }
}
