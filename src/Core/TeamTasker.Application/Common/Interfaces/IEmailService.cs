using System.Threading.Tasks;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for email service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="isHtml">Whether the body is HTML</param>
        /// <returns>Task</returns>
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        
        /// <summary>
        /// Send a password reset email
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="resetToken">Password reset token</param>
        /// <param name="callbackUrl">Callback URL for password reset</param>
        /// <returns>Task</returns>
        Task SendPasswordResetEmailAsync(string to, string resetToken, string callbackUrl);
    }
}
