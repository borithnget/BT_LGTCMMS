using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class EmailTemplate
    {
        public static string template_1 = @"Dear Sir/Madam, <br/><br/>"
                                        + "This is request for content template. <br/>"
                                        + "Thanks, <br/>"

            ;
        public static string template_2 = @"Dear Sir/Madam, <br/><br/>"
                                        + "Request {req_no} is waiting for approval. <br/>"
                                        + "<ul>"
                                        + "<li>Request : {req_no} <a href='{link}'>Link</a></li>"
                                        + "<li>Requestor name : {name}</li>"
                                        + "</ul><br/><br/>"
                                        + "Thanks, <br/>"

            ;
    }
}