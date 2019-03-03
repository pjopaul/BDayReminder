using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BDayReminder.API.ViewModels;
using BDayReminder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace BDayReminder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BDayController : ControllerBase
    {
        private readonly StatelessServiceContext serviceContext;

        public BDayController(StatelessServiceContext context)
        {
           
            this.serviceContext = context;
           
        }

        private IBDayDataService GetBDayDataService(int monthKey = 0)
        {

            long key = GetPartitionKey(monthKey);

            return ServiceProxy.Create<IBDayDataService>(
                                new Uri("fabric:/BDayReminder/BDayReminder.Data")
                                , new ServicePartitionKey(monthKey));
            
        }


        // GET 
        [HttpGet]
        public async Task<IEnumerable<BDayDetails>> Get()
        {

            IEnumerable<BDay> allBDays = await GetBDayDataService(DateTime.Now.Month).GetAll();


            return allBDays.Select(b => new BDayDetails
            {

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

            var bDayItem = await GetBDayDataService(0).GetBDayDetails(bDayItemId);


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


            await GetBDayDataService(bDayDetails.BDayMonth).AddBDay(newBDayItem);
        }




        [HttpGet("{month:int}/{day:int?}")]
        public async Task<IEnumerable<BDayDetails>> Get(int month, int? day = null)
        {
            IEnumerable<BDay> allBDays = new List<BDay>();

            if (day.HasValue)
            {
                allBDays = await GetBDayDataService(month).GetAllByMonthAndDay((ushort)month, (ushort)day.Value);
            }
            else
            {
                allBDays = await GetBDayDataService(month).GetAllByMonth((ushort)month);
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




        private long GetPartitionKey(int month)
        {
            //Partition logic
            return month;

        }

    }
}
