using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Models
{
    /// <summary>
    /// 帳號MODEL
    /// </summary>
    public class AccountModels : SetResult
    {
        //帳號
        public string Account { get; set; }

        //密碼
        public string Password { get; set; }

        //再次確認密碼
        public string PasswordCheck { get; set; }
    }

    /// <summary>
    /// 會員MODEL
    /// </summary>
    public class Member : AccountModels
    {
        //使用者名稱
        public string Username { get; set; }

        //使用者真實姓名
        public string Nickname { get; set; }

        //性別
        public string Gender { get; set; }

        //生日
        public DateTime Birthday { get; set; }

        //大頭照路徑
        public string PhotoURL { get; set; }

        //註冊驗證判別
        public string MailCheck { get; set; }

        //更改密碼判別
        public string PwdChangeCheck { get; set; }

        //帳號啟用時間
        public DateTime AccountStart { get; set; }

        //帳號停用時間
        public DateTime AccountEnd { get; set; }

        //系統權限
        public string Authority { get; set; }
    }

    /// <summary>
    /// 信箱驗證碼 MODEL
    /// </summary>
    public class Verify : SetResult
    {
        public Verify()
        {
            //預設 尚未驗證完畢
            Verified = false;
        }

        public string Account { get; set; }

        public string Username { get; set; }

        public string VerificationCode { get; set; }

        public bool Verified { get; set; }
    }

    /// <summary>
    /// 執行成功與否 檢測用Model
    /// </summary>
    public class SetResult
    {
        public SetResult()
        {
            //預設
            ok = true;
            ResultMessage = "";
        }

        //是否成功
        public bool ok { get; set; }

        //結果訊息
        public string ResultMessage { get; set; }
    }
}
