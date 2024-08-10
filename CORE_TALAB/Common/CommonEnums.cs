namespace CORE_TALAB.Common
{
    public class CommonEnums
    {
        public enum RetCode
        {
            /// <summary>
            /// return if success
            /// </summary>
            Success = 0,

            /// <summary>
            /// Use for login. Incorrect username or password.
            /// </summary>
            IncorrectUserPassword,

            /// <summary>
            /// return if fail
            /// </summary>
            Fail,

            /// <summary>
            /// return if a method throw a excetion
            /// </summary>
            SystemError,

            /// <summary>
            /// Account has not loged in
            /// </summary>
            NotLogin,
            /// <summary>
            /// 
            /// </summary>
            InvalidDateTime,
            /// <summary>
            /// return if not found data 
            /// </summary>
            NoExistedData,

            /// <summary>
            /// return if data is existing
            /// </summary>
            ExistedData,

            /// <summary>
            /// return if accountId not existed in OTS DB
            /// </summary>
            ErrorAccount,

            /// <summary>
            /// Incorrect login password
            /// </summary>
            IncorrectPassword,

            /// <summary>
            /// Old password is not match with new password
            /// </summary>
            PasswordNotMatch,

            /// <summary>
            /// Password is empty
            /// </summary>
            PasswordEmpty,

            /// <summary>
            /// Password is inactived
            /// </summary>
            PasswordInactived,

            /// <summary>
            /// Account is inactive
            /// </summary>
            AccountInactive,

            /// <summary>
            /// Send warning SMS because of login failed many times.
            /// </summary>
            SendWarningSms,

            /// <summary>
            /// Show captchar 
            /// </summary>
            ShowCaptcha,

            /// <summary>
            /// Account is locked
            /// </summary>
            AccountLocked,

            /// <summary>
            /// 
            /// </summary>
            IsValid,

            /// <summary>
            /// Permission to do something
            /// </summary>
            NotAllow,

            /// <summary>
            /// Incorrect pin
            /// </summary>
            IncorrectPin,

            ErrorEmpty,

            ErrorMinLength,

            ErrorMaxLength,

            IncorectInformation,

            CanNotDelete,
            IncorrectIme,
            ErrorUpdateIme,

        }

        public enum ACTION_TYPE
        {
            CREATE = 0,
            EDIT,
            DELETE,
            LOGIN,
            LOGOUT,
            CHANGE_PASSWORD,
            ACCOUNT_LOCKED,
            ACCOUNT_ACTIVED,
            IMPORT,
            CLEAR_BANNED,
            BANNED,
            EDIT_HISTORY,
            FINISH_EVENT,
            INSERT_HANDLING_EVENT,
            EDIT_HANDLING_EVENT,
            SEND_EVENT,
        }

    }
}