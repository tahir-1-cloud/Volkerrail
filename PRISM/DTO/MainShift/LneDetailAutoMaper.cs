using AutoMapper;
using PRISM.Models;

namespace PRISM.DTO.MainShift
{
    public class LneDetailAutoMaper : Profile
    {
    
        public LneDetailAutoMaper()
        {
            CreateMap<Lnedetail, Lnedetail>();
        }

    }
}
