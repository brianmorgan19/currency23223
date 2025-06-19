using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.Models
{
    public class CurrencyRate
    {
        public DateTime Date { get; set; }
        public double USD { get; set; }
        public double EUR { get; set; }
        public double CNY { get; set; }
    }
}
