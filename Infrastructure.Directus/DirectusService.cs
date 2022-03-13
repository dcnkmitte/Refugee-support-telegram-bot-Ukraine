using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dawn;
using Infrastructure.Directus.Configuration;
using Infrastructure.Directus.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Directus;

public class DirectusService : IDirectusService
{
  private readonly ILogger<DirectusService> log;
  private readonly HttpClient restClient;
  private readonly string loginUrl;
  private readonly LoginPayload loginPayload;
  private readonly string getTopicNamesUrl;
  private readonly string getTopicBodiesUrl;
  private readonly int targetCityId;

  public DirectusService(IOptions<DirectusConfig> config, ILogger<DirectusService> log)
  {
    Guard.Argument(config.Value.Email, "Directus:Email").NotEmpty();
    Guard.Argument(config.Value.Password, "Directus:Password").NotEmpty();
    Guard.Argument(config.Value.CityId, "Directus:CityId").NotDefault();

    this.log = log;
    this.restClient = new HttpClient();
    this.loginPayload = new LoginPayload { Email = config.Value.Email, Password = config.Value.Password };
    this.loginUrl = "https://02h9qgyp.directus.app/auth/login";
    this.getTopicNamesUrl = "https://02h9qgyp.directus.app/items/BotArea";
    this.getTopicBodiesUrl = "https://02h9qgyp.directus.app/items/BotContent";
    this.targetCityId = config.Value.CityId;
  }

  public async Task<(DirectusTopicName names, DirectusTopicBody bodies)> GetTopicsAsync()
  {
    var accessToken = await this.GetAccessTokenAsync();
    var topicNames = await this.GetAsync<DirectusTopicName>(accessToken, this.getTopicNamesUrl);
    topicNames.Data = topicNames.Data.Where(x => x.CityId == this.targetCityId).ToArray();
    var topicBodies = await this.GetAsync<DirectusTopicBody>(accessToken, this.getTopicBodiesUrl);

    return (names: topicNames, bodies: topicBodies);
  }

  private async Task<T> GetAsync<T>(string accessToken, string url)
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await this.restClient.SendAsync(request);

    if (!response.IsSuccessStatusCode)
    {
      this.log.LogError($"Failed to get values from {url}. Status code: {response.StatusCode}");
      response.EnsureSuccessStatusCode();
    }

    var resultJson = await response.Content.ReadAsStringAsync();
    var result = JsonConvert.DeserializeObject<T>(resultJson);

    return result;
  }

  private async Task<string> GetAccessTokenAsync()
  {
    var loginResponse = this.restClient.PostAsync(loginUrl, JsonContent.Create(this.loginPayload));
    if (!loginResponse.Result.IsSuccessStatusCode)
    {
      this.log.LogError($"Failed to login. Status code: {loginResponse.Result.StatusCode}");
      loginResponse.Result.EnsureSuccessStatusCode();
    }

    var loginInfoJson = await loginResponse.Result.Content.ReadAsStringAsync();
    var loginInfo = JsonConvert.DeserializeObject<LoginResponse>(loginInfoJson);
    var accessToken = loginInfo?.Data?.AccessToken;
    Guard.Argument(accessToken, "AccessToken").NotEmpty();

    return accessToken;
  }
}