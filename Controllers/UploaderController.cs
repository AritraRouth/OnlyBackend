using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.Model;
using net.Services;

namespace net.Controllers
{
    [ApiController]
    [Route("upload")]
    [AllowAnonymous]
    public class UploaderController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;

        public UploaderController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        public async Task<Response> UploadFile([FromForm] FileModel filemodel)
        {
            var response = new Response();

            try
            {
                string imageUrl = await _cloudinaryService.UploadImageAsync(filemodel.file);
                response.Statuscode = 200;
                response.ErrorMessage = $"Image uploaded successfully. URL: {imageUrl}";
            }
            catch (Exception ex)
            {
                response.Statuscode = 500;
                response.ErrorMessage = "Upload failed: " + ex.Message;
            }

            return response;
        }
    }
}
