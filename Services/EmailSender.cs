using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

public sealed class EmailSender :
    IEmailSender,                          // non-generic (used by your controllers)
    IEmailSender<ApplicationUser>          // generic (required by MapIdentityApi<TUser>())
{
    // ===== Low-level method you’ll call from your controllers =====
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // TODO: implement SMTP / SendGrid / MailKit
        return Task.CompletedTask;
    }

    // ===== Methods Identity’s minimal APIs expect at startup =====
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        => SendEmailAsync(email, "Confirm your email",
            $"<p>Thanks for registering!</p><p><a href=\"{confirmationLink}\">Confirm account</a></p>");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        => SendEmailAsync(email, "Reset your password",
            $"<p>Reset your password:</p><p><a href=\"{resetLink}\">Reset password</a></p>");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        => SendEmailAsync(email, "Your reset code",
            $"<p>Code: <strong>{resetCode}</strong></p>");
}
