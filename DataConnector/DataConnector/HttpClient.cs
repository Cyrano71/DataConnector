using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DataConnector
{
    public class HttpInseeConnector
    {
        private HttpClient _client = new HttpClient();
        public HttpInseeConnector(String token)
        {
            _client.DefaultRequestHeaders.Authorization =
                          new AuthenticationHeaderValue("Bearer", token);
        }

        public HttpResponseMessage Get(String baseUrl, String parametersUrlFormat)
        {
            var path = baseUrl + parametersUrlFormat;

            //https://api.insee.fr/donnees-locales/V0.1/donnees/geo-NA5_B-ENTR_INDIVIDUELLE@GEO2017REE2017/COM-51108.all.all
            var response = _client.GetAsync(path).GetAwaiter().GetResult();
            return response;        
        }
    }
}
