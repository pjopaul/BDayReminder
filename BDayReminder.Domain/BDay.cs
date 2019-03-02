using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BDayReminder.Domain
{
    public class BDay
    {
        public BDay()
        {
            BDayId = Guid.NewGuid();
        }
        public Guid BDayId { get; set; }

        public string PersonName { get; set; }

        public ushort?  BDayYear { get; set; }

        [Range(1,12,ErrorMessage = "Month value should be in between 1 and 12")]
        public ushort BDayMonth { get; set; }

        [Range(1, 31, ErrorMessage = "Day value should be in between 1 and 31")]
        public ushort BDayDay { get; set; }

    }
}
