using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Integration.Tracker.Exceptions;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Integration.Tracker.Util;

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
            return await PostRequest<CreateCustomerRequest, CreateCustomerResponse>("SetCustomer", request);
        }

        public async Task<UpdateCustomerResponse> UpdateCustomerStatus(UpdateCustomerRequest request)
        {
            return await PostRequest<UpdateCustomerRequest, UpdateCustomerResponse>("SetCustomerStatus", request);
        }

        public async Task<UpdateDeliveryStatusResponse> UpdateDeliveryStatus(UpdateDeliveryStatusRequest request)
        {
            return await PostRequest<UpdateDeliveryStatusRequest, UpdateDeliveryStatusResponse>("SetStatus", request);
        }

        public async Task<UpdateTrackingNoDetailResponse> UpdateTrackingNoDetails(UpdateTrackingNoDetailRequest request)
        {
            return await PostRequest<UpdateTrackingNoDetailRequest, UpdateTrackingNoDetailResponse>("SetCustomerTrackingNoDetails", request);
        }

        public async Task<GetCustomerStatusResponse> GetCustomerStatus(GetCustomerStatusRequest request)
        {
            return await PostRequest<GetCustomerStatusRequest, GetCustomerStatusResponse>("GetCustomerStatus", request);
        }

        public async Task<GetDeliveryStatusResponse> GetDeliveryStatus(GetDeliveryStatusRequest request)
        {
            return await PostRequest<GetDeliveryStatusRequest, GetDeliveryStatusResponse>("GetStatus", request);
        }

        public async Task<GetTrackingNoRangeResponse> GetTrackingNoRange(GetTrackingNoRangeRequest request)
        {
            return await PostRequest<GetTrackingNoRangeRequest, GetTrackingNoRangeResponse>("GetfromCustomerTrackingNoRange", request);
        }

        public async Task<GetInwardResponse> GetInward(GetInwardRequest request)
        {
            return await PostRequest<GetInwardRequest, GetInwardResponse>("GetInward", request);
        }

        #region Private Methods

        private async Task<TResponse> PostRequest<TRequest, TResponse>(string url, TRequest request)
            where TRequest : class where TResponse : class
        {
            using var httpClinet = GetHttpClient();

            var jsonSerializeOption = new JsonSerializerOptions();
            jsonSerializeOption.Converters.Add(new DateTimeConverter());

            var data = new StringContent(JsonSerializer.Serialize(request, options: jsonSerializeOption), 
                Encoding.UTF8, "application/json");

            var response = await httpClinet.PostAsync(url, data);

            var contentBody = JsonSerializer.Serialize(request, options: jsonSerializeOption);

            if (!response.IsSuccessStatusCode)
            {
                throw new TrackingApiException("Unable to process tracking api request");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, options: jsonSerializeOption);
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_host) };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic UHJvbnRvM1BMOlBMM1BMMjAyMTA2");
            return httpClient;
        }

        #endregion
    }
}
