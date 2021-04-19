using RestSharp;
using Newtonsoft.Json;

namespace Guth.OpenTrivia.Abstractions
{
    public class OpenTriviaRequest<TRequest>
    {
        public IRestRequest Request { get; private set; }
        public OpenTriviaRequest(string resource, string sessionToken = null)
        {
            Request = new RestRequest(resource, Method.GET, DataFormat.Json);
            AddParameter("token", sessionToken);
        }

        public OpenTriviaRequest<TRequest> AddParameter(string name, object value)
        {
            Request.AddParameter(name, value);
            return this;
        }
        public OpenTriviaRequest<TRequest> AddSerializedParameter(string name, object value)
        {
            object val = value is null
                ? ""
                : JsonConvert.SerializeObject(value);
            Request.AddParameter(name, val);
            return this;
        }
    }
}
