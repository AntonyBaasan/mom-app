using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestConsumer
{
    public class RequestArgs
    {
        public string Uri;
        public string AuthToken;
        public string XsrfToken;
    }

    public class RestClient : IRestClient
    {
        private readonly string _baseUri;

        public RestClient(string baseUri)
        {
            _baseUri = baseUri;
        }

        public virtual async Task<HttpResponseMessage> GetAsync(RequestArgs args, Dictionary<string, object> queryStringParams = null, string overrideUserLang = "")
        {
            var handler = new HttpClientHandler { UseCookies = false };
            var client = new HttpClient(handler) { BaseAddress = new Uri(_baseUri) };
            var uri = AttachParamsToUri(_baseUri + args.Uri.Substring(1), queryStringParams);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Cookie", "authToken=" + args.AuthToken);
            request.Headers.Add("X-XSRF-TOKEN", args.XsrfToken);

            return await client.SendAsync(request);
        }

        private string AttachParamsToUri(string uri, Dictionary<string, object> queryStringParams)
        {
            if (queryStringParams == null)
            {
                return uri;
            }

            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query = ConvertQueryParams(queryStringParams, query);
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        private NameValueCollection ConvertQueryParams(Dictionary<string, object> queryStringParams, NameValueCollection query)
        {
            queryStringParams.ToList().ForEach(kv =>
            {
                var valType = kv.Value.GetType();

                if (valType.IsPrimitive || valType == typeof(string) || valType == typeof(decimal) || valType == typeof(decimal?))
                    query[kv.Key] = kv.Value.ToString();
                else if (valType == typeof(DateTime) || valType == typeof(DateTime?))
                    query[kv.Key] = Convert.ToDateTime(kv.Value).ToString("o");
                else if (valType == typeof(DateTime?))
                {
                    var date = (DateTime?)kv.Value;

                    query[kv.Key] = date.Value.ToString("o");
                }
                else
                    query[kv.Key] = JsonConvert.SerializeObject(kv.Value, GetCamelCaseSerializer());
            });

            return query;
        }

        private JsonSerializerSettings GetCamelCaseSerializer()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new PxCamelCasePropertyNamesContractResolver();
            serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            return serializerSettings;
        }

    }

    public class PxCamelCasePropertyNamesContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization);
            foreach (var p in result)
            {
                p.PropertyName = Regex.Replace(p.PropertyName, "\b[A-Z]", new MatchEvaluator(match =>
                {
                    var v = match.ToString();
                    return Char.ToLower(v[0]) + v.Substring(1);
                }));
            }
            return result;
        }

    }

}
