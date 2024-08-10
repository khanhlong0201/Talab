using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CORE_TALAB.Enum
{
    public enum  ERightsGroup
    {
        [Description("Xem giám sát")]
        ViewMonitor = 5,


        [Description("Xem xử lý")]
        ViewHandling = 1,
        [Description("Xử lý tin báo")]
        HandlingEvent = 2,
        [Description("In biên bản")]
        PrintEvent = 3,
        [Description("Kết thúc tin báo")]
        FinishEvent = 4,
        [Description("Tạo tin cảnh báo")]
        CreateEvent = 6,

        //[Description("Chuyển tin báo")]
        //PushAlarmEvent = 7,
        //[Description("Cập nhật tin báo C07")]
        //PushUpdateEvent = 8,

        //---------------------------------------------
        [Description("Xem lịch sử trạng thái thiết bị")]
        ViewHistory = 10,
        [Description("Xuất Excel trạng thái thiết bị")]
        ExportExcelHistory = 11,
        
        [Description("Xem lịch sử cảnh báo cháy")]
        ViewHistoryAlarm = 16,
        [Description("Xuất Excel cảnh báo cháy")]
        ExportExcelHistoryAlarm = 17,




        //---------------------------------------------
        [Description("Xem người dùng")]
        ViewUser = 20,
        [Description("Thêm người dùng")]
        InsertUser = 21,
        [Description("Cập nhật người dùng")]
        UpdateUser = 22,
        [Description("Xóa người dùng")]
        DeleteUser = 23,
        [Description("Reset password người dùng")]
        ResetPasswordUser = 24,



        //---------------------------------------------
        [Description("Xem tổ chức")]
        ViewOrganization = 30,
        [Description("Thêm tổ chức")]
        InsertOrganization = 31,
        [Description("Cập nhật tổ chức")]
        UpdateOrganization = 32,
        [Description("Xóa tổ chức")]
        DeleteOrganization = 33,
        [Description("Hiệu chỉnh người dùng")]
        EditUserInOrganization = 34,



        //---------------------------------------------
        [Description("Xem tài khoản PC07")]
        ViewPC07 = 35,
        [Description("Thêm tài khoản PC07")]
        InsertPC07 = 36,
        [Description("Cập tài khoản PC07")]
        UpdatePC07 = 37,
        [Description("Xóa tài khoản PC07")]
        DeletePC07 = 38,
        [Description("Reset password tài khoản PC07")]
        ResetPasswordPC07 = 39,
        //---------------------------------------------
        [Description("Xem khách hàng")]
        ViewCustomer = 40,
        [Description("Thêm khách hàng")]
        InsertCustomer = 41,
        [Description("Cập khách hàng")]
        UpdateCustomer = 42,
        [Description("Xóa khách hàng")]
        DeleteCustomer = 43,
        [Description("Hiệu chỉnh thiết bị")]
        EditDeviceInCustomer = 44,


        //---------------------------------------------
        //[Description("Xem cơ sở")]
        //ViewFacility = 50,
        [Description("Thêm cơ sở")]
        InsertFacility = 51,
        [Description("Cập nhật cơ sở")]
        UpdateFacility = 52,
        [Description("Xóa cơ sở")]
        DeleteFacility = 53,
        [Description("Cập nhật hồ sơ cơ sở")]
        ViewUpdateDocumentFacility = 54,
        [Description("Thêm tài khoản cơ sở")]
        InsertAccountFacility = 55,
        //---------------------------------------------
        [Description("Xem tài liệu")]
        ViewDocument = 60,
        [Description("Thêm tài liệu")]
        InsertDocument = 61,
        [Description("Cập nhật tài liệu")]
        UpdateDocument = 62,
        [Description("Xóa tài liệu")]
        DeleteDocument = 63,
        //---------------------------------------------
        //[Description("Xem thiết bị")]
        //ViewDevice = 70,
        [Description("Thêm thiết bị")]
        InsertDevice = 71,
        [Description("Cập nhật thiết bị")]
        UpdateDevice = 72,
        [Description("Xóa thiết bị")]
        DeleteDevice = 73,

        //---------------------------------------------
        [Description("Xem nguồn lực")]
        ViewMarker = 80,
        [Description("Thêm nguồn lực")]
        InsertMarker = 81,
        [Description("Cập nhật nguồn lực")]
        UpdateMarker = 82,
        [Description("Xóa nguồn lực")]
        DeleteMarker = 83,

        //---------------------------------------------
        [Description("Xem nhóm quyền")]
        ViewGroup = 90,
        [Description("Thêm nhóm quyền")]
        InsertGroup = 91,
        [Description("Cập nhật nhóm quyền")]
        UpdateGroup = 92,
        [Description("Xóa nhóm quyền")]
        DeleteGroup = 93,
        [Description("Hiệu chỉnh người dùng")]
        UpdateUserIntoGroup = 94,
        [Description("Cập nhật chi tiết phân quyền")]
        UpdateRightsIntoGroup = 95,

        //---------------------------------------------
        [Description("Xem tin tức")]
        ViewNews = 100,
        [Description("Thêm tin tức")]
        InsertNews = 101,
        [Description("Cập nhật tin tức")]
        UpdateNews = 102,
        [Description("Xóa tin tức")]
        DeleteNews = 103,

        [Description("Xem danh sách import thiết bị")]
        ViewImportDevice = 104,
        [Description("Lấy mẫy import thiết bị")]
        ExportImportDeviceSample = 105,
        [Description("Thêm import thiết bị")]
        InsertImportDevice = 106,
        [Description("Cập nhật thiết bị import")]
        UpdateImportDevice = 107,
        [Description("Xuất excel import thiết bị")]
        ExcelImportDevice = 108,
        //---------------------------------------------
        [Description("Xem báo cáo cơ sở - thiết bị")]
        ViewReportFacilityDevice = 110,
        [Description("Xuất báo cáo thời gian sử dụng dịch vụ")]
        ViewReportUseingServiceTime = 111,
        [Description("Xem báo cáo lịch sử cuộc gọi")]
        ViewReportHistoryAutoCall = 112,
        [Description("Xuất Excel trạng thái thiết bị")]
        ExportExcelReportHistoryAutoCall = 113,
        [Description("Xem báo cáo thống kê xác nhận tin thật giả từ app khách hàng")]
        ViewReportRealFake = 114,
        [Description("Xuất Excel trạng thái thiết bị")]
        ExportExcelReportRealFake = 115,
        [Description("Xem báo cáo thống kê xử lý tin thiết bị")]
        ViewReportProcessingDeviceStatus = 116,
        [Description("Xuất Excel trạng thái thiết bị")]
        ExportReportProcessingDeviceStatus = 117,
        [Description("Xem báo cáo lắp đặt thiết bị")]
        ViewReportInstallationDevice= 118,
        [Description("Xem Giám sát vận hành hệ thống")]
        ViewPCMonitoring = 119,
        [Description("Xem Giám sát thiết bị")]
        ViewDeviceMonitoring = 140,
        [Description("Xem lịch sử thiết bị")]
        ViewDeviceHistory = 141,
        [Description("Xuất Excel lịch sử thiết bị Heartbeat")]
        ExportDeviceHistoryHearbeat = 142,
        [Description("Xuất Excel thiết bị Alarm")]
        ExportDeviceHistoryAlarm = 143,
        [Description("Xem báo cáo quản lý khách hàng")]
        ViewReportCustomerManager = 144,
        [Description("Xuất Excel báo cáo quản lý khách hàng")]
        ExportCustomerManager= 145,

        //---------------------------------------------
        //---------------------------------------------

        //---------------------------------------------
        [Description("Xem lịch sử thao tác người dùng")]
        ViewHistoryAction = 120,
        [Description("Thêm tài liệu")]
        InsertHistoryAction = 121,
        [Description("Cập nhật tài liệu")]
        UpdateHistoryAction = 122,
        [Description("Xóa tài liệu")]
        DeleteHistoryAction = 123,

        //---------------------------------------------
        [Description("Xem nhân viên lắp đặt")]
        ViewInstallationStaff = 130,
        [Description("Thêm nhan viên lắp đặt")]
        InsertInstallationStaff = 131,
        [Description("Cập nhật nhân viên lắp đặt")]
        UpdateInstallationStaff = 132,
        [Description("Xóa nhân viên lắp đặt")]
        DeleteInstallationStaff = 133,
    }
}
