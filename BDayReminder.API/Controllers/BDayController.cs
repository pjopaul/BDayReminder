using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDayReminder.API.ViewModels;
using BDayReminder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace BDayReminder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BDayController : ControllerBase
    {
        private readonly IBDayDataService _bDayDataService;

        public BDayController()
        {
            this._bDayDataService = ServiceProxy.Create<IBDayDataService>(
                                    new Uri("fabric:/BDayReminder/BDayReminder.Data")
                                    , new ServicePartitionKey(0));
        }
        // GET 
        [HttpGet]
        public async Task<IEnumerable<BDayDetails>> Get()
        {
            // return new [] { new BDayDetails() { BDayId = Guid.NewGuid(), PersonName = "From API", BDayDay = 26, BDayMonth = 2, BDayYear = 1986 } };

            IEnumerable<BDay> allBDays = await _bDayDataService.GetAll();


            return allBDays.Select(b => new BDayDetails {

                BDayId = b.BDayId,
                PersonName = b.PersonName,
                BDayYear = b.BDayYear,
                BDayMonth = b.BDayMonth,
                BDayDay = b.BDayDay

             
            });
        }


        [HttpGet("{bDayItemId}")]
        [ProducesResponseType(typeof(BDayDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid bDayItemId)
        {
            // return new [] { new BDayDetails() { BDayId = Guid.NewGuid(), PersonName = "From API", BDayDay = 26, BDayMonth = 2, BDayYear = 1986 } };

            var bDayItem = await _bDayDataService.GetBDayDetails(bDayItemId);


            if (bDayItem != null)
            {
                return Ok(new BDayDetails
                {

                    BDayId = bDayItem.BDayId,
                    PersonName = bDayItem.PersonName,
                    BDayYear = bDayItem.BDayYear,
                    BDayMonth = bDayItem.BDayMonth,
                    BDayDay = bDayItem.BDayDay


                });

            }
            return NotFound();

            
        }


        [HttpPost]
        public async Task Post([FromBody] BDayDetails bDayDetails)
        {
            var newBDayItem = new BDay()
            {
               
                PersonName = bDayDetails.PersonName,
                BDayDay = bDayDetails.BDayDay,
                BDayMonth = bDayDetails.BDayMonth,
                BDayYear = bDayDetails.BDayYear
            };


            await _bDayDataService.AddBDay(newBDayItem);
        }

        


        [HttpGet("{month:int}/{day:int?}")]
        public async Task<IEnumerable<BDayDetails>> Get(int month, int? day = null)
        {
            IEnumerable<BDay> allBDays = new List<BDay>();

            if (day.HasValue)
            {
                allBDays = await _bDayDataService.GetAllByMonthAndDay((ushort) month, (ushort) day.Value);
            }
            else
            {
                allBDays = await _bDayDataService.GetAllByMonth((ushort)month);
            }

            


            return allBDays.Select(b => new BDayDetails
            {

                BDayId = b.BDayId,
                PersonName = b.PersonName,
                BDayYear = b.BDayYear,
                BDayMonth = b.BDayMonth,
                BDayDay = b.BDayDay


            });
        }
    }
}
