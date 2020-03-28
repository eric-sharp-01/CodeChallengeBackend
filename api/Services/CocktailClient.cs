using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Services
{
    public class CocktailClient : ICocktailClient
    {
        public HttpClient _client;
        public CocktailClient(HttpClient client)
        {
            this._client = client;
        }

        public async Task<string> Get(string url)
        {
            var res = await this._client.GetAsync(url);
            string body = await res.Content.ReadAsStringAsync();
            return body;
        }
    }
}
