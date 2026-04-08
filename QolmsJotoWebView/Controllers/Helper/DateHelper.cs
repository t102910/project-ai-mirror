using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class DateHelper
    {
        public static int GetAge(DateTime birthday, DateTime oneDay) 
        {
            int result = int.MinValue;

            if (birthday != DateTime.MinValue
                && oneDay != DateTime.MinValue
                && oneDay >= birthday)
            {
                int age = ((oneDay.Year * 10000 + oneDay.Month * 100 + oneDay.Day)
                         - (birthday.Year * 10000 + birthday.Month * 100 + birthday.Day)) / 10000;

                if (age >= byte.MinValue && age <= byte.MaxValue)
                {
                    result = age;
                }
            }

            return result;
        }
    }
}