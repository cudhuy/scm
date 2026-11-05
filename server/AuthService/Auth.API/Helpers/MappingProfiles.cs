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
            // MaxDepth per map: mitigation CVE-2026-32933 (NU1903) — AutoMapper 14 dropped the global config
            CreateMap<User, UserDto>().MaxDepth(64);
            CreateMap<User, UserRegisterMessage>().MaxDepth(64);
        }
    }
}
