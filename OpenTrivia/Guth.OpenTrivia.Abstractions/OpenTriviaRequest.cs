using RestSharp;

namespace Guth.OpenTrivia.Abstractions
{
    public class OpenTriviaRequest<T>
    {
        public IRestRequest Request { get; private set; }
        public OpenTriviaRequest(string resource, string sessionToken = null)
        {
            Request = new RestRequest(resource, Method.GET, DataFormat.Json);
            AddParameterIfNotNull("token", sessionToken);
        }

        public OpenTriviaRequest<T> AddParameter(string name, object value)
        {
            Request.AddParameter(name, value);
            return this;
        }

        public OpenTriviaRequest<T> AddParameterIfNotNull(string name, object value)
        {
            if (value != null)
            {
                return AddParameter(name, value);
            }
            return this;
        }
    }
}
