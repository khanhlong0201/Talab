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
        public IActionResult GetWarrantys(int page = 1, int pageSize = 100, string PatientName ="", string PatientPhoneNumber ="",string Clinic ="", string LabName = "",
            string Doctor = "", string Product = "", string CodeNumber = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
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
                    && ((string.IsNullOrEmpty(PatientName) || (!string.IsNullOrEmpty(PatientName) && d.patientName.ToLower().Contains(PatientName.ToLower()))))
                    && ((string.IsNullOrEmpty(PatientPhoneNumber) || (!string.IsNullOrEmpty(PatientPhoneNumber) && d.patientPhoneNumber.ToLower().Contains(PatientPhoneNumber.ToLower()))))
                     && ((string.IsNullOrEmpty(Clinic) || (!string.IsNullOrEmpty(Clinic) && d.clinic.ToLower().Contains(Clinic.ToLower()))))
                      && ((string.IsNullOrEmpty(LabName) || (!string.IsNullOrEmpty(LabName) && d.clinic.ToLower().Contains(LabName.ToLower()))))
                      && ((string.IsNullOrEmpty(Doctor) || (!string.IsNullOrEmpty(Doctor) && d.doctor.ToLower().Contains(Doctor.ToLower()))))
                      && ((string.IsNullOrEmpty(Product) || (!string.IsNullOrEmpty(Product) && d.product.ToLower().Contains(Product.ToLower()))))
                       && ((string.IsNullOrEmpty(CodeNumber) || (!string.IsNullOrEmpty(CodeNumber) && d.codeNumber.ToLower().Contains(CodeNumber.ToLower()))))
                       && ((!fromDate.HasValue && fromDate == null) || (fromDate.HasValue && fromDate!=null && d.expirationDate>= fromDate))
                       && ((!toDate.HasValue && toDate == null) || (toDate.HasValue && toDate != null && d.expirationDate <= toDate)))
                    .OrderByDescending(d => d.created_at) // Sắp xếp trước khi phân trang
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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
                    datas = warrantyQuery,
                    total,  // Tổng số bản ghi
                    page,  // Trang hiện tại
                    pageSize  // Kích thước trang
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Notification GetWarrantys: " + ex.Message);
                return Ok(new
                {
                    status = "error",
                    message = "Thất bại",
                    datas = new List<object>(),
                    total = 0,  // Tổng số bản ghi là 0 khi lỗi
                    page,
                    pageSize
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Dữ liệu đầu vào không hợp lệ !",null);
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
                if (request.ListImageUrl == null || request.ListImageUrl.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                var existingWarranty = await _context.warrantys.FirstOrDefaultAsync(a => a.warrantyId == request.WarrantyId);
                if (existingWarranty == null)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Không tìm thấy mã thẻ bảo thành !",null);
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
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Lỗi khi cập nhật thẻ bảo thành !",null);
                }
                if (request.ListImageUrl != null && request.ListImageUrl.Count > 0)
                {
                    foreach (var link in request.ListImageUrl)
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
                if (request.PatientName + ""=="")
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
                if (request.ListImageUrl == null || request.ListImageUrl.Count <= 0)
                {
                    return HttpResponseModel.Make(REPONSE_ENUM.RS_NOT_OK, "Bạn chưa tải lên hình thẻ bảo hành lên !");
                }
                using var transaction = _context.Database.BeginTransaction();
                warrantys warranty = new warrantys()
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
                     updated_at= new DateTime(),
                     state = (short)EState.Active
                 };
                _context.warrantys.Add(warranty);
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    if (request.ListImageUrl != null && request.ListImageUrl.Count > 0)
                    {
                        foreach (var link in request.ListImageUrl)
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

        [HttpGet("{CodeNumber}")]
        public async Task<HttpResponseModel> GetWarrantyByCodeNumber(string CodeNumber = "")
        {
            try
            {
                var warrantysDB = _context.warrantys.AsNoTracking().
                    Where(d => d.state == (short)EState.Active && d.codeNumber == CodeNumber);
                var assignQuery = (from a in warrantysDB 
                                   select new WarrantyModel
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
                                   }
                );
                return HttpResponseModel.Make(REPONSE_ENUM.RS_OK, "Thành công", null, assignQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError("Warranty GetWarrantyByCodeNumber: " + ex.Message);
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
