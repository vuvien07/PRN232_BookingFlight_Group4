using System.Net.Mail;
using System.Net;

namespace BookingFlightServer.Services
{
    public interface IEmailService
    {
        Task<bool> SendForgotPasswordEmailAsync(string toEmail, string resetToken);
        Task<bool> SendEmailVerificationAsync(string toEmail, string verificationToken);
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
    }
}
