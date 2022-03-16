using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Extensions;

public static class HashExtensions
{
  public static string GetMd5Hash(this string source)
  {
    using var md5Hash = MD5.Create();
    var sourceBytes = Encoding.UTF8.GetBytes(source);

    var hashBytes = md5Hash.ComputeHash(sourceBytes);

    var hash = BitConverter.ToString(hashBytes);

    return hash;
  }
}