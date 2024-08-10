using System.ComponentModel;

namespace CORE_TALAB.Enum
{
    public enum REPONSE_ENUM
    {
        RS_NOT_OK = -1,
        RS_OK,
        RS_FAILED_NOT_LOGIN,
        RS_CONTINUE,
        RS_STOP,
        RS_END,
        RS_AU_NOT_OK,
        RS_VI_NOT_OK,
        RS_EQUAL,
        RS_GREATER,
        RS_LESS,
        RS_IGNORE,
        [Description("Lỗi dữ liệu đầu vào")]
        RS_INVALID_DATA_INPUT,
        RS_BUFFRING,
        RS_EXCEPTION,
        RS_RESTART,
        RS_UPDATE,
        RS_NOT_FOUND,
        RS_HASH_VERIFY_FAILED,
        RS_REGISTER_FACE_IMAGE_FAILED,
        RS_NOT_AVAILABLE=-2,
        RS_NO_SIGNAL=-3,
        RS_IN_USE=-4,
        RS_NO_CHANGE=-5,

    }
}