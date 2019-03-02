using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDayReminder.API.ViewModels
{
    public class BDayDetails
    {
        [JsonProperty("bDayId")]
        public Guid BDayId { get; set; }

        [JsonProperty("personName")]
        public string PersonName { get; set; }

        [JsonProperty("bDayYear")]
        public ushort? BDayYear { get; set; }

        [JsonProperty("bDayMonth")]
        [Range(1, 12, ErrorMessage = "Month value should be in between 1 and 12")]
        public ushort BDayMonth { get; set; }

        [JsonProperty("bDayDay")]
        [Range(1, 31, ErrorMessage = "Day value should be in between 1 and 31")]
        public ushort BDayDay { get; set; }

    }
}
