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
        public BDayDetails()
        {
            BDayId = Guid.NewGuid();
        }
        [JsonProperty("bDayId")]
        public Guid BDayId { get; set; }

        [JsonProperty("personName")]
        [Required(ErrorMessage = "personName is required")]
        public string PersonName { get; set; }

        [JsonProperty("bDayYear")]
        public ushort? BDayYear { get; set; }

        [JsonProperty("bDayMonth")]
        //[Required(ErrorMessage = "bDayMonth is required")]
        [Range(1, 12, ErrorMessage = "Month value should be in between 1 and 12")]
        public ushort? BDayMonth { get; set; }

        [JsonProperty("bDayDay")]
        [Required(ErrorMessage = "bDayDay is required")]
        [Range(1, 31, ErrorMessage = "Day value should be in between 1 and 31")]
        public ushort BDayDay { get; set; }

    }
}
