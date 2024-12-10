using MailKit.Net.Smtp;
using MimeKit;



namespace Chess.Services
{
    public static class EmailService
    {
        private static readonly string _smtpServer = "smtp.gmail.com";
        private static readonly int _port = 587;
        private static readonly string _username = "mariamwageeh71@gmail.com";
        private static readonly string _password = "cbulkfdgcrhyzncc";

        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", _username));
            message.To.Add(new MailboxAddress("Recipient", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(message);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
