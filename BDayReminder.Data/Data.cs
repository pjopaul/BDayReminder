using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BDayReminder.Domain;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BDayReminder.Data
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Data : StatefulService, IBDayDataService
    {
        private IBDayReminderRepository _repo;

        public Data(StatefulServiceContext context)
            : base(context)
        { }

        #region IBDayDataService Members
        public async Task<bool> AddBDay(BDay bDay)
        {
            return await _repo.AddBDay(bDay);
        }

        public async Task<IEnumerable<BDay>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<IEnumerable<BDay>> GetAllByMonth(ushort month)
        {
            return await _repo.GetAll(month);
        }

        public async Task<IEnumerable<BDay>> GetAllByMonthAndDay(ushort month, ushort day)
        {
            return await _repo.GetAll(month,day);
        }

        public async Task<BDay> GetBDayDetails(Guid bDayId)
        {
            return await _repo.GetBDayDetails(bDayId);
        }

        #endregion
        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
         //   return new[]
         //{
         //          new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context))
         //      };


            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            #region Default Code
            //// TODO: Replace the following sample code with your own logic 
            ////       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //} 
            #endregion

            _repo = new BDayReminderRepository(this.StateManager);



            var bInitialData = new BDay[]
            {
                new BDay{ BDayDay=12, BDayMonth=10, BDayYear =1980, PersonName = "Test A"},
                new BDay{ BDayDay=27, BDayMonth=2, BDayYear =1979, PersonName = "Test B"},
                new BDay{ BDayDay=2, BDayMonth=4, BDayYear =1980, PersonName = "Test C"},
                new BDay{ BDayDay=24, BDayMonth=2, BDayYear =1981, PersonName = "Test D"},
            };


            foreach (var item in bInitialData)
            {
                await _repo.AddBDay(item);
            }

           //IEnumerable<BDay> all = await _repo.GetAll();


        }
    }
}
