using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Infrastructure.Settings;

namespace TeamTasker.Infrastructure.Services
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="isHtml">Whether the body is HTML</param>
        /// <returns>Task</returns>
        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            // In a real application, this would send an actual email
            // For now, we'll just log the email details
            _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
            _logger.LogDebug("Email body: {Body}", body);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send a password reset email
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="resetToken">Password reset token</param>
        /// <param name="callbackUrl">Callback URL for password reset</param>
        /// <returns>Task</returns>
        public Task SendPasswordResetEmailAsync(string to, string resetToken, string callbackUrl)
        {
            var resetUrl = $"{callbackUrl}?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(to)}";
            
            var subject = "Reset Your Password";
            var body = $@"
                <h1>Reset Your Password</h1>
                <p>Please reset your password by clicking the link below:</p>
                <p><a href='{resetUrl}'>Reset Password</a></p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            ";
            
            return SendEmailAsync(to, subject, body);
        }
    }
}
