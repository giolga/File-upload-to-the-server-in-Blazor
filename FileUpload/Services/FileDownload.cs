using Microsoft.AspNetCore.StaticFiles;

namespace FileUpload.Services
{
    public interface IFileDownload
    {
        Task<List<string>> getUploadedFiles();
        Task DownloadFile(string url);
    }

    public class FileDownload : IFileDownload
    {
        private IWebHostEnvironment _webHostEnvironment;

        public FileDownload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Task DownloadFile(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> getUploadedFiles()
        {
            var base64urls = new List<string>();
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var files = Directory.GetFiles(uploadPath);

            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    using (var fileInput = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var memoryStream = new MemoryStream();
                        await fileInput.CopyToAsync(memoryStream);

                        var buffer = memoryStream.ToArray();
                        var fileType = GetMimeTypeFileExtension(file);
                        base64urls.Add($"data:{fileType};base64,{Convert.ToBase64String(buffer)}");
                    }
                }
            }

            return base64urls;
        }

        private string GetMimeTypeFileExtension(string filePath)
        {
            const string defaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = defaultContentType;
            }

            return contentType;
        }
    }
}
