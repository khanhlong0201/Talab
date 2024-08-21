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
using CORE_TALAB.Model.Reponse;
using CORE_TALAB.Data;
using CORE_TALAB.Model.Reponse;
using System.Net.NetworkInformation;
using Talab.Model.Warranty;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Talab.Model.Search;
using LinqKit;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Reflection;
using System.Reflection;
using Talab.Model.Image;
using Newtonsoft.Json;
namespace Talab.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WarrantyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WarrantyController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WarrantyController(ApplicationDbContext context, ILogger<WarrantyController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        public IActionResult GetWarrantys([FromBody] SearchQueryModel search)
        {
            try
            {
                if (search == null || search.Page <= 0 || search.PageSize <= 0)
                {
                    throw new Exception("Dữ liệu đầu vào không hợp lệ !");
                }

              

                // Tạo predicate cho tìm kiếm
                var predicate = PredicateBuilder.New<Warrantys>(d => d.state == (short)EState.Active);

                if (!string.IsNullOrWhiteSpace(search.SearchModel?.SearchString))
                {
                    var searchString = search.SearchModel.SearchString.ToLower();
                    predicate = predicate.And(d =>
                        d.patientName.ToLower().Contains(searchString) ||
                        d.patientPhoneNumber.ToLower().Contains(searchString) ||
                        d.codeNumber.ToLower().Contains(searchString) ||
                        d.clinic.ToLower().Contains(searchString) ||
                        d.labName.ToLower().Contains(searchString) ||
                        d.doctor.ToLower().Contains(searchString) ||
                        d.product.ToLower().Contains(searchString)
                    );
                }

                if (search.SearchModel?.FromDate.HasValue == true)
                {
                    predicate = predicate.And(d => d.expirationDate >= search.SearchModel.FromDate);
                }

                if (search.SearchModel?.ToDate.HasValue == true)
                {
                    predicate = predicate.And(d => d.expirationDate <= search.SearchModel.ToDate);
                }

                // Truy vấn cơ sở dữ liệu
                var query = _context.warrantys.Where(predicate);
                // Tính tổng số bản ghi
                var total = query.Count();
                // Xử lý sắp xếp động
                if (!string.IsNullOrWhiteSpace(search.Sort))
                {
                    var sortProperty = search.Sort.Trim();
                    var sortDirection = search.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";

                    // Xác minh rằng thuộc tính tồn tại trên lớp Warrantys
                    var propertyInfo = typeof(Warrantys).GetProperty(sortProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        var parameter = Expression.Parameter(typeof(Warrantys), "w");
                        var property = Expression.Property(parameter, propertyInfo);
                        var lambda = Expression.Lambda<Func<Warrantys, object>>(Expression.Convert(property, typeof(object)), parameter);

                        query = sortDirection == "descending"
                            ? query.OrderByDescending(lambda)
                            : query.OrderBy(lambda);
                    }
                    else
                    {
                        // Nếu thuộc tính không hợp lệ, sắp xếp theo mặc định
                        query = query.OrderByDescending(d => d.created_at);
                    }
                }
                else
                {
                    // Sắp xếp theo mặc định nếu không có cột sắp xếp
                    query = query.OrderByDescending(d => d.created_at);
                }

                // Phân trang và lấy dữ liệu
                var warrantyDB = query
                    .Skip((search.Page - 1) * search.PageSize)
                    .Take(search.PageSize)
                    .ToList();

                var warrantyQuery = warrantyDB.Select(a => new WarrantyViewModel
                {
                    WarrantyId = a.warrantyId,
                    PatientName = a.patientName,
                    PatientPhoneNumber = a.patientPhoneNumber,
                    Clinic = a.clinic,
                    LabName = a.labName,
                    Doctor = a.doctor,
                    Product = a.product,
                    CodeNumber = a.codeNumber,
                    ExpirationDate = a.expirationDate,
                    CreatedAt = a.created_at,
                    UpdatedAt = a.updated_at
                }).ToList();

                return Ok(new
                {
                    status = "success",
                    message = "Thành công",
                    data = warrantyQuery,
                    total,
                    search.Page,
                    search.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Notification GetWarrantys: " + ex.Message);
                return Ok(new
                {
                    status = "error",
                    message = ex.Message ?? "Thất bại",
                    data = new List<object>(),
                    total = 0,
                    search.Page,
                    search.PageSize
                });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<HttpResponseModel> UpdateWarranty(
            int id,
            [FromForm] string? PatientName,
            [FromForm] string? PatientPhoneNumber,
            [FromForm] string? Clinic,
            [FromForm] string? LabName,
            [FromForm] string? Doctor,
            [FromForm] string? Product,
            [FromForm] string? CodeNumber,
            [FromForm] DateTime? ExpirationDate,
            [FromForm] List<IFormFile> ImageSrcList,
            [FromForm] string ImageSrcPreviewList
            )
        {
            try
            {
                Response.StatusCode = 400; // TODO: Chỗ này nếu lỗi thì e set 400, còn thành công thì e set 200
                List<BasicFile> files = JsonConvert.DeserializeObject<List<BasicFile>>(ImageSrcPreviewList);
                if (id <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "ID bảo hành không hợp lệ", null);
                }
                // Xác thực dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(PatientName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (string.IsNullOrWhiteSpace(PatientPhoneNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số điện thoại bệnh nhân !", null);
                }
                if (string.IsNullOrWhiteSpace(Clinic))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (string.IsNullOrWhiteSpace(LabName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (string.IsNullOrWhiteSpace(Doctor))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (string.IsNullOrWhiteSpace(Product))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (string.IsNullOrWhiteSpace(CodeNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                if (ExpirationDate == default(DateTime))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if ((ImageSrcList == null || ImageSrcList.Count == 0) &&(files ==null || files.Count ==0))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành !");
                }
                var checkExit = _context.warrantys
    .FirstOrDefault(d => d.codeNumber == CodeNumber && d.warrantyId != id);

                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Tìm bảo hành theo ID
                var warranty =  _context.warrantys.Find(id);

                if (warranty == null)
                {
                    _logger.LogWarning("Warranty not found with ID: " + id);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Bảo hành không tồn tại", null);
                }

                // Cập nhật thông tin bảo hành
                warranty.patientName = PatientName;
                warranty.patientPhoneNumber = PatientPhoneNumber;
                warranty.clinic = Clinic;
                warranty.labName = LabName;
                warranty.doctor = Doctor;
                warranty.product = Product;
                warranty.codeNumber = CodeNumber;
                warranty.expirationDate = ExpirationDate;
                warranty.updated_at = DateTime.Now;

                _context.warrantys.Update(warranty);

                // Xử lý hình ảnh
                //var imageById = _context.images
                //    .Where(i => i.warrantyId == id)
                //    .ToList();

                var existingImages = _context.images
    .Where(i => i.warrantyId == id)
    .AsEnumerable() // Chuyển đổi thành danh sách trong bộ nhớ
    .Where(i => files != null
        && files.Count > 0
        && !files.Any(d => d.Title.Contains(i.link_name)))
    .ToList();

                // Xóa các hình ảnh cũ từ thư mục
                var webRootPath = _webHostEnvironment.WebRootPath;
                foreach (var image in existingImages)
                {
                    var relativePath = image.link.TrimStart('/');
                    var filePath = Path.Combine(webRootPath, relativePath);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation("Deleted file: " + filePath);
                    }
                    else
                    {
                        _logger.LogWarning("File not found: " + filePath);
                    }
                }

                // Xóa các hình ảnh cũ khỏi cơ sở dữ liệu
                _context.images.RemoveRange(existingImages);

                // Thêm các hình ảnh mới
                var imagesResponse = new List<string>();
                var now = DateTime.Now;
                var folderPath = Path.Combine(webRootPath, "Image", "warrantyImage");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var file in ImageSrcList)
                {
                    var uniqueId = Guid.NewGuid().ToString();
                    var fileName = $"{uniqueId}_{now.Ticks}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var relativePath = Path.Combine("Image", "warrantyImage");
                    imagesResponse.Add(Path.Combine("/", relativePath).Replace("\\", "/"));

                    var image = new images
                    {
                        warrantyId = warranty.warrantyId,
                        link = Path.Combine("/", relativePath).Replace("\\", "/"),
                        link_name = Path.GetFileName(fileName),
                        type = EType.Warranty.ToString(),
                        state = (short)EState.Active,
                        created_at = DateTime.Now,
                    };
                    _context.images.Add(image);
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    await transaction.CommitAsync();
                    Response.StatusCode = 200;
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Cập nhật bảo hành thành công");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NO_CHANGE, "Không có thay đổi nào được thực hiện", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty UpdateWarranty: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<HttpResponseModel> CreateWarranty(
           [FromForm] string? PatientName,
           [FromForm] string? PatientPhoneNumber,
           [FromForm] string? Clinic,
           [FromForm] string? LabName,
           [FromForm] string? Doctor,
           [FromForm] string? Product,
           [FromForm] string? CodeNumber,
           [FromForm] DateTime? ExpirationDate,
           List<IFormFile> ImageSrcList)
        {
            try
            {
                Response.StatusCode = 400; // TODO: Chỗ này nếu lỗi thì e set 400, còn thành công thì e set 200
                if (string.IsNullOrWhiteSpace(PatientName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (string.IsNullOrWhiteSpace(PatientPhoneNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (string.IsNullOrWhiteSpace(Clinic))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (string.IsNullOrWhiteSpace(LabName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (string.IsNullOrWhiteSpace(Doctor))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (string.IsNullOrWhiteSpace(Product))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (string.IsNullOrWhiteSpace(CodeNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                if (ExpirationDate == default(DateTime))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if (ImageSrcList == null || ImageSrcList.Count == 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành !");
                }

                var checkExit =  _context.warrantys
                    .FirstOrDefault(d => d.codeNumber == CodeNumber);

                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Lưu thông tin bảo hành
                    var warranty = new Warrantys
                    {
                        patientName = PatientName,
                        patientPhoneNumber = PatientPhoneNumber,
                        clinic = Clinic,
                        labName = LabName,
                        doctor = Doctor,
                        product = Product,
                        codeNumber = CodeNumber,
                        expirationDate = ExpirationDate,
                        created_at = DateTime.Now,
                        updated_at = null,
                        state = (short)EState.Active
                    };
                    _context.warrantys.Add(warranty);
                    await _context.SaveChangesAsync();

                    // Lưu hình ảnh
                    var imagesResponse = new List<string>();
                    var webRootPath = _webHostEnvironment.WebRootPath;
                    var now = DateTime.Now;
                    var folderPath = Path.Combine(webRootPath, "Image", "warrantyImage");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    foreach (var file in ImageSrcList)
                    {
                        // Tạo một GUID mới để đảm bảo tính duy nhất
                        var uniqueId = Guid.NewGuid().ToString();

                        // Sử dụng GUID kết hợp với thời gian hiện tại và phần mở rộng của tệp
                        var fileName = $"{uniqueId}_{now.Ticks}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(folderPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var relativePath = Path.Combine("Image", "warrantyImage");
                        imagesResponse.Add(Path.Combine("/", relativePath).Replace("\\", "/"));

                        var image = new images
                        {
                            warrantyId = warranty.warrantyId,
                            link = Path.Combine("/", relativePath).Replace("\\", "/"),
                            link_name = Path.GetFileName(fileName),
                            type = EType.Warranty.ToString(),
                            state = (short)EState.Active,
                            created_at = DateTime.Now,
                        };
                        _context.images.Add(image);
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    Response.StatusCode = 200;
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Đã lưu thành công");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Ném lại lỗi để xử lý ngoại lệ toàn cục
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty CreateWarranty: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpGet("{cardNumber}")]
        public async Task<HttpResponseModel> GetWarrantyWithImagesByCardNumber(string cardNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(cardNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Mã số không hợp lệ");
                }

                // Base URL cho hình ảnh
                var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v1/Warranty/image/";

                var result = _context.warrantys
                    .Where(w => w.state == (short)EState.Active && w.codeNumber == cardNumber)
                    .GroupJoin(
                        _context.images.Where(i => i.state == (short)EState.Active),
                        w => w.warrantyId,
                        i => i.warrantyId,
                        (w, images) => new
                        {
                            Warranty = w,
                            Images = images
                        })
                    .Select(wi => new WarrantyReponseModel
                    {
                        WarrantyId = wi.Warranty.warrantyId,
                        PatientName = wi.Warranty.patientName,
                        PatientPhoneNumber = wi.Warranty.patientPhoneNumber,
                        Clinic = wi.Warranty.clinic,
                        LabName = wi.Warranty.labName,
                        Doctor = wi.Warranty.doctor,
                        Product = wi.Warranty.product,
                        CodeNumber = wi.Warranty.codeNumber,
                        ExpirationDate = wi.Warranty.expirationDate,
                        CreatedAt = wi.Warranty.created_at,
                        UpdatedAt = wi.Warranty.updated_at,
                        State = wi.Warranty.state,
                        FileWithSrcList = wi.Images.Select(i => new BasicFile()
                        {
                            Src = $"{baseUrl}{i.link_name}",
                            Id = i.image_id.ToString(),
                            Title = i.link_name
                        }).ToList() // Tạo URL đầy đủ
                    })
                    .SingleOrDefault();

                if (result == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tìm thấy");
                }

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty GetWarrantyWithImagesByCardNumber: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }


        [HttpGet("id/{id}")]
        public async Task<HttpResponseModel> GetWarrantyWithImagesById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Mã số không hợp lệ");
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v1/Warranty/image/"; // Base URL cho hình ảnh

                var result = _context.warrantys
                    .Where(w => w.state == (short)EState.Active && w.warrantyId == id)
                    .GroupJoin(
                        _context.images.Where(i => i.state == (short)EState.Active),
                        w => w.warrantyId,
                        i => i.warrantyId,
                        (w, images) => new
                        {
                            Warranty = w,
                            Images = images
                        })
                    .Select(wi => new WarrantyReponseModel
                    {
                        WarrantyId = wi.Warranty.warrantyId,
                        PatientName = wi.Warranty.patientName,
                        PatientPhoneNumber = wi.Warranty.patientPhoneNumber,
                        Clinic = wi.Warranty.clinic,
                        LabName = wi.Warranty.labName,
                        Doctor = wi.Warranty.doctor,
                        Product = wi.Warranty.product,
                        CodeNumber = wi.Warranty.codeNumber,
                        ExpirationDate = wi.Warranty.expirationDate,
                        CreatedAt = wi.Warranty.created_at,
                        UpdatedAt = wi.Warranty.updated_at,
                        State = wi.Warranty.state,
                        ImageSrcPreviewList = wi.Images.Select(i => new BasicFile()
                        {
                            Id = i.image_id.ToString(),
                            Src = $"{baseUrl}{i.link_name}",
                            Title = i.link_name
                        }).ToList(),  // Tạo URL đầy đủ
                        ImageLinkNameSrcPreviewList = wi.Images.Select(i => $"{i.link_name}").ToList(),  // Tạo URL đầy đủ
                    })
                    .SingleOrDefault();

                if (result == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tìm thấy");
                }

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty GetWarrantyWithImagesById: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }


        [HttpDelete("{Id}")]
        public async Task<HttpResponseModel> DeleteWarranty(int Id = -1)
        {
            try
            {
                // Bắt đầu giao dịch
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Tìm bảo hành cần xóa
                var warranty =  _context.warrantys
                    .FirstOrDefault(i => i.warrantyId == Id);

                if (warranty == null)
                {
                    _logger.LogWarning("Warranty not found with ID: " + Id);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tồn tại", null);
                }

                // Tìm danh sách hình ảnh liên quan đến bảo hành này
                var images =  _context.images
                    .Where(i => i.warrantyId == Id)
                    .ToList();

                // Xóa các hình ảnh từ thư mục
                var webRootPath = _webHostEnvironment.WebRootPath;
                foreach (var image in images)
                {
                    var relativePath = image.link.TrimStart('/');
                    var filePath = Path.Combine(webRootPath, relativePath);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation("Deleted file: " + filePath);
                    }
                    else
                    {
                        _logger.LogWarning("File not found: " + filePath);
                    }
                }

                // Xóa các hình ảnh khỏi cơ sở dữ liệu
                _context.images.RemoveRange(images);

                // Xóa bảo hành khỏi cơ sở dữ liệu
                _context.warrantys.Remove(warranty);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Xóa thẻ bảo hành thành công", null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting warranty and images: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpDelete("deleteMany")]
        public async Task<HttpResponseModel> DeleteWarranties([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Danh sách ID không hợp lệ", null);
            }
            try
            {
                // Bắt đầu giao dịch
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Tìm tất cả các bảo hành cần xóa
                var warranties =  _context.warrantys
                    .Where(w => ids.Contains(w.warrantyId))
                    .ToList();

                if (!warranties.Any())
                {
                    _logger.LogWarning("No warranties found for the provided IDs.");
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Không tìm thấy thẻ bảo hành nào", null);
                }

                // Tìm danh sách hình ảnh liên quan đến các bảo hành này
                var warrantyIds = warranties.Select(w => w.warrantyId).ToList();
                var images =  _context.images
                    .Where(i => warrantyIds.Contains(i.warrantyId))
                    .ToList();

                // Xóa các hình ảnh từ thư mục
                var webRootPath = _webHostEnvironment.WebRootPath;
                foreach (var image in images)
                {
                    var relativePath = image.link.TrimStart('/');
                    var filePath = Path.Combine(webRootPath, relativePath);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation("Deleted file: " + filePath);
                    }
                    else
                    {
                        _logger.LogWarning("File not found: " + filePath);
                    }
                }

                // Xóa các hình ảnh khỏi cơ sở dữ liệu
                _context.images.RemoveRange(images);

                // Xóa các bảo hành khỏi cơ sở dữ liệu
                _context.warrantys.RemoveRange(warranties);

                // Lưu các thay đổi vào cơ sở dữ liệu
                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    await transaction.CommitAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Đã xóa danh sách bảo hành và hình ảnh thành công", null);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NO_CHANGE, "Không có thay đổi nào được thực hiện", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting warranties and images: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpGet("image/{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                // Đường dẫn tới thư mục chứa hình ảnh0
                var webRootPath = _webHostEnvironment.WebRootPath;
                var folderPath = Path.Combine(webRootPath, "Image", "warrantyImage");
                var filePath = Path.Combine(folderPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(); // Trả về 404 nếu không tìm thấy hình ảnh
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath); // Lấy loại nội dung từ phần mở rộng của tệp

                return File(fileBytes, contentType); // Trả về hình ảnh dưới dạng tệp
            }
            catch (Exception ex)
            {
                _logger.LogError("GetImage: " + ex.Message);
                return StatusCode(500, "Lỗi hệ thống khi lấy hình ảnh.");
            }
        }

        // Helper method to determine content type based on file extension
        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream", // Default type
            };
        }


    }
}
