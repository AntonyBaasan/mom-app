using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestConsumer
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> GetAsync(RequestArgs args, Dictionary<string, object> queryStringParams = null, string overrideUserLang = "");
    }
}
