using SportStore.Application.DTOs;

namespace SportStore.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioResponseDTO> CriarUsuarioAsync(UsuarioCreateDTO usuarioDto);
    Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
    Task<UsuarioResponseDTO?> ObterUsuarioPorIdAsync(int id);
    Task<bool> EmailExisteAsync(string email);
}



