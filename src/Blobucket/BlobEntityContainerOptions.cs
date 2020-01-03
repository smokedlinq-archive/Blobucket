using System;
using Blobucket.Formatters;

namespace Blobucket
{
    public sealed class BlobEntityContainerOptions
    {
        public string ContainerName { get; set; } = string.Empty;
        public BlobEntityFormatter Formatter { get; set; } = BlobEntityFormatter.Null;
    }
}
