using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BDayReminder.Domain;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace BDayReminder.Data
{
    public class BDayReminderRepository : IBDayReminderRepository
    {
        private const string _reliableBDayData = "BDayData";
        private readonly IReliableStateManager _stateManager;

        public BDayReminderRepository(IReliableStateManager stateManager)
        {
            this._stateManager = stateManager;
        }
        public async Task<bool> AddBDay(BDay bDay)
        {
            try
            {
                var bDayData = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, BDay>>(_reliableBDayData);

                using (var tx = _stateManager.CreateTransaction())
                {
                    //await bDayData.AddOrUpdateAsync(tx, bDay.BDayId, bDay, (id, value) => bDay);
                    await bDayData.AddAsync(tx, bDay.BDayId, bDay);

                    await tx.CommitAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                //TODO:Exception handling
                return false;
            }

        }

        public async  Task<IEnumerable<BDay>> GetAll()
        {
            var bDayData = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, BDay>>(_reliableBDayData);
            var result = new List<BDay>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var allBDayItems = await bDayData.CreateEnumerableAsync(tx, EnumerationMode.Ordered);

                using (var enumerator = allBDayItems.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, BDay> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<BDay>> GetAll(ushort month)
        {
            var bDayData = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, BDay>>(_reliableBDayData);
            var result = new List<BDay>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var allBDayItems = await bDayData.CreateEnumerableAsync(tx, EnumerationMode.Ordered);

                using (var enumerator = allBDayItems.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, BDay> current = enumerator.Current;

                        if (current.Value.BDayMonth == month)
                        {
                            result.Add(current.Value);

                        }
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<BDay>> GetAll(ushort month, ushort day)
        {
            var bDayData = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, BDay>>(_reliableBDayData);
            var result = new List<BDay>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var allBDayItems = await bDayData.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (var enumerator = allBDayItems.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, BDay> current = enumerator.Current;

                        if (current.Value.BDayMonth == month && current.Value.BDayDay == day)
                        {
                            result.Add(current.Value);

                        }
                    }
                }
            }

            return result;
        }

        public async Task<BDay> GetBDayDetails(Guid bDayId)
        {
            var bDayData = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, BDay>>(_reliableBDayData);
            var result = new List<BDay>();

            using (var tx = _stateManager.CreateTransaction())
            {
                ConditionalValue<BDay> bDay = await bDayData.TryGetValueAsync(tx, bDayId);

                return bDay.HasValue ? bDay.Value : null;
            }

        }
    }
}
