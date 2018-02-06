using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskMgrTypes;
using TaskMgrTypes.Constants;
using MailKit.Net.Smtp;
using MimeKit;

namespace TaskMgrConsole
{
    // as outlook was giving client thread limit exceed error, created quartz schedule to look in email queue periodically and email one at a time
    // adjust schedule frequency (config key "EmailIntervalInSeconds")
    public class EmailExecution : IJob
    {
        private static List<Tuple<string, string, string, string>> EmailQueue = new List<Tuple<string, string, string, string>>();
        private static string Password;
        public EmailExecution()
        {
        }

        public static void SetPassword(string password)
        {
            Password = password;
        }
        public static void AddEmailQueue(string toAddress, string toAddressTitle, string subject, string body)
        {
            // add email to queue
            EmailQueue.Add(new Tuple<string, string, string, string>(toAddress, toAddressTitle, subject, body));

        }


        public Task Execute(IJobExecutionContext jonExecutionContext)
        {
            if (EmailQueue.Count > 0)
            {
                var firstItem = EmailQueue[0];
                try
                {
                    
                    EmailQueue.RemoveAt(0);

                    string SmtpServer = Program.Configuration[ConfigKey.SmtpServer];
                    int SmtpPortNumber = int.Parse(Program.Configuration[ConfigKey.SmtpPortNumber]);
                    string FromEmailId = Program.Configuration[ConfigKey.FromEmailId];
                    string FromEmailTitle = Program.Configuration[ConfigKey.FromEmailTitle];

                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, SmtpPortNumber, false);
                        client.Authenticate(FromEmailId, Password);

                        var mimeMessage = new MimeMessage();
                        mimeMessage.From.Add(new MailboxAddress(FromEmailTitle, FromEmailId));
                        mimeMessage.To.Add(new MailboxAddress(firstItem.Item2, firstItem.Item1));
                        mimeMessage.Subject = firstItem.Item3;
                        mimeMessage.Body = new TextPart("plain")
                        {
                            Text = firstItem.Item4
                        };

                        client.Send(mimeMessage);
                    }
                }
                catch(Exception ex)
                {
                    Program.LogException(new ExceptionInfo { Message = "Error sending email Subject :" + firstItem.Item3 + " . Message : " + ex.Message });
                }
            }

            return Task.FromResult(0);
        }
    }
}
