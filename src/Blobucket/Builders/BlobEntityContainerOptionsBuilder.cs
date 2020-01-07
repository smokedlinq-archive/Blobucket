using System;
using System.Text.RegularExpressions;
using Blobucket.Formatters;

namespace Blobucket.Builders
{
    internal class BlobEntityContainerOptionsBuilder : OptionsBuilder<BlobEntityContainerOptions>, IBlobEntityContainerOptionsBuilder
    {
        public IBlobEntityContainerOptionsBuilder UseContainerName<T>()
        {
            Add(x => x.ContainerName = Regex.Replace(typeof(T).Name, "(?<!^)([A-Z])", "-$1", RegexOptions.Compiled).TrimStart('-').ToLower(System.Globalization.CultureInfo.CurrentCulture));
            return this;
        }

        public IBlobEntityContainerOptionsBuilder UseContainerName(string containerName)
        {
            Add(x => x.ContainerName = containerName);
            return this;
        }

        public IBlobEntityContainerOptionsBuilder UseFormatter<T>(T formatter) where T : BlobEntityFormatter
        {
            Add(x => x.Formatter = formatter);
            return this;
        }

        public BlobEntityContainerOptionsBuilder Append(Action<IBlobEntityContainerOptionsBuilder>? action)
        {
            action?.Invoke(this);
            return this;
        }
    }
}
