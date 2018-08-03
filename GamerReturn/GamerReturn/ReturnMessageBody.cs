using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GamerReturn
{
    public class ReturnMessageBody
    {
        private string status;

        private string msg;

        private object data;

        public string Status { get => status; set => status = value; }
        public string Msg { get => msg; set => msg = value; }
        public object Data { get => data; set => data = value; }
    }
}