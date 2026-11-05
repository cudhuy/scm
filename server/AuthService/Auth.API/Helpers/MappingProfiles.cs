using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Messaging.KafkaMessage;
using AutoMapper;

namespace Auth.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserRegisterMessage>();
        }
    }
}
