using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Integration.Tracker.Exceptions;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;

namespace TransnationalLanka.ThreePL.Integration.Tracker
{
    public class TrackerApiService
    {
        private const string DevHost = "http://192.168.52.101:1257/";
        private const string LiveHost = "http://192.168.52.101:1257/";

        private readonly string _host;

        public TrackerApiService(bool isDevEnv)
        {
            _host = isDevEnv ? DevHost : LiveHost;
        }

        public async Task<CreateCustomerResponse> CreateCustomer(CreateCustomerRequest request)
        {
            return await PostRequest<CreateCustomerRequest,CreateCustomerResponse>("SetCustomer", request);
        }

        #region Private Methods

        private async Task<TResponse> PostRequest<TRequest, TResponse>(string url, TRequest request)
            where TRequest : class where TResponse : class
        {
            using var httpClinet = GetHttpClient();
            var data = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await httpClinet.PostAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                throw new TrackingApiException("Unable to process tracking api request");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent);
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_host) };
            return httpClient;
        } 

        #endregion
    }
}
