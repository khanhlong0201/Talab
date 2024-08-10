using System.ComponentModel;
using System.Reflection;

namespace CORE_TALAB.Enum
{
    public enum EClickActionMobile
    {
        OPEN_NOTIFICATION,
        OPEN_FACILITY,
        OPEN_DEVICE,
        OPEN_WEB,
        CALL_MOBILE,
        OPEN_LINK,
        OPEN_DEFAULT
    }

    public enum EState
    {
        Delete = 0,
        Active = 1,
    }

    public enum EType
    {
        [Description("Thẻ bảo hành")]
        Warranty,
    }
}
