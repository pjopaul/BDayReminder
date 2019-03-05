using AutoMapper;
using BDayReminder.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDayReminder.API.ViewModels
{
    public class BDayMapperProfile : Profile
    {
        public BDayMapperProfile()
        {

            CreateMap<BDayDetails, BDay>()
                          
                .ReverseMap();
        }
    }
}
