using Microsoft.EntityFrameworkCore;
using SportStore.Application.DTOs;
using SportStore.Application.Interfaces;
using SportStore.Application.Events;
using SportStore.Domain;
using SportStore.Infrastructure;
using SportStore.Infrastructure.Kafka;

namespace SportStore.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly SportStoreContext _context;
    private readonly IJwtService _jwtService;
    private readonly IKafkaProducerService _kafkaProducer;

    public UsuarioService(SportStoreContext context, IJwtService jwtService, IKafkaProducerService kafkaProducer)
    {
        _context = context;
        _jwtService = jwtService;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<UsuarioResponseDTO> CriarUsuarioAsync(UsuarioCreateDTO usuarioDto)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Normalizar email
        var emailNormalizado = usuarioDto.Email.Trim().ToLowerInvariant();

        // Validações adicionais
        if (string.IsNullOrWhiteSpace(emailNormalizado))
            throw new InvalidOperationException("Email não pode ser vazio.");

        if (!IsValidAsciiEmail(emailNormalizado))
            throw new InvalidOperationException("Email deve conter apenas caracteres ASCII (sem acentos, cedilhas, etc.) e ter formato válido (exemplo@dominio.com).");

        if (emailNormalizado.Length < 5)
            throw new InvalidOperationException("Email deve ter pelo menos 5 caracteres.");

        stopwatch.Stop();
        Console.WriteLine($"⏱️ Validações iniciais: {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        if (await EmailExisteAsync(emailNormalizado))
            throw new InvalidOperationException("Email já está em uso.");

        stopwatch.Stop();
        Console.WriteLine($"⏱️ Verificação de email duplicado: {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);
        stopwatch.Stop();
        Console.WriteLine($"⏱️ Hash da senha (BCrypt): {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        var usuario = new Usuario
        {
            Nome = usuarioDto.Nome.Trim(),
            Email = emailNormalizado,
            Senha = senhaHash,
            TipoUsuario = usuarioDto.TipoUsuario,
            DataCriacao = DateTime.UtcNow,
            Ativo = true,
            Uuid = Guid.NewGuid()
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        stopwatch.Stop();
        Console.WriteLine($"⏱️ Salvar no banco de dados: {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        // Publicar evento Kafka
        var tipoUsuarioTexto = usuario.TipoUsuario == TipoUsuario.Administrador ? "Administrador" : "Vendedor";
        var evento = new UsuarioCadastradoEvent(
            usuario.Uuid.ToString(),
            usuario.Nome,
            usuario.Email,
            tipoUsuarioTexto
        );
        await _kafkaProducer.PublicarEventoAsync("usuarios", evento);

        stopwatch.Stop();
        Console.WriteLine($"⏱️ Publicar evento Kafka: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"✅ TEMPO TOTAL: {stopwatch.Elapsed.TotalMilliseconds}ms\n");

        return new UsuarioResponseDTO
        {
            Id = usuario.Id,
            Uuid = usuario.Uuid,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            DataCriacao = usuario.DataCriacao,
            Ativo = usuario.Ativo
        };
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto)
    {
        // Normalizar email para login
        var emailNormalizado = loginDto.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado && u.Ativo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.Senha))
            throw new UnauthorizedAccessException("Email ou senha inválidos.");

        var usuarioResponse = new UsuarioResponseDTO
        {
            Id = usuario.Id,
            Uuid = usuario.Uuid,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            DataCriacao = usuario.DataCriacao,
            Ativo = usuario.Ativo
        };

        var token = _jwtService.GerarToken(usuarioResponse);

        // Publicar evento Kafka de login
        var tipoUsuarioTexto = usuario.TipoUsuario == TipoUsuario.Administrador ? "Administrador" : "Vendedor";
        var eventoLogin = new UsuarioLoginEvent(
            usuario.Uuid.ToString(),
            usuario.Email,
            tipoUsuarioTexto,
            true
        );
        await _kafkaProducer.PublicarEventoAsync("usuarios", eventoLogin);

        return new LoginResponseDTO
        {
            Token = token,
            Usuario = usuarioResponse
        };
    }

    public async Task<UsuarioResponseDTO?> ObterUsuarioPorIdAsync(int id)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id && u.Ativo);

        if (usuario == null)
            return null;

        return new UsuarioResponseDTO
        {
            Id = usuario.Id,
            Uuid = usuario.Uuid,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            DataCriacao = usuario.DataCriacao,
            Ativo = usuario.Ativo
        };
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _context.Usuarios.AnyAsync(u => u.Email == emailNormalizado);
    }

    /// <summary>
    /// Valida se o email contém apenas caracteres ASCII e tem formato válido
    /// </summary>
    /// <param name="email">Email a ser validado</param>
    /// <returns>True se o email é válido e ASCII-only</returns>
    private bool IsValidAsciiEmail(string email)
    {
        // Regex para email ASCII-only: ^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$
        var asciiEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Verificar se contém apenas caracteres ASCII (0-127)
        if (email.Any(c => c > 127))
            return false;

        // Verificar formato com regex
        return System.Text.RegularExpressions.Regex.IsMatch(email, asciiEmailPattern);
    }
}
