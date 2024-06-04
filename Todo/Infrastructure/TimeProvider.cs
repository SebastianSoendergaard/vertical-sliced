using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Infrastructure
{
    internal class TimeProvider : ITimeProvider
    {
        public DateOnly GetCurrentDate()
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
