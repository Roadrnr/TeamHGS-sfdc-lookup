using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TeamHGS_Functions
{
    public static class Function3
    {
        [FunctionName("Function3")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}