using AutoMapper;
using CompanyIntranet.DTOs;
using CompanyIntranet.Models;

namespace CompanyIntranet.Mappings
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Announcement, AnnouncementDto>().ReverseMap();
        }
    }
}
