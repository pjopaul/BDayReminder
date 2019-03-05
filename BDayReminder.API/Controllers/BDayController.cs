using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BDayReminder.API.ViewModels;
using BDayReminder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        private readonly LinkGenerator linkGenerator;
        private readonly IMapper mapper;

        public BDayController(StatelessServiceContext context, LinkGenerator linkGenerator, IMapper mapper)
        {

            this.serviceContext = context;
            this.linkGenerator = linkGenerator;
            this.mapper = mapper;
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
        public async Task<ActionResult<BDayDetails>> Get()
        {

            IEnumerable<BDay> allBDays = await GetBDayDataService(DateTime.Now.Month).GetAll();


            return Ok(mapper.Map<BDayDetails[]>(allBDays));
        }


        [HttpGet("{bDayItemId}")]
        [ProducesResponseType(typeof(BDayDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid bDayItemId)
        {

            var bDayItem = await GetBDayDataService(0).GetBDayDetails(bDayItemId);


            if (bDayItem != null)
            {
                return Ok(mapper.Map<BDayDetails>(bDayItem));

            }
            return NotFound();


        }


        [HttpPost]
        public async Task<ActionResult<BDayDetails>> Post(BDayDetails bDayDetails)
        {



            var newBDayItem = mapper.Map<BDay>(bDayDetails);


            var location = linkGenerator.GetPathByAction("Get", "BDay",
                                                        new { month = bDayDetails.BDayMonth, day = bDayDetails.BDayDay });

            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Invalid BDayDetails data");
            }


            if (await GetBDayDataService(bDayDetails.BDayMonth).AddBDay(newBDayItem))
            {
                return Created(location, mapper.Map<BDayDetails>(newBDayItem));
            }

            return this.StatusCode(StatusCodes.Status500InternalServerError, "Error while saving the data");
        }




        [HttpGet("{month:int}/{day:int?}")]
        public async Task<ActionResult<BDayDetails>> Get(int month, int? day = null)
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

            return Ok(mapper.Map<BDayDetails[]>(allBDays));
        }



        /// <summary>
        /// Get the aprtition key
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        private long GetPartitionKey(int month)
        {
            //Partition logic
            return month;

        }

    }
}
