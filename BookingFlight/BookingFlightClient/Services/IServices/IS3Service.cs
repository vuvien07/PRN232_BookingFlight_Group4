namespace BookingFlightClient.Services.IServices
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream);
        Task<Stream> GetFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}
