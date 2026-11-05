using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class OrderEnum
    {
        public enum EStatus
        {
            Pending = 0,
            Processing = 1,
            Completed = 2,
            Canceled = 3,
        //    Processed = 4,// offline
            Delivering = 5,//online 
        }
        public enum EOrderType
        {
            Online = 0,
            Offline = 1
        }
        public enum EPaymentMethod
        {
            Banking = 1,
            Cash = 2,          
        }
    }
}
