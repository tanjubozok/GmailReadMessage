using System;
using System.Collections.Generic;

//start - firstMethod
using EAGetMail;
//end - firstMethod

//start - secondMethod
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
//end - secondMethod

namespace GmailReadMessage
{
    static class Program
    {
        static void Main(string[] args)
        {
            FirstReadMessage();
            SecondReadMessage();
        }

        private static void FirstReadMessage()
        {
            MailServer oServer = new MailServer("imap.gmail.com", "abcdefg@gmail.com", "Password", ServerProtocol.Imap4);
            MailClient oClient = new MailClient("TryIt");

            oServer.SSLConnection = true;
            oServer.Port = 993;
            //oServer.SSLConnection = false;
            //oServer.Port = 143;
            oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
            oClient.Connect(oServer);

            MailInfo[] infos = oClient.GetMailInfos();
            for (int i = infos.Length - 1; i > 0; i--)
            {
                MailInfo info = infos[i];
                Mail oMail = oClient.GetMail(info);
                Console.WriteLine(oMail.TextBody);
            }
        }

        private static void SecondReadMessage()
        {
            using (Imap imap = new Imap())
            {
                imap.Connect("imap.gmail.com", 993, true);
                imap.UseBestLogin("abcdefg@gmail.com", "Password");

                imap.SelectInbox();
                List<long> uids = imap.Search(Flag.Unseen);

                foreach (long uid in uids)
                {
                    var eml = imap.GetMessageByUID(uid);
                    IMail email = new MailBuilder().CreateFromEml(eml);
                    Console.WriteLine(email.Subject);// Subject
                    foreach (MailBox m in email.From) // From
                    {
                        Console.WriteLine(m.Address);
                        Console.WriteLine(m.Name);
                    }
                    Console.WriteLine(email.Date);// Date
                    Console.WriteLine(email.Text);// Text body of the message
                    Console.WriteLine(email.Html);// Html body of the message
                    Console.WriteLine(email.Document.Root.Headers["x-spam"]);// Custom header
                    foreach (MimeData mime in email.Attachments)// Save all attachments to disk
                    {
                        mime.Save(@"c:\" + mime.SafeFileName);
                    }
                }
                imap.Close();
            }
        }
    }
}
