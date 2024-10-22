using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace CredentialTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController:ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;

        public BucketsController(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAsync()
        {
            var data = await _amazonS3.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }
    }
}
