using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Extension
{
    public class AutoMailClass : Controller
    {
        //MailMessage 全域
        MailMessage Msg;

        //SmtpClient 全域
        SmtpClient Sender;

        /// <summary>
        /// AutoMail 建構元
        /// </summary>
        public AutoMailClass()
        {
            //MailMessage 實體化
            Msg = new MailMessage();

            //SmtpClient 實體化
            Sender = new SmtpClient();

            //發件人地址與姓名
            Msg.From = new MailAddress("comeindustry@gmail.com", "ComeIndus Inc.", System.Text.Encoding.UTF8);

            //寄信者帳號密碼
            Sender.Credentials = new NetworkCredential("comeindustry@gmail.com", "comeindus8");

            //設定Smtp Server
            Sender.Host = "smtp.gmail.com";

            //設定Port
            Sender.Port = 25;

            //gmail預設開啟驗證
            Sender.EnableSsl = true; 
        }

        /// <summary>
        /// 註冊信
        /// </summary>
        /// <param name="RecipientAddress"></param>
        /// <param name="RecipientName"></param>
        /// <param name="VerifyCode"></param>
        /// <returns></returns>
        public bool RegisterVerify(string RecipientAddress, string RecipientName, string VerifyCode)
        {
            //宣告
            string mail_subject, mail_body;

            //信件標題
            mail_subject = "[ComeIndus]Please verify your account";

            //信件內容
            mail_body = string.Format(
                "<p>Hey {0}!</p>" +
                "<p>Welcome to join ComeIndus,</p>" +
                "<p>To complete the register, enter the verification code on the Website.</p>" +
                "<br>" +
                "<p>Verification Code: <b>{1}</b><p>" + 
                "<br>" +
                "<p>This letter is sent automatically by the system, please do not reply.</p>" +
                "<p>If you have any questions, or if you notice a problem, please let us know through <a href='https://comindus.azurewebsites.net/feedback'>https://comindus.azurewebsites.net/feedback</a>.</p>" +
                "<br>" +
                "<p>Thanks,</p>" +
                "<p>The ComeIndus Team</p>", 
                RecipientName, VerifyCode);

            //信件寄出
            if(AutoMailSend(RecipientAddress, mail_subject, mail_body))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <param name="RecipientAddress"></param>
        /// <param name="RecipientName"></param>
        /// <param name="VerifyCode"></param>
        /// <returns></returns>
        public bool ForgetPasswordSend(string RecipientAddress, string RecipientName, string VerifyCode)
        {
            //宣告
            string mail_subject, mail_body;

            //信件標題
            mail_subject = "[ComeIndus]Your new password";

            //信件內容
            mail_body = string.Format(
                "<p>Hey {0}!</p>" +
                "<p>This is your new password,</p>" +
                "<p>You can use this password sign in your account now.</p>" +
                "<br>" +
                "<p>New password: <b>{1}</b><p>" +
                "<br>" +
                "<p>This letter is sent automatically by the system, please do not reply.</p>" +
                "<p>If you have any questions, or if you notice a problem, please let us know through <a href='https://comindus.azurewebsites.net/feedback'>https://comindus.azurewebsites.net/feedback</a>.</p>" +
                "<br>" +
                "<p>Thanks,</p>" +
                "<p>The ComeIndus Team</p>",
                RecipientName, VerifyCode);

            //信件寄出
            if (AutoMailSend(RecipientAddress, mail_subject, mail_body))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 信件寄出
        /// </summary>
        /// <param name="Recipient"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <param name="Priority"></param>
        public bool AutoMailSend(string Recipient, string Title, string Content, MailPriority Priority = MailPriority.Normal)
        {
            try
            {
                //收件者信箱
                Msg.To.Add(Recipient);

                //郵件標題
                Msg.Subject = Title;

                //郵件標題編碼
                Msg.SubjectEncoding = System.Text.Encoding.UTF8;

                //郵件內容
                Msg.Body = Content;
                //郵件內容編碼 
                Msg.BodyEncoding = System.Text.Encoding.UTF8;

                //是否是HTML郵件
                Msg.IsBodyHtml = true;

                //郵件優先等級 
                Msg.Priority = Priority;

                //寄出信件
                Sender.Send(Msg); 

                //結束TCP
                Sender.Dispose();

                //結束TCP
                Msg.Dispose();

                //成功
                return true;
            }
            catch (Exception ex)
            {
                //失敗
                return false;
            }
        }
    }
}
