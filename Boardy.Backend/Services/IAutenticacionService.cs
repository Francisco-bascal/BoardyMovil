using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IAutenticacionService
    {
        Task<ResultadoOperacion<SesionDto>> RegistrarAsync(RegistrarUsuarioDto dto);

        Task<ResultadoOperacion<SesionDto>> IniciarSesionAsync(IniciarSesionDto dto);
    }
}
