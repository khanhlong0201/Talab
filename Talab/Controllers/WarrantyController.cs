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

        [HttpGet]
        public IActionResult GetWarrantys([FromBody] SearchQueryModel search)
        {
            try
            {
                if (search.page <= 0 || search.pageSize <= 0 || search==null)
                {
                    throw new Exception("Dữ liệu đầu vào không hợp lệ !");
                }

                // Tính tổng số bản ghi
                var total = _context.warrantys
                    .Where(d => d.state == (short)EState.Active)
                    .Count();

                // Lấy dữ liệu cho trang hiện tại với sắp xếp
                var warrantyDB = _context.warrantys.AsNoTracking()
                    .Where(d => d.state == (short)EState.Active
                    && ((string.IsNullOrEmpty(search.search.patientName) || (!string.IsNullOrEmpty(search.search.patientName) && d.patientName.ToLower().Contains(search.search.patientName.ToLower()))))
                    && ((string.IsNullOrEmpty(search.search.patientPhoneNumber) || (!string.IsNullOrEmpty(search.search.patientPhoneNumber) && d.patientPhoneNumber.ToLower().Contains(search.search.patientPhoneNumber.ToLower()))))
                     && ((string.IsNullOrEmpty(search.search.clinic) || (!string.IsNullOrEmpty(search.search.clinic) && d.clinic.ToLower().Contains(search.search.clinic.ToLower()))))
                      && ((string.IsNullOrEmpty(search.search.labName) || (!string.IsNullOrEmpty(search.search.labName) && d.clinic.ToLower().Contains(search.search.labName.ToLower()))))
                      && ((string.IsNullOrEmpty(search.search.doctor) || (!string.IsNullOrEmpty(search.search.doctor) && d.doctor.ToLower().Contains(search.search.doctor.ToLower()))))
                      && ((string.IsNullOrEmpty(search.search.product) || (!string.IsNullOrEmpty(search.search.product) && d.product.ToLower().Contains(search.search.product.ToLower()))))
                       && ((string.IsNullOrEmpty(search.search.codeNumber) || (!string.IsNullOrEmpty(search.search.codeNumber) && d.codeNumber.ToLower().Contains(search.search.codeNumber.ToLower()))))
                       && ((!search.search.fromDate.HasValue && search.search.fromDate == null) || (search.search.fromDate.HasValue && search.search.fromDate != null && d.expirationDate>= search.search.fromDate))
                       && ((!search.search.toDate.HasValue && search.search.toDate == null) || (search.search.toDate.HasValue && search.search.toDate != null && d.expirationDate <= search.search.toDate)))
                    .OrderByDescending(d => d.created_at) // Sắp xếp trước khi phân trang
                    .Skip((search.page - 1) * search.pageSize)
                    .Take(search.pageSize)
                    .ToList();

                var warrantyQuery = warrantyDB.Select(a => new WarrantyModel
                {
                    warrantyId = a.warrantyId,
                    patientName = a.patientName,
                    patientPhoneNumber = a.patientPhoneNumber,
                    clinic = a.clinic,
                    labName = a.labName,
                    doctor = a.doctor,
                    product = a.product,
                    codeNumber = a.codeNumber,
                    expirationDate = a.expirationDate,
                    createdAt = a.created_at,
                    updatedAt = a.updated_at
                }).ToList();

                return Ok(new
                {
                    status = "success",
                    message = "Thành công",
                    datas = warrantyQuery,
                    total,  // Tổng số bản ghi
                    search.page,  // Trang hiện tại
                    search.pageSize  // Kích thước trang
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Notification GetWarrantys: " + ex.Message);
                return Ok(new
                {
                    status = "error",
                    message = ex.Message !=null ? ex.Message : "Thất bại",
                    datas = new List<object>(),
                    total = 0,  // Tổng số bản ghi là 0 khi lỗi
                    search.page,
                    search.pageSize
                });
            }
        }

        [HttpPut]
        public async Task<HttpResponseModel> UpdateWarranty([FromBody] WarrantyModel request)
        {
            try
            {
                if (request == null || request.warrantyId <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Dữ liệu đầu vào không hợp lệ !",null);
                }
                if (request.patientName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (request.patientPhoneNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (request.clinic + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (request.labName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (request.doctor + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (request.product + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (request.codeNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                var defaultDate = new DateTime();
                if (request.expirationDate == null || request.expirationDate == defaultDate)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if (request.listImageUrl == null || request.listImageUrl.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                var existingWarranty = await _context.warrantys.FirstOrDefaultAsync(a => a.warrantyId == request.warrantyId);
                if (existingWarranty == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Không tìm thấy mã thẻ bảo thành !",null);
                }
                var checkExit = _context.warrantys.FirstOrDefault(d => d.codeNumber == request.codeNumber && d.warrantyId !=request.warrantyId);
                if (checkExit != null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }
                using var transaction = _context.Database.BeginTransaction();
                existingWarranty.patientName = request.patientName;
                existingWarranty.clinic = request.clinic;
                existingWarranty.labName = request.labName;
                existingWarranty.doctor = request.doctor;
                existingWarranty.product = request.product;
                existingWarranty.codeNumber = request.codeNumber;
                existingWarranty.expirationDate = request.expirationDate;
                existingWarranty.updated_at = DateTime.Now;
                var status = _context.SaveChanges();
                if (status <= 0)
                {
                    transaction.Rollback();
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Lỗi khi cập nhật thẻ bảo thành !",null);
                }
                if (request.listImageUrl != null && request.listImageUrl.Count > 0)
                {
                    foreach (var link in request.listImageUrl)
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

        [HttpPost]
        public async Task<HttpResponseModel> CreateWarranty([FromBody] WarrantyModel request)
        {
            try
            {
                if (request == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Dữ liệu đầu vào không hợp lệ !", null);
                }
                if (request.patientName + ""=="")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bệnh nhân !", null);
                }
                if (request.patientPhoneNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập sdt bệnh nhân !", null);
                }
                if (request.clinic + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên phòng khám !", null);
                }
                if (request.labName + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên Lab !", null);
                }
                if (request.doctor + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên bác sĩ !", null);
                }
                if (request.product + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập tên sản phẩm !", null);
                }
                if (request.codeNumber + "" == "")
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập số thẻ bảo hành !", null);
                }
                var defaultDate = new DateTime();
                if (request.expirationDate == null || request.expirationDate == defaultDate)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa nhập ngày hết hạn thẻ bảo hành !", null);
                }
                if (request.listImageUrl == null || request.listImageUrl.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                var checkExit = _context.warrantys.FirstOrDefault(d=> d.codeNumber == request.codeNumber);
                if (checkExit!= null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Số thẻ bảo hành đã tồn tại !");
                }
                using var transaction = _context.Database.BeginTransaction();
                warrantys warranty = new warrantys()
                 {
                     patientName = request.patientName,
                     patientPhoneNumber = request.patientPhoneNumber,
                     clinic = request.clinic,
                     labName = request.labName,
                     doctor = request.doctor,
                     product = request.product,
                     codeNumber = request.codeNumber,
                     expirationDate = request.expirationDate,
                     created_at = DateTime.Now,
                     updated_at= new DateTime(),
                     state = (short)EState.Active
                 };
                _context.warrantys.Add(warranty);
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    if (request.listImageUrl != null && request.listImageUrl.Count > 0)
                    {
                        foreach (var link in request.listImageUrl)
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

        [HttpGet("{codeNumber}")]
        public async Task<HttpResponseModel> GetWarrantyBycodeNumber(string codeNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(codeNumber))
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Mã số không hợp lệ");
                }

                // Lấy bảo hành theo mã số
                var warrantyDB = await _context.warrantys.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.state == (short)EState.Active && d.codeNumber == codeNumber);

                if (warrantyDB == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_FOUND, "Thẻ bảo hành không tìm thấy");
                }

                // Chuyển đổi đối tượng lấy được thành WarrantyModel
                var warrantyModel = new WarrantyModel
                {
                    warrantyId = warrantyDB.warrantyId,
                    patientName = warrantyDB.patientName,
                    patientPhoneNumber = warrantyDB.patientPhoneNumber,
                    clinic = warrantyDB.clinic,
                    labName = warrantyDB.labName,
                    doctor = warrantyDB.doctor,
                    product = warrantyDB.product,
                    codeNumber = warrantyDB.codeNumber,
                    expirationDate = warrantyDB.expirationDate,
                    createdAt = warrantyDB.created_at,
                    updatedAt = warrantyDB.updated_at
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
                    warrantyId = warrantyDB.warrantyId,
                    patientName = warrantyDB.patientName,
                    patientPhoneNumber = warrantyDB.patientPhoneNumber,
                    clinic = warrantyDB.clinic,
                    labName = warrantyDB.labName,
                    doctor = warrantyDB.doctor,
                    product = warrantyDB.product,
                    codeNumber = warrantyDB.codeNumber,
                    expirationDate = warrantyDB.expirationDate,
                    createdAt = warrantyDB.created_at,
                    updatedAt = warrantyDB.updated_at
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

    }
}
