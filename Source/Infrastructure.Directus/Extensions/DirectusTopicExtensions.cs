using Infrastructure.Directus.Models;

namespace Infrastructure.Directus.Extensions;

public static class DirectusTopicExtensions
{
  public static DateTime GetLastModifiedUtc(
    this DirectusTopic topic)
  {
    return topic.DateUpdated ?? topic.DateCreated;
  }
}