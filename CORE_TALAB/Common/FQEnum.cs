using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CORE_TALAB.Common.EventEnums
{
    public enum CAMERA_STATUS_ENUM
    {
        CONNECTING = 0,
        CONNECTED,
        DISCONNECTED,
        VIDEO_LOST,
        CONTROL_CONNECTED,
        VIDEO_CONNECTED,
        BUFFERING,
    }

    public enum RESULT_ENUM
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
        RS_INVALID_DATA_INPUT,
        RS_BUFFRING,
        RS_EXCEPTION,
        RS_RESTART,
        RS_UPDATE,
        RS_NOT_FOUND,
        RS_HASH_VERIFY_FAILED,
        RS_REGISTER_FACE_IMAGE_FAILED
    }
}
