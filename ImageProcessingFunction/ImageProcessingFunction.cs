using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace ImageProcessingFunction
{
    public static class ImageProcessingFunction
    {
        [FunctionName("ImageProcessingFunctionpostimage")]
        public static async Task Run([BlobTrigger("postimages/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            string name,
                    [Blob("postimages/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream outputBlob,
            ILogger log)
        {
            log.LogInformation($"Processing blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(myBlob))
            {
                image.Mutate(x => x.Resize(600, 600));
                await image.SaveAsJpegAsync(outputBlob);
            }
        }
    }
}
