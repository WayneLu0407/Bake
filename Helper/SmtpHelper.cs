using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bake.Helper
{
    public class SmtpHelper
    {
        private readonly string _host = "smtp.gmail.com";
        private readonly int _port = 587;
        private readonly string _userName = "SweetStack01@gmail.com";
        private readonly string _password = "ofzo zpqo ldvl ifkw"; // 建議使用應用程式專用密碼

        /// <summary>
        /// 寄送電子郵件 (非同步)
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            using (MailMessage message = new MailMessage())
            {
                // 設定發件者與收件者
                message.From = new MailAddress(_userName, "系統通知");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;

                using (SmtpClient client = new SmtpClient(_host, _port))
                {
                    client.Credentials = new NetworkCredential(_userName, _password);
                    client.EnableSsl = true; // 現代郵件伺服器幾乎都要求 SSL/TLS

                    try
                    {
                        // 使用非同步傳送，避免卡住呼叫端
                        await client.SendMailAsync(message);
                    }
                    catch (Exception ex)
                    {
                        // 這裡可以實作 Log 紀錄
                        throw new Exception($"郵件寄送失敗: {ex.Message}");
                    }
                }
            }
        }
    }
}
