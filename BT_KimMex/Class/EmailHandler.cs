//using Microsoft.Analytics.Interfaces;
//using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using System.Configuration;
using Eco.Ocl.Support;
using System.Web.Services.Description;
using BT_KimMex.Models;

namespace BT_KimMex.Class
{
    public class EmailHandler
    {
        public static string email_sender = ConfigurationManager.AppSettings["EmailSender"];
        public static string email_pwd = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ConfigurationManager.AppSettings["EmailPWD"]));
        public SmtpClient InitSmtp()
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(email_sender, email_pwd),
                EnableSsl = true,
                UseDefaultCredentials = true
            };
            return client;
        }

        public string GenerateEmailList(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (list != null)
                {
                    foreach (string s in list)
                    {
                        sb.Append(s);
                        if (s != list.LastOrDefault())
                        {
                            sb.Append(", ");
                        }
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {

            }

            return sb.ToString();
        }

        public string SendEmail(List<string> email_to, string subject, int template_no, List<string> email_cc = null)
        {
            string result = "";
            try
            {
                
                MailMessage message = new MailMessage();
                // Set up the SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(email_sender, email_pwd);
                smtp.EnableSsl = true;
                var to = GenerateEmailList(email_to);
                var cc = "";

                if (email_cc != null)
                {
                    cc = GenerateEmailList(email_cc);
                    message.CC.Add(cc);
                }
                MailAddress fromAddress = new MailAddress(email_sender);
                message.From = fromAddress;
                message.To.Add(to);
                message.Subject = subject;
                message.Body = generateEmailBody(template_no);
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                try
                {
                    // Send the email
                    smtp.Send(message);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                result = "Sent";

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string generateEmailBody(int template_no=1, string linkUrl = "",string req_no="",string requestor_name = "")

        { 
            string result = string.Empty;
            if(template_no == 1)
            {

                result = EmailTemplate.template_1;
            }
            else if (template_no == 2)
            {

                result = EmailTemplate.template_2;
                result.Replace("{req_no}", req_no).Replace("{link}", linkUrl).Replace("{name}", requestor_name);
            }

            return result;
        }

    }
}