using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.DTO
{
    public class SessionInfo
    {
        public String Guid { get; set; }
        public Int32 TimeoutSec { get; set; }
        public String Message { get; set; }
    }
}