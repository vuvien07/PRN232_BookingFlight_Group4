using System.Net.Mail;
using System.Net;
using BookingFlightServer.Utils;
using MimeKit;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
    public interface IEmailService
    {
        Task<bool> SendForgotPasswordEmailAsync(string toEmail, string resetToken);
        Task<bool> SendEmailVerificationAsync(string toEmail, string verificationToken);
		Task<bool> IsEmailExistsAsync(string email);
		Task TestSendMailAsync(string email);
        Task<bool> SendTicketCodeByEmailAsync(List<string> ticketCodes,FlightCheckoutRequestDTO flightCheckoutRequest);
	}

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<bool> SendForgotPasswordEmailAsync(string toEmail, string resetToken)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var fromEmail = smtpSettings["FromEmail"];
                var fromPassword = smtpSettings["FromPassword"];
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"]);

                var resetLink = $"http://localhost:5001/ForgotPassword/Reset?token={resetToken}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "BookingFlight Support"),
                    Subject = "Reset Your Password - BookingFlight",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Reset Your Password</h2>
                            <p>You have requested to reset your password for BookingFlight.</p>
                            <p>Click the link below to reset your password:</p>
                            <p><a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                            <p>This link will expire in 15 minutes.</p>
                            <p>If you did not request this, please ignore this email.</p>
                            <br>
                            <p>Best regards,<br>BookingFlight Team</p>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(string toEmail, string verificationToken)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var fromEmail = smtpSettings["FromEmail"];
                var fromPassword = smtpSettings["FromPassword"];
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"]);

                var verificationLink = $"http://localhost:5001/Register/VerifyEmail?token={verificationToken}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "BookingFlight Support"),
                    Subject = "Verify Your Email Address - BookingFlight",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Welcome to BookingFlight!</h2>
                            <p>Thank you for registering with BookingFlight.</p>
                            <p>To complete your registration, please verify your email address by clicking the link below:</p>
                            <p><a href='{verificationLink}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email Address</a></p>
                            <p>This link will expire in 24 hours.</p>
                            <p>If you did not create this account, please ignore this email.</p>
                            <br>
                            <p>Best regards,<br>BookingFlight Team</p>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Verification email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send verification email: {ex.Message}");
                return false;
            }
        }
        private string GetEmailBody(MimeMessage mimeMessage)
        {
            if (!string.IsNullOrEmpty(mimeMessage.TextBody))
            {
				return mimeMessage.TextBody;
            }
            else if (!string.IsNullOrEmpty(mimeMessage.HtmlBody))
            {
                return mimeMessage.HtmlBody;
            }
            return string.Empty;
        }

		public async Task<bool> IsEmailExistsAsync(string email)
		{
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var fromEmail = smtpSettings["FromEmail"];
                var fromPassword = smtpSettings["FromPassword"];
                using var client = new ImapClient();
                await client.ConnectAsync("imap.gmail.com", 993, true);
				await client.AuthenticateAsync(fromEmail, fromPassword);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var today = DateTime.Now.Date;
                var tommorrow = today.AddDays(1);
				var messages = await inbox.SearchAsync(SearchQuery.DeliveredAfter(today).And(SearchQuery.DeliveredBefore(tommorrow)));
                foreach (var message in messages)
                {
                    var mimeMessage = await inbox.GetMessageAsync(message);
                    string body = GetEmailBody(mimeMessage);
					if ((body.Contains("your message wasn't delivered to", StringComparison.OrdinalIgnoreCase) &&
				body.Contains(email, StringComparison.OrdinalIgnoreCase)) || (body.Contains("tin nhắn của bạn không được gửi đến", StringComparison.OrdinalIgnoreCase) &&
				body.Contains(email, StringComparison.OrdinalIgnoreCase)) || (body.Contains("thư của bạn không được gửi đến", StringComparison.OrdinalIgnoreCase) &&
				body.Contains(email, StringComparison.OrdinalIgnoreCase)))
					{
						return false; // Email không tồn tại, bị trả về
					}
				}
            }
			catch (Exception ex)
            {
				Console.WriteLine($"Failed to send verification email: {ex.Message}");
                return false;
            }
            return true;
		}

		public async Task TestSendMailAsync(string email)
		{
			try
			{
				var smtpSettings = _configuration.GetSection("SmtpSettings");
				var fromEmail = smtpSettings["FromEmail"];
				var fromPassword = smtpSettings["FromPassword"];
				var smtpHost = smtpSettings["Host"];
				var smtpPort = int.Parse(smtpSettings["Port"]);
				var mailMessage = new MailMessage
				{
					From = new MailAddress(fromEmail, "BookingFlight Support"),
					Subject = "Test send mail - BookingFlight",
					Body = "Test send mail",
				};

				mailMessage.To.Add(email);

				using var smtpClient = new SmtpClient(smtpHost, smtpPort)
				{
					Credentials = new NetworkCredential(fromEmail, fromPassword),
					EnableSsl = true
				};

				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send verification email: {ex.Message}");
			}
		}

		public async Task<bool> SendTicketCodeByEmailAsync(List<string> ticketCodes, FlightCheckoutRequestDTO flightCheckoutRequest)
		{
			try
			{
				var smtpSettings = _configuration.GetSection("SmtpSettings");
				var fromEmail = smtpSettings["FromEmail"];
				var fromPassword = smtpSettings["FromPassword"];
				var smtpHost = smtpSettings["Host"];
				var smtpPort = int.Parse(smtpSettings["Port"]);
                var body = $@"
                        <div style=""font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9; margin: 0 auto;"">
                    <h1 style=""color: #333; text-align: center;""> Thông Tin Đặt Vé</h1>
                    <hr style=""border: none; height: 1px; background-color: #ccc;"">
                    <p style=""font-size: 16px; color: #555;"">Kính gửi quý khách {flightCheckoutRequest.FullNameContact}  </p>
                        <p style=""font-size: 16px; color: #555;"">
                            Dưới đây là thông tin các mã vé quý khách đã đặt:
                        </p>
<div style=""background-color: #fff; padding: 15px; border-radius: 5px; box-shadow: 0px 2px 5px rgba(0,0,0,0.1);"">
                        <ul style=""list-style: none; padding: 0;"">
                    ";

                for(int i = 0; i < ticketCodes.Count; i++)
                {
                    body += $@"<li style=\""font-size: 16px; color: #333; padding: 5px 0;\"">Mã đặt chỗ {i}: {ticketCodes[i]}</li>";
                }
                body += $@" </ul>
                    </div>
                    <p style=""font-size: 14px; color: #777; margin-top: 20px; text-align: center;"">
                        Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi.
                    </p>
                </div>";

				var mailMessage = new MailMessage
				{
					From = new MailAddress(fromEmail, "BookingFlight Support"),
					Subject = "Your ticket - BookingFlight",
					Body = body,
					IsBodyHtml = true
				};

				mailMessage.To.Add(flightCheckoutRequest.EmailContact);

				using var smtpClient = new SmtpClient(smtpHost, smtpPort)
				{
					Credentials = new NetworkCredential(fromEmail, fromPassword),
					EnableSsl = true
				};

				await smtpClient.SendMailAsync(mailMessage);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send verification email: {ex.Message}");
				return false;
			}
		}
	}
}
