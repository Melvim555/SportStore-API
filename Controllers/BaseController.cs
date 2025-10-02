using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;

namespace SportStore.Controllers;

[ApiController]
[Authorize]
public abstract class BaseController : ControllerBase
{
    protected UsuarioResponseDTO ObterUsuarioLogado()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
        var nameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
        var tipoUsuarioClaim = User.FindFirst("TipoUsuario");

        if (userIdClaim == null || emailClaim == null || nameClaim == null || tipoUsuarioClaim == null)
            throw new UnauthorizedAccessException("Token inválido.");

        return new UsuarioResponseDTO
        {
            Id = int.Parse(userIdClaim.Value),
            Nome = nameClaim.Value,
            Email = emailClaim.Value,
            TipoUsuario = (Domain.TipoUsuario)int.Parse(tipoUsuarioClaim.Value)
        };
    }

    protected bool IsAdmin()
    {
        var usuario = ObterUsuarioLogado();
        return usuario.TipoUsuario == Domain.TipoUsuario.Administrador;
    }

    protected bool IsVendedor()
    {
        var usuario = ObterUsuarioLogado();
        return usuario.TipoUsuario == Domain.TipoUsuario.Vendedor;
    }
}



