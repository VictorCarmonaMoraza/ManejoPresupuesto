using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Mappeador
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>().ReverseMap();
        }
    }
}
