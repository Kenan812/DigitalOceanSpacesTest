using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalOceanSpaces1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private const int TIMEOUT = 2500;

        // Digital Ocean settings
        private static readonly string S3LoginRoot = "https://testgpt.fra1.digitaloceanspaces.com";
        private static readonly string S3BucketName = "";
        private static readonly string AccessKey = "DO00CDJHPMKWLJFRF4WX";
        private static readonly string AccessKeySecret = "yHNEhAcAPDin8H5u4T+OaJj8lFmMAGADhU9xvnTSbf8";
        private static readonly string S3FolderName = "";


        private IWebHostEnvironment _webHostEnvironment;

        public ImageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            bool Sucesso = false;

            try
            {
                AmazonS3Client? s3Client = new(new BasicAWSCredentials(AccessKey, AccessKeySecret), new AmazonS3Config
                {
                    ServiceURL = S3LoginRoot,
                    Timeout = TimeSpan.FromSeconds(TIMEOUT),
                    MaxErrorRetry = 8,
                });

                TransferUtility fileTransferUtility = new(s3Client);

                TransferUtilityUploadRequest? fileTransferUtilityRequest = new()
                {
                    BucketName = S3BucketName + @"/" + S3FolderName,
                    Key = file.FileName,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456,
                    CannedACL = S3CannedACL.PublicRead,
                };

                fileTransferUtility.Upload(fileTransferUtilityRequest);

                Sucesso = true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

            return Ok(new { Sucesso });
        }
    }
}
