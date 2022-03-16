using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class LoginPayload
{
  [JsonProperty("email")]
  public string Email { get; set; }

  [JsonProperty("password")]
  public string Password { get; set; }
}