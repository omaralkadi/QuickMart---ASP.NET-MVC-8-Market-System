using System.Net.Mail;
using System.Net;
using DAL.Entities;

namespace QuickMart.Helpers
{
    public static class SendEmail
    {
        public static async Task send(Email email)
        {
            try
            {
                // Set up the SMTP client
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Your SMTP server address
                    Port = 587, // SMTP server port
                    EnableSsl = true, // Enable SSL
                    Credentials = new NetworkCredential("omaralkadi111@gmail.com", "emsyvikagixextcr")
                };
                // Send the email
                smtpClient.Send("omaralkadi111@gmail.com", email.Recipient, email.Subject, email.Body);
            }
            catch (Exception ex)
            {
                // Handle any errors here
                Console.WriteLine("Error sending email: " + ex.Message);
                // Optionally log the exception or handle it as required
            }
        }
    }
}
