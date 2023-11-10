using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MsftFabricManagementFunctionApp
{
    public class DailyTriggeredStatusChangeFunction
    {
		private HttpClient _client = new HttpClient();

		private string _bearerToken;

		private enum StatusChange { Suspend = 0, Resume = 1, };

		// "0 1 18 * * *" = every day at 18:01 UTC
		// "0 */5 * * * *" = every 5 minutes

		[FunctionName("DailyTriggeredStatusChangeFunction")]
        public void Run([TimerTrigger("0 1 18 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Daily triggered suspend function executed at: {DateTime.Now}");

			var statusChange = StatusChange.Suspend;

			string tennant = Environment.GetEnvironmentVariable("tennant");


			string appId = Environment.GetEnvironmentVariable("appId");
			string password = Environment.GetEnvironmentVariable("password");

			_bearerToken = GetBearer(tennant, appId, password);

			var subsciptionId = Environment.GetEnvironmentVariable("subsciptionId");
			var resourceGroupName = Environment.GetEnvironmentVariable("resourceGroupName");
			var dedicatedCapacityName = Environment.GetEnvironmentVariable("dedicatedCapacityName");
			var apiVersion = Environment.GetEnvironmentVariable("apiVersion");

			try
			{
				ChangeMsftFabricState(statusChange, subsciptionId, resourceGroupName, dedicatedCapacityName, apiVersion, log).Wait(); // resume or pause / suspend
			}
			catch (Exception ex)
			{
				log.LogError($"Exception: {ex.Message}");

				throw;
			}
		}

		// Resume or Pause / Suspend
		private async Task ChangeMsftFabricState(StatusChange statusChange, string subsciptionId, string resourceGroupName, string dedicatedCapacityName, string apiVersion, ILogger log)
		{
			if (await TestPausedFabricState(subsciptionId, resourceGroupName, dedicatedCapacityName, apiVersion, log))
			{
				log.LogInformation($"Status change skipped");
				return;
			}

			var url = $"https://management.azure.com/subscriptions/{subsciptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Fabric/capacities/{dedicatedCapacityName}/{statusChange.ToString().ToLower()}?api-version={apiVersion}";

			try
			{
				using var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Add("Authorization", $"Bearer {_bearerToken}");

				using var response = await _client.SendAsync(request);
				response.EnsureSuccessStatusCode();

				log.LogInformation($"Status change: {statusChange}");
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<bool> TestPausedFabricState(string subsciptionId, string resourceGroupName, string dedicatedCapacityName, string apiVersion, ILogger log)
		{
			var url = $"https://management.azure.com/subscriptions/{subsciptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Fabric/capacities/{dedicatedCapacityName}?api-version={apiVersion}";

			try
			{
				using var request = new HttpRequestMessage(HttpMethod.Get, url);
				request.Headers.Add("Authorization", $"Bearer {_bearerToken}");

				using var response = await _client.SendAsync(request);
				response.EnsureSuccessStatusCode();

				var jsonString = await response.Content.ReadAsStringAsync();
				var json = JsonConvert.DeserializeObject<dynamic>(jsonString);

				var state = (string) json.properties.state;

				log.LogInformation($"Status found: {state}");

				return state == "Paused";
			}
			catch (Exception)
			{
				throw;
			}
		}

		private string GetBearer(string tennant, string appId, string password)
		{
			var nvc = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("client_id", appId),
				new KeyValuePair<string, string>("client_secret", password),
				new KeyValuePair<string, string>("resource", "https://management.azure.com/")
			};

			var url = $"https://login.microsoftonline.com/{tennant}/oauth2/token";

			using var client = new HttpClient();
			using var request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new FormUrlEncodedContent(nvc)
			};

			using var response = client.SendAsync(request).Result;
			var jsonString = response.Content.ReadAsStringAsync().Result;
			var json = JsonConvert.DeserializeObject<dynamic>(jsonString);
			return json.access_token;
		}
	}
}
