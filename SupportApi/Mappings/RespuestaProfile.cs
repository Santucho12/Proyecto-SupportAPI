using AutoMapper;
using SupportApi.Models;
using SupportApi.DTOs;

namespace SupportApi.Mappings
{
    public class RespuestaProfile : Profile
    {
        public RespuestaProfile()
        {
            CreateMap<Respuesta, RespuestaDto>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : null));
            CreateMap<RespuestaDto, Respuesta>();
        }
    }
}
