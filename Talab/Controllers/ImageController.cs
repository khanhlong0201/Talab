using CORE_TALAB.Data;
using CORE_TALAB.Enum;
using CORE_TALAB.Enum.Customers;
using CORE_TALAB.Models.Table;
using CORE_TALAB.Services;
using CORE_TALAB.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using CORE_TALAB.Model.Reponse;
using Talab.Model.Image;
using Microsoft.AspNetCore.Hosting;
namespace Talab.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImageController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(ApplicationDbContext context, ILogger<ImageController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        //[HttpPost]
        //public async Task<HttpResponseModel> UploadOneImage([FromForm] UploadImageModel model)
        //{
        //    try
        //    {
        //        List<string> imagesResponse = new List<string>();

        //        if (model.Image != null && model.Image.Count > 0)
        //        {
        //            var webRootPath = _webHostEnvironment.WebRootPath; // Lấy đường dẫn của thư mục wwwroot
        //            var now = DateTime.Now;
        //            var folderPath = Path.Combine(webRootPath, "Image", "warrantyImage", $"{now.Year}_{now.Month}");

        //            // Tạo thư mục nếu chưa tồn tại
        //            if (!Directory.Exists(folderPath))
        //            {
        //                Directory.CreateDirectory(folderPath);
        //            }

        //            _logger.LogInformation("UploadImage << SaveLocalFile Begin: " + DateTime.Now);

        //            foreach (var file in model.Image)
        //            {
        //                var fileName = $"{now.Ticks}{Path.GetExtension(file.FileName)}";
        //                var filePath = Path.Combine(folderPath, fileName);

        //                // Lưu file vào thư mục
        //                using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(fileStream);
        //                }

        //                // Đường dẫn tương đối tới hình ảnh đã lưu
        //                var relativePath = Path.Combine("Image", "warrantyImage", $"{now.Year}_{now.Month}", fileName);
        //                imagesResponse.Add(Path.Combine("/", relativePath).Replace("\\", "/"));
        //            }

        //            _logger.LogInformation("Notification UploadImage << SaveLocalFile End: " + DateTime.Now);
        //            return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, imagesResponse);
        //        }
        //        else
        //        {
        //            _logger.LogError("Notification Insert Image : image null");
        //            throw new Exception("Dữ liệu đầu vào không hợp lệ !");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Notification UploadImage: " + ex.Message);
        //        return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
        //    }
        //}

        [HttpPost]
        public async Task<HttpResponseModel> UploadImage([FromForm] UploadImageModel model)
        {
            try
            {
                List<string> imagesResponse = new List<string>();

                if (model.Image != null && model.Image.Count > 0)
                {
                    var webRootPath = _webHostEnvironment.WebRootPath; // Lấy đường dẫn của thư mục wwwroot
                    var now = DateTime.Now;
                    var folderPath = Path.Combine(webRootPath, "Image", "warrantyImage", $"{now.Year}_{now.Month}");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    _logger.LogInformation("UploadImage << SaveLocalFile Begin: " + DateTime.Now);

                    // Xóa các hình ảnh cũ nếu cần (Tùy thuộc vào yêu cầu của bạn)
                    // Ví dụ: Xóa tất cả hình ảnh cũ trong thư mục liên quan đến customer_id và facility_id

                    foreach (var file in model.Image)
                    {
                        var fileName = $"{now.Ticks}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(folderPath, fileName);

                        // Lưu file vào thư mục
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Đường dẫn tương đối tới hình ảnh đã lưu
                        var relativePath = Path.Combine("Image", "warrantyImage", $"{now.Year}_{now.Month}", fileName);
                        imagesResponse.Add(Path.Combine("/", relativePath).Replace("\\", "/"));
                    }

                    _logger.LogInformation("Notification UploadImage << SaveLocalFile End: " + DateTime.Now);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Successful", null, imagesResponse);
                }
                else
                {
                    _logger.LogError("Notification Insert Image : image null");
                    throw new Exception("Invalid input data !");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Notification UploadImage: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }


        [HttpGet]
        public async Task<HttpResponseModel> GetImageByWarantyId(int warranty_id)
        {
            try
            {
                var assignQuery = (from i in _context.warrantys.Where(d => d.warrantyId == warranty_id)
                                   join m in _context.images.Where(f => f.state == (short)EState.Active) on i.warrantyId equals m.warrantyId
                                   select new ImageModel
                                   {
                                       imageId = m.image_id,
                                       warantyId = m.warrantyId,
                                       link = m.link,
                                       linkName = m.link_name,
                                       type = m.type,
                                       createdAt = m.created_at,
                                   }
                        ).OrderByDescending(e => e.createdAt).ToList();
                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Successful", assignQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetImageByWarantyId: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<HttpResponseModel> DeleteImage(int id)
        {
            try
            {
                // Tìm hình ảnh trong cơ sở dữ liệu
                var image = await _context.images
                    .FirstOrDefaultAsync(i => i.image_id == id);

                if (image == null)
                {
                    _logger.LogError("Image not found: " + id);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Image does not exist !");
                }

                // Xóa tệp hình ảnh từ thư mục
                var webRootPath = _webHostEnvironment.WebRootPath;
                // Loại bỏ dấu gạch chéo đầu tiên từ đường dẫn liên kết hình ảnh
                var relativePath = image.link.TrimStart('/');
                // Kết hợp đường dẫn gốc với đường dẫn tương đối để tạo thành đường dẫn tuyệt đối
                var filePath = Path.Combine(webRootPath, relativePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Xóa bản ghi hình ảnh khỏi cơ sở dữ liệu
                _context.images.Remove(image);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image deleted successfully: " + id);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteImage: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

    }
}
