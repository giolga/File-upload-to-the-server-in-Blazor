using Microsoft.AspNetCore.Components.Forms;

namespace FileUpload.Services
{
    public interface IFileUpload
    {
        public Task UploadFile(IBrowserFile file);
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
    }
}
