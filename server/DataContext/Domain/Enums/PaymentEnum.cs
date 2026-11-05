using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class PaymentEnum
    {
        public enum EMethod
        {
            Pending = 0,
            Confirmed = 1,
            Failed = 3
        }
    }
}
