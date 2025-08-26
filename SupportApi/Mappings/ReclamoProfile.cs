using AutoMapper;
using SupportApi.Models;
using SupportApi.DTOs;

namespace SupportApi.Mappings
{
    public class ReclamoProfile : Profile
    {
        public ReclamoProfile()
        {
            CreateMap<Reclamo, ReclamoDto>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : null))
                .ForMember(dest => dest.Respuestas, opt => opt.MapFrom(src => src.Respuestas));

            CreateMap<Respuesta, RespuestaDto>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : null));
        }
    }
}
