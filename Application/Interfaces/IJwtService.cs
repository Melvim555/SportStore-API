using SportStore.Application.DTOs;

namespace SportStore.Application.Interfaces;

public interface IJwtService
{
    string GerarToken(UsuarioResponseDTO usuario);
    UsuarioResponseDTO? ValidarToken(string token);
}



