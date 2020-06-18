using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Talk, TalksModel>().ReverseMap();
            CreateMap<Speaker, SpeakerModel>();
            CreateMap<Camp, CampModel>()
                .ForMember(m => m.Talks, t => t.MapFrom(c => c.Talks)).ReverseMap();
        }
    }
}
