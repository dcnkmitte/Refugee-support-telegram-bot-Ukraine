﻿using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusLanguage
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Code")]
    public string Code { get; set; }
}