using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using RestSharp;

namespace MiniDropbox.Web.Clases{

    class EnvioEmail
    {
        //MailMessage msg;

        RestClient client = new RestClient();
        RestRequest request = new RestRequest();

        public EnvioEmail()
        {
          //  msg = new MailMessage();
            client.BaseUrl = "https://api.mailgun.net/v2";
            client.Authenticator = new HttpBasicAuthenticator("api", "key-0jxgq19v3jg2t9rwk9cp28yydqolmf38");
        }

        public void Limpiar()
        {
            request.Parameters.Clear();
            //msg.To.Clear();
            //msg.CC.Clear();
            //msg.Attachments.Clear();
        }

        public void EnviarA(string Correo){
            request.AddParameter("to", Correo);
            //msg.To.Add(new MailAddress(Correo));
        }            

        public void Subject(string Subjet){
            //msg.Subject=Subjet;
            request.AddParameter("subject", Subjet);
        }

        public void Body(string body){
            //msg.IsBodyHtml = true;  
            //msg.Body=body;
            request.AddParameter("text",body);
        }

        //public void Attachment(string FileName){
        //    //msg.Attachments.Add(new Attachment(FileName));
        //}

        public bool Enviar(){
            //msg.From = new MailAddress(ConfigurationManager.AppSettings["Login"].ToString());
            //SmtpClient clienteSmtp = new SmtpClient(ConfigurationManager.AppSettings["Hostname"].ToString());
            //clienteSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Login"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            //try
            //{
            //    clienteSmtp.Send(msg);
                
            //}
            //catch (Exception ex)
            //{//MessageBox.Show(ex.Message);
            //    return false;
            //}
            try
            {
                request.AddParameter("domain", "apazminidropbox.mailgun.org", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "postmaster@apazminidropbox.mailgun.org");
                request.Method = Method.POST;
                client.Execute(request);
            }
            catch (Exception ee)
            {
                return false;
            }

            return true;
        }          
    }
}




