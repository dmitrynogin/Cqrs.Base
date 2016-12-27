using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Base
{
    public abstract class Event
    {
        public Exception Error { get; protected set; }
        public TimeSpan Taken { get; protected set; }
    }    
}
