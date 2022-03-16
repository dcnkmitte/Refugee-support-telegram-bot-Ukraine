using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class LoginResponseData
{
  [JsonProperty("access_token")]
  public string AccessToken { get; set; }
}