using System;
using System.Threading;
using VisitorSignInSystem.Server.Models;

/// <summary>
/// Manages visitor wait time. Sends email to manager when predefined time is exceeded
/// TODO: Look into adding Dbcontext DI when available to manage other email operations
/// </summary>
namespace VisitorSignInSystem.Server.BLL
{
    public class VisitorWaitingTimer
    {        
        public Timer WaitTimer { get; set; }
        public Visitor Visitor{ get; set; }
        public int MaxWait { get; set; }
        public string MailTo { get; set; }

        public VisitorWaitingTimer()
        {
        }

        /// <summary>
        /// StartVisitorTimer
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="cat_descr"></param>
        /// <param name="maxWait"></param>
        /// <param name="mailTo"></param>
        public void StartVisitorTimer(Visitor visitor, string cat_descr, int maxWait, string mailTo)
        {
            // inherits from Visitor, adds mailto and max wait properties
            Notifier nfy = new Notifier(visitor, mailTo, maxWait, cat_descr);

            // create the object
            var visitorNotServed = new VisitorNotServed(nfy);

            // convert minutes to milliseconds
            int ms = maxWait * 60000;

            var autoEvent = new AutoResetEvent(false);

            WaitTimer = new Timer(visitorNotServed.WaitExceeded, autoEvent, ms, Timeout.Infinite);
        }
    }

    class VisitorNotServed
    {
        public Notifier visitor { get; set; }

        public VisitorNotServed(Notifier v)
        {
            visitor = v;
        }

        public void WaitExceeded(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            // release thread
            autoEvent.Set();
            // send an email, this can be anything
            SendManagerMail();
        }

        private void SendManagerMail()
        {
            VsisMail mail = new VsisMail(
                "no-reply@manateepao.com", 
                "'MCPAO VSIS' <no-reply@manateepao.com>", 
                visitor.MailTo, 
                visitor.MailSubj, 
                visitor.MailTitle, 
                visitor.MailBody
            );
            mail.SendTheMail();
        }
    }

    class Notifier : Visitor
    {
        public string MailFrom { get; set; }
        public string MailDisplayName { get; set; }
        public string MailTo { get; set; }
        public string MailSubj { get; set; }
        public string MailTitle { get; set; }
        public string MailBody { get; set; }
        public int MaxWaitTime { get; set; }

        public Notifier(Visitor v, string mailto, int maxWait, string cat_descr)
        {
            // build the email

            var header = $"<tr><td valign=\"top\" style=\"border: 1px solid #0A4370;margin:5px;padding:10px;color:#D01110;text-align:center;\"><h2>A visitor is waiting longer than usual.</h2></td></tr>";

            var body = "<table cellpadding=\"0\" cellspacing=\"1\" border=\"0\" style=\"font-size:16px;color:#0A4370;width:90%;\">";
            body += header;
            body += $"<tr><td valign=\"top\" style=\"margin: 6px;padding: 10px;\"><h3><li>{v.FirstName} {v.LastName} signed in for {cat_descr} and is waiting longer than usual.</li></h3></td></tr>";
            body += $"<tr><td valign=\"top\" ><br /><br /><br /><p style=\"font-size:16px;margin: 6px;padding: 10px;\">MCPAO - Visitor Sign In System</p></td></tr></table>";

            AssignedCounter = v.AssignedCounter;
            CalledTime = v.CalledTime;
            Created = v.Created;
            FirstName = v.FirstName;
            LastName = v.LastName;
            Location = v.Location;
            VisitCategoryId = v.VisitCategoryId;
            MailTo = mailto;
            MailSubj = "VSIS - Visitor Waiting Alert";
            MailTitle = "";
            MailBody = body;
            MaxWaitTime = maxWait;
        }
    }
}
