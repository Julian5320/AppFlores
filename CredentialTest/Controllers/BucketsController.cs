using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace CredentialTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController:ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BucketsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAsync()
        {
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var s3Client = new AmazonS3Client(accessKey, secretKey);
            var data = await s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }
    }
}
