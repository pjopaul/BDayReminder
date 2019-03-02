using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDayReminder.Domain
{
    public interface IBDayDataService : IService
    {
        Task<IEnumerable<BDay>> GetAll();

        Task<IEnumerable<BDay>> GetAllByMonth(ushort month);

        Task<IEnumerable<BDay>> GetAllByMonthAndDay(ushort month, ushort day);

        Task<BDay> GetBDayDetails(Guid bDayId);

        Task AddBDay(BDay bDay);
    }
}
