using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BookingFlightClient.Services.IServices;

namespace BookingFlightClient.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;

        public S3Service(IConfiguration configuration)
        {
            var awsConfig = configuration.GetSection("AWS");
            _bucketName = awsConfig["BucketName"];
            _region = awsConfig["Region"];
            var credentials = new Amazon.Runtime.BasicAWSCredentials(awsConfig["AccessKey"], awsConfig["SecretKey"]);

            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region)
            };

            // Initialize the S3 client with the credentials and configuration
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                // Create a request to delete the object from S3
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                // Delete the object from S3
                await _s3Client.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Error deleting file from S3: {ex.Message}", ex);
            }
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            try
            {
                // Create a request to get the object from S3
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };
                // Get the object from S3
                var response = await _s3Client.GetObjectAsync(request);
                // Return the response stream
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Error getting file from S3: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            try
            {
                // Create a request to upload the file to S3
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fileStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    //CannedACL = S3CannedACL.PublicRead
                };

                // Use TransferUtility to upload the file
                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);


                // Return the public URL of the uploaded file
                return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{fileName}";
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Error uploading file to S3: {ex.Message}", ex);
            }
        }
    }
}
