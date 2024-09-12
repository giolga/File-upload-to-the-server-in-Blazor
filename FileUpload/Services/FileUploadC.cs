using Microsoft.AspNetCore.Components.Forms;

namespace FileUpload.Services
{
    public interface IFileUpload
    {
        public Task UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUrl(IBrowserFile file);
    }

    public class FileUploadC : IFileUpload
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileUploadC> _logger;

        public FileUploadC(IWebHostEnvironment webHostEnvironment, ILogger<FileUploadC> logger)
        {
            this._webHostEnvironment = webHostEnvironment;
            this._logger = logger;
        }

        public async Task UploadFile(IBrowserFile file)
        {
            if (file is not null)
            {
                try
                {
                    var upload = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.Name);

                    using (var stream = file.OpenReadStream())
                    {
                        var fileStream = File.Create(upload);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        public async Task<string> GeneratePreviewUrl(IBrowserFile file)
        {
            if(!file.ContentType.Contains("image"))
            {
                if(file.ContentType.Contains("pdf"))
                {
                    return "images/pdf_logo.png";
                }
            }

            var resizedImg = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizedImg.Size];
            await resizedImg.OpenReadStream().ReadAsync(buffer);

            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        }
    }
}
