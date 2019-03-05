using BDayReminder.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDayReminder.Data
{
    public interface IBDayReminderRepository
    {
        Task<IEnumerable<BDay>> GetAll();

        Task<IEnumerable<BDay>> GetAll(ushort month);
        Task<IEnumerable<BDay>> GetAll(ushort month, ushort day);


        Task<BDay> GetBDayDetails(Guid bDayId);

        Task<bool> AddBDay(BDay bDay);

    }
}
