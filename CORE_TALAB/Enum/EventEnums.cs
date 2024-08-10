using System;
using System.ComponentModel;

namespace CORE_TALAB.EventEnums
{
    public enum TypeOfEvent
    {
        /// <summary>
        /// return if fake event
        /// </summary>
        Fake = 0,
        /// <summary>
        /// return if fake event
        /// </summary>
        Real = 1,
    }

    public enum EventStatus
    {
        /// <summary>
        /// New event
        /// </summary>
        New = 0,
        /// <summary>
        /// New event
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Viewed but have not handled yet
        /// </summary>
        Processing = 2,
        /// <summary>
        /// Handled
        /// </summary>
        Finished = 3
    }

    public enum EventRate
    {
        Bad = 0,
        Good = 1,
        Normal = 2
    }
    public enum EventLevel
    {
        
        Normal = 1,
        Careful,
        Dangerous,
        VeryDangerous
        // Cấp 0: mất kết nối
        // Cấp 1: thông thường
        // o Cấp 2: chú ý
        // o Cấp 3: nguy hiểm
        // o Cấp 4: rất nguy hiểm
    }
    public enum TypeOfSignal
    {
        [Description("Tin cháy")]
        FIRE,
        [Description("Tin nút nhấn khẩn cấp")]
        FIRE_PRESS_BUTTON,
        [Description("Tin sos")]
        FIRE_SOS,
        [Description("Mất kết nối")]
        CONNECT_LOST,
        [Description("Có kết nối")]
        CONNECT_WORKING,
        [Description("Báo lỗi tủ trung tâm")]
        ERROR_CABINET,
        [Description("Đứt dây kết nối kênh 1")]
        ERROR_CHANNEL1,
        [Description("Đứt dây kết nối kênh 2")]
        ERROR_CHANNEL2,
        [Description("Đứt dây kết nối kênh 3")]
        ERROR_CHANNEL3,
    }

    public enum ProcessType
    {
        ManeuveringForces = 1,              // Điều động lực lượng
        ContactSeniorLeadership,            // Liên lạc lãnh đạo cấp cao
        ReceiveSeniorLeadershipDirection,   // Nhận chỉ đạo lãnh đạo cấp cao
        HandlingCommonSituations,           // Xử lý tình huống thông thường
        Finish                              // Kết thúc sự kiện
    }

    public enum EventSource
    {
        FromVMS = 0,       //Event được thêm từ VMS(Camera, G2)
        FromAppCBCS = 1,   //Event được thêm từ App mobile CBCS
        FromTTCH = 2       //Event được thêm từ trung tâm chỉ huy C3
    }
}