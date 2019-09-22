using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    class FeedbackModel
    {
        public string Key { get; set; }
        public string UserUID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public bool Status { get; set; }
    }
}
