using RestSharp;
using Newtonsoft.Json;

namespace Guth.OpenTrivia.Abstractions
{
    public class OpenTriviaRequest<TRequest>
    {
        public RestRequest Request { get; private set; }
        public OpenTriviaRequest(string resource, string sessionToken = null)
        {
            RestRequest request = new RestRequest(resource, Method.Get);
            AddParameter("token", sessionToken);
        }

        public OpenTriviaRequest<TRequest> AddParameter(string name, object value)
        {
            Request.AddParameter(name, value, ParameterType.QueryString);
            return this;
        }
        public OpenTriviaRequest<TRequest> AddSerializedParameter(string name, object value)
        {
            object val = value is null
                ? ""
                : JsonConvert.SerializeObject(value);
            Request.AddParameter(name, val, ParameterType.QueryString);
            return this;
        }
    }
}
