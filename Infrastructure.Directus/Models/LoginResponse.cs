using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class LoginResponse
{
  [JsonProperty("data")]
  public LoginResponseData Data { get; set; }
}