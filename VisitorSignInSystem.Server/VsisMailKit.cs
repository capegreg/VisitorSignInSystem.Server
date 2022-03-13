using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Diagnostics;

namespace VisitorSignInSystem.Mail
{
    public class VsisMailKit
    {
		public void DoMail()
		{
			try
			{

				const string _SMTP = "xxxxxxxxx";
				const int _PORT = 25;

				var message = new MimeMessage();
				message.From.Add(new MailboxAddress("'MCPAO AerialMaker' <no-reply@xxxxxxxxx.com>", "no-reply@xxxxxxxxx.com"));
				message.To.Add(new MailboxAddress("Gregory Bologna", "xxxxxxxxx"));
				message.Subject = "test";

				message.Body = new TextPart("plain")
				{
					Text = @"mail message test"
				};

				using (var client = new SmtpClient())
				{
					client.Connect(_SMTP, _PORT, false);

					// Note: only needed if the SMTP server requires authentication
					//client.Authenticate("", "");

					client.Send(message);
					client.Disconnect(true);
				}


			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}
}
