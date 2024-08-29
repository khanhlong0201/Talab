using CORE_TALAB.Data;
using CORE_TALAB.Enum;
using CORE_TALAB.Model.Reponse;
using CORE_TALAB.Models.Table;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Talab.Model.Search;
using Talab.Model.Warranty;
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
                Response.StatusCode = 400; // TODO: Chỗ này nếu lỗi thì e set 400, còn thành công thì e set 200
                if (search == null || search.Page <= 0 || search.PageSize <= 0)
                {
                    throw new Exception("Invalid input data !");
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
                    predicate = predicate.And(d => d.expirationDate.Value.Date >= search.SearchModel.FromDate.Value.Date);
                }

                if (search.SearchModel?.ToDate.HasValue == true)
                {
                    predicate = predicate.And(d => d.expirationDate.Value.Date <= search.SearchModel.ToDate.Value.Date);
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
                Response.StatusCode = 200;
                return Ok(new
                {
                    status = "success",
                    message = "Successfully",
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
                    message = ex.Message ?? "Error",
                    data = new List<object>(),
                    total = 0,
                    search.Page,
                    search.PageSize
                });
            }
        }

        [HttpPost("update/{id}")]
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
          List<IFormFile> ImageSrcList,
            [FromForm] string ImageSrcPreviewList
            )
        {
            try
            {
                Response.StatusCode = 400; // TODO: Chỗ này nếu lỗi thì e set 400, còn thành công thì e set 200
                List<BasicFile> files = JsonConvert.DeserializeObject<List<BasicFile>>(ImageSrcPreviewList);
                if (id <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Invalid warranty ID !", null);
                }
                // Xác thực dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(PatientName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the patient's name !", null);
                }
                if (string.IsNullOrWhiteSpace(PatientPhoneNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the patient's phone number !", null);
                }
                if (string.IsNullOrWhiteSpace(Clinic))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the clinic's name !", null);
                }
                if (string.IsNullOrWhiteSpace(LabName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the lab name !", null);
                }
                if (string.IsNullOrWhiteSpace(Doctor))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the doctor !", null);
                }
                if (string.IsNullOrWhiteSpace(Product))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the product !", null);
                }
                if (string.IsNullOrWhiteSpace(CodeNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the warranty card number !", null);
                }
                if (ExpirationDate == default(DateTime))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the warranty card's expiration date ! !", null);
                }
                if ((ImageSrcList == null || ImageSrcList.Count == 0) &&(files ==null || files.Count ==0))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not uploaded the warranty card image !");
                }
                var checkExit = _context.warrantys
    .FirstOrDefault(d => d.codeNumber == CodeNumber && d.warrantyId != id);

                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "The warranty card number already exists !");
                }
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Tìm bảo hành theo ID
                var warranty =  _context.warrantys.Find(id);

                if (warranty == null)
                {
                    _logger.LogWarning("Warranty not found with ID: " + id);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Warranty does not exist !", null);
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

                var fileNameList = files.Select(i => i.Title).ToList();
                var needRemoveImages = _context.images
                                                                                            .Where(i => i.warrantyId == id)
                                                                                            .AsEnumerable() // Chuyển đổi thành danh sách trong bộ nhớ
                                                                                            .Where(i =>!fileNameList.Contains(i.link_name))
                                                                                            .ToList();
                foreach (var image in needRemoveImages)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), image.link);

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
                _context.images.RemoveRange(needRemoveImages);

                // Thêm các hình ảnh mới
                var imagesResponse = new List<string>();
                var now = DateTime.Now;
                //var folderPath = Path.Combine( "Image", "warrantyImage");

                //if (!Directory.Exists(folderPath))
                //{
                //    Directory.CreateDirectory(folderPath);
                //}
                var folderPath = Path.Combine("wwwroot", "Image", "warrantyImage");
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
                var serverUrl = _configuration.GetSection("Server").Value;

                foreach (var file in ImageSrcList)
                {
                    var uniqueId = Guid.NewGuid().ToString();
                    var fileName = $"{uniqueId}_{now.Ticks}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    // Tạo đường dẫn URL
                    var imageUrl = $"{folderPath}/{fileName}".Replace("\\", "/");
                    var imageUrlResponse = $"{serverUrl}/{imageUrl}".Replace("\\", "/");
                    imagesResponse.Add(imageUrlResponse);

                    var image = new images
                    {
                        warrantyId = warranty.warrantyId,
                        link = imageUrl,
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Warranty update Successfully");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NO_CHANGE, "No changes were made !", null);
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the patient's name !", null);
                }
                if (string.IsNullOrWhiteSpace(PatientPhoneNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (string.IsNullOrWhiteSpace(Clinic))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the patient's phone number !", null);
                }
                if (string.IsNullOrWhiteSpace(LabName))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the lab name !", null);
                }
                if (string.IsNullOrWhiteSpace(Doctor))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the doctor !", null);
                }
                if (string.IsNullOrWhiteSpace(Product))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the product !", null);
                }
                if (string.IsNullOrWhiteSpace(CodeNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the warranty card number !", null);
                }
                if (ExpirationDate == default(DateTime))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "You have not entered the warranty card expiration date !", null);
                }
                if (ImageSrcList == null || ImageSrcList.Count == 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Warranty card image has not been uploaded. !");
                }

                var checkExit = _context.warrantys
                    .FirstOrDefault(d => d.codeNumber == CodeNumber);

                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "The warranty card number already exists !");
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
                    var serverUrl = _configuration.GetSection("Server").Value;
                    var now = DateTime.Now;
                
                    var folderPath = Path.Combine( "wwwroot", "Image", "warrantyImage");
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(),folderPath);
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    foreach (var file in ImageSrcList)
                    {
                        var uniqueId = Guid.NewGuid().ToString();
                        var fileName = $"{uniqueId}_{now.Ticks}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Tạo đường dẫn URL
                        var imageUrl = $"{folderPath}/{fileName}".Replace("\\", "/");

                        var image = new images
                        {
                            warrantyId = warranty.warrantyId,
                            link = imageUrl,
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Saved Successfully");
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Invalid code !");
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Warranty card not found !");
                }

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Successfully", null, result);
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Invalid code !");
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Warranty card not found !");
                }

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Successfully", null, result);
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Warranty card does not exist !", null);
                }

                // Tìm danh sách hình ảnh liên quan đến bảo hành này
                var images =  _context.images
                    .Where(i => i.warrantyId == Id)
                    .ToList();

                // Xóa các hình ảnh từ thư mục
                //var webRootPath = _webHostEnvironment.WebRootPath;
                foreach (var image in images)
                {
                    var relativePath = image.link.TrimStart('/');
                    var filePath = relativePath;//Path.Combine(Directory.GetCurrentDirectory(), relativePath);

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

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Warranty card deleted Successfully !", null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting warranty and images: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }
        [HttpGet("authByCode/{code}")]
        public async Task<HttpResponseModel> AuthByCode(string code)
        {
            try
            {
                Response.StatusCode = 400;
                var auth = _context.auth.Where(d => d.key == "code").FirstOrDefault();
                if (auth!=null)
                {
                    if(auth.name == code)
                    {
                        Response.StatusCode = 200;
                        return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Login successful.", null);
                    }
                    else
                    {
                        return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Invalid code !", null);
                    }
                }
                else
                {
                     return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Code does not exist in the database !", null);

                }
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
                return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Invalid ID list", null);
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "No warranty card found !", null);
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
                    var filePath = relativePath;//Path.Combine(webRootPath, relativePath);

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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Successfully deleted warranty list and images !", null);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NO_CHANGE, "No changes were made !", null);
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
                return StatusCode(500, "System error when fetching image.");
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
