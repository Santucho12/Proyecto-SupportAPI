using AutoMapper;
using SupportApi.Models;
using SupportApi.DTOs;

namespace SupportApi.Mappings
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<UsuarioDto, Usuario>();
        }
    }
}
