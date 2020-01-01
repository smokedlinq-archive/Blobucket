using System.Text.RegularExpressions;
using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainerOptions<T>
    {
        public string ContainerName { get; set; } = Regex.Replace(typeof(T).Name, "(?<!^)([A-Z])", "-$1", RegexOptions.Compiled).TrimStart('-').ToLower();
        public BlobEntityFormatter Formatter { get; set; } = new JsonBlobEntityFormatter();
    }
}
