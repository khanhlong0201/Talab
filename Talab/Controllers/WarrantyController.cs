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

                // Tính tổng số bản ghi
                var total = _context.warrantys
                    .Where(d => d.state == (short)EState.Active)
                    .Count();

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
                var query = _context.warrantys.AsNoTracking().Where(predicate);

                // Xử lý sắp xếp
                switch (search.Sort?.ToLower())
                {
                    case "patientname":
                        query = search.SortDirection?.ToLower() == "desc" ?
                            query.OrderByDescending(d => d.patientName) :
                            query.OrderBy(d => d.patientName);
                        break;
                    case "patientphonenumber":
                        query = search.SortDirection?.ToLower() == "desc" ?
                            query.OrderByDescending(d => d.patientPhoneNumber) :
                            query.OrderBy(d => d.patientPhoneNumber);
                        break;
                    case "codenumber":
                        query = search.SortDirection?.ToLower() == "desc" ?
                            query.OrderByDescending(d => d.codeNumber) :
                            query.OrderBy(d => d.codeNumber);
                        break;
                    case "clinic":
                        query = search.SortDirection?.ToLower() == "desc" ?
                            query.OrderByDescending(d => d.clinic) :
                            query.OrderBy(d => d.clinic);
                        break;
                    case "expirationdate":
                        query = search.SortDirection?.ToLower() == "desc" ?
                            query.OrderByDescending(d => d.expirationDate) :
                            query.OrderBy(d => d.expirationDate);
                        break;
                    // Thêm các trường sắp xếp khác tại đây
                    default:
                        query = query.OrderByDescending(d => d.created_at); // Mặc định nếu không có sắp xếp
                        break;
                }

                // Phân trang và lấy dữ liệu
                var warrantyDB = query
                    .Skip((search.Page - 1) * search.PageSize)
                    .Take(search.PageSize)
                    .ToList();

                var warrantyQuery = warrantyDB.Select(a => new WarrantyModel
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



        [HttpPut]
        public async Task<HttpResponseModel> UpdateWarranty([FromBody] WarrantyModel request)
        {
            try
            {
                if (request == null || request.WarrantyId <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Dữ liệu đầu vào không hợp lệ !", null);
                }
                if (request.PatientName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (request.PatientPhoneNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (request.Clinic + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (request.LabName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (request.Doctor + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (request.Product + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (request.CodeNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                var defaultDate = new DateTime();
                if (request.ExpirationDate == null || request.ExpirationDate == defaultDate)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if (request.ImageSrcList == null || request.ImageSrcList.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                var existingWarranty = await _context.warrantys.FirstOrDefaultAsync(a => a.warrantyId == request.WarrantyId);
                if (existingWarranty == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Không tìm thấy mã thẻ bảo thành !", null);
                }
                var checkExit = _context.warrantys.FirstOrDefault(d => d.codeNumber == request.CodeNumber && d.warrantyId != request.WarrantyId);
                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }
                using var transaction = _context.Database.BeginTransaction();
                existingWarranty.patientName = request.PatientName;
                existingWarranty.clinic = request.Clinic;
                existingWarranty.labName = request.LabName;
                existingWarranty.doctor = request.Doctor;
                existingWarranty.product = request.Product;
                existingWarranty.codeNumber = request.CodeNumber;
                existingWarranty.expirationDate = request.ExpirationDate;
                existingWarranty.updated_at = DateTime.Now;
                var status = _context.SaveChanges();
                if (status <= 0)
                {
                    transaction.Rollback();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Lỗi khi cập nhật thẻ bảo thành !", null);
                }
                if (request.ImageSrcList != null && request.ImageSrcList.Count > 0)
                {
                    foreach (var link in request.ImageSrcList)
                    {
                        images imgae = new images
                        {
                            warrantyId = existingWarranty.warrantyId,
                            link = link,
                            link_name = Path.GetFileName(link),
                            type = EType.Warranty.ToString(),
                            state = (short)EState.Active,
                            created_at = DateTime.Now,
                        };
                        _context.images.Add(imgae);
                        var status_Image = _context.SaveChanges();
                        if (status_Image <= 0)
                        {
                            transaction.Rollback();
                            return HttpResponseModel.Make(REPONSE_ENUM.RS_AU_NOT_OK, "Lỗi lưu hình ảnh thẻ bảo thành !");
                        }
                    }
                }
                transaction.Commit();
                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Đã lưu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError("Waranty UpdateWarranty: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<HttpResponseModel> CreateWarranty([FromBody] WarrantyModel request)
        {
            try
            {
                if (request == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Dữ liệu đầu vào không hợp lệ !", null);
                }
                if (request.PatientName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (request.PatientPhoneNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (request.Clinic + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (request.LabName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (request.Doctor + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (request.Product + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (request.CodeNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                var defaultDate = new DateTime();
                if (request.ExpirationDate == null || request.ExpirationDate == defaultDate)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if (request.ImageSrcList == null || request.ImageSrcList.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                var checkExit = _context.warrantys.FirstOrDefault(d => d.codeNumber == request.CodeNumber);
                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }
                using var transaction = _context.Database.BeginTransaction();
                Warrantys warranty = new Warrantys()
                {
                    patientName = request.PatientName,
                    patientPhoneNumber = request.PatientPhoneNumber,
                    clinic = request.Clinic,
                    labName = request.LabName,
                    doctor = request.Doctor,
                    product = request.Product,
                    codeNumber = request.CodeNumber,
                    expirationDate = request.ExpirationDate,
                    created_at = DateTime.Now,
                    updated_at = null as DateTime?,
                    state = (short)EState.Active
                };
                _context.warrantys.Add(warranty);
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    if (request.ImageSrcList != null && request.ImageSrcList.Count > 0)
                    {
                        foreach (var link in request.ImageSrcList)
                        {
                            images imgae = new images
                            {
                                warrantyId = warranty.warrantyId,
                                link = link,
                                link_name = Path.GetFileName(link),
                                type = EType.Warranty.ToString(),
                                state = (short)EState.Active,
                                created_at = DateTime.Now,
                            };
                            _context.images.Add(imgae);
                            var status_Image = _context.SaveChanges();
                            if (status_Image <= 0)
                            {
                                transaction.Rollback();
                                return HttpResponseModel.Make(REPONSE_ENUM.RS_AU_NOT_OK, "Lỗi lưu hình ảnh thẻ bảo thành !");
                            }
                        }
                    }
                    transaction.Commit();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Đã lưu thành công");
                }
                else
                {
                    transaction.Rollback();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Lỗi không thêm được !");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty CreateWarranty: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpGet("{cardNumber?}")]
        public async Task<HttpResponseModel> GetWarrantyBycodeNumber(string cardNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(cardNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Mã số không hợp lệ");
                }

                // Lấy bảo hành theo mã số
                var warrantyDB = await _context.warrantys.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.state == (short)EState.Active && d.codeNumber == cardNumber);

                if (warrantyDB == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tìm thấy");
                }

                // Chuyển đổi đối tượng lấy được thành WarrantyModel
                var warrantyModel = new WarrantyModel
                {
                    WarrantyId = warrantyDB.warrantyId,
                    PatientName = warrantyDB.patientName,
                    PatientPhoneNumber = warrantyDB.patientPhoneNumber,
                    Clinic = warrantyDB.clinic,
                    LabName = warrantyDB.labName,
                    Doctor = warrantyDB.doctor,
                    Product = warrantyDB.product,
                    CodeNumber = warrantyDB.codeNumber,
                    ExpirationDate = warrantyDB.expirationDate,
                    CreatedAt = warrantyDB.created_at,
                    UpdatedAt = warrantyDB.updated_at
                };

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, warrantyModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty GetWarrantyBycodeNumber: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }

        [HttpGet("id/{id}")]
        public async Task<HttpResponseModel> GetWarrantyById(int id)
        {
            try
            {
                // Kiểm tra xem id có hợp lệ không
                if (id <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "ID không hợp lệ");
                }

                // Lấy bảo hành theo ID
                var warrantyDB = await _context.warrantys.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.state == (short)EState.Active && d.warrantyId == id);

                if (warrantyDB == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tìm thấy");
                }

                // Chuyển đổi đối tượng lấy được thành WarrantyModel
                var warrantyModel = new WarrantyModel
                {
                    WarrantyId = warrantyDB.warrantyId,
                    PatientName = warrantyDB.patientName,
                    PatientPhoneNumber = warrantyDB.patientPhoneNumber,
                    Clinic = warrantyDB.clinic,
                    LabName = warrantyDB.labName,
                    Doctor = warrantyDB.doctor,
                    Product = warrantyDB.product,
                    CodeNumber = warrantyDB.codeNumber,
                    ExpirationDate = warrantyDB.expirationDate,
                    CreatedAt = warrantyDB.created_at,
                    UpdatedAt = warrantyDB.updated_at
                };

                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, warrantyModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty GetWarrantyById: " + ex.Message);
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
                var warranty = await _context.warrantys
                    .FirstOrDefaultAsync(i => i.warrantyId == Id);

                if (warranty == null)
                {
                    _logger.LogWarning("Warranty not found with ID: " + Id);
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Bảo hành không tồn tại", null);
                }

                // Xóa bảo hành
                _context.warrantys.Remove(warranty);

                // Tìm danh sách hình ảnh liên quan đến bảo hành này
                var images = await _context.images
                    .Where(i => i.warrantyId == Id)
                    .ToListAsync();

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

                // Lưu các thay đổi vào cơ sở dữ liệu
                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    await transaction.CommitAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Xóa thẻ bảo hành thành công", null);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NO_CHANGE, "Không có thay đổi nào được thực hiện", null);
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
                return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Danh sách ID không hợp lệ", null);
            }

            try
            {
                // Bắt đầu giao dịch
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Tìm tất cả các bảo hành cần cập nhật
                var warranties = await _context.warrantys
                    .Where(w => ids.Contains(w.warrantyId))
                    .ToListAsync();

                if (!warranties.Any())
                {
                    _logger.LogWarning("No warranties found for the provided IDs.");
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Không tìm thấy bảo hành nào", null);
                }

                // Cập nhật trạng thái của các bảo hành
                warranties.ForEach(w => w.state = 1);
                _context.warrantys.UpdateRange(warranties);

                // Tìm danh sách hình ảnh liên quan đến các bảo hành này
                var warrantyIds = warranties.Select(w => w.warrantyId).ToList();
                var images = await _context.images
                    .Where(i => warrantyIds.Contains(i.warrantyId))
                    .ToListAsync();

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

                // Cập nhật trạng thái của các hình ảnh
                images.ForEach(i => i.state = 1);
                _context.images.UpdateRange(images);

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
                _logger.LogError("Error updating warranties and images: " + ex.Message);
                return HttpResponseModel.Make(REPONSE_ENUM.RS_EXCEPTION, ex.Message);
            }
        }




    }
}
