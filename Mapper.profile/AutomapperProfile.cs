using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Dtos.RequestsInfo;


namespace Skipperu.MappingProfile
{
   
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<RequestDBinputDTO, RequestDBstore>().ForMember(x => x.Host, options => options.MapFrom(x => x.UserTargetedHost))
            .ForMember(x => x.HeaderJSON, options => options.MapFrom(x => x.UserHeaderKVP))
            .ForMember(x => x.BodyJSON, options => options.MapFrom(x => x.UserBodyKVP))
            .ForMember(x => x.Endpoint, options => options.MapFrom(x => x.UserEndpoint))
            .ForMember(x => x.QueryParametersJSON, options => options.MapFrom(x => x.UserQueryKVP));
        }
    }
}
