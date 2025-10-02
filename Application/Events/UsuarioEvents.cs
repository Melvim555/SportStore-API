namespace SportStore.Application.Events;

/// <summary>
/// Evento: Usuário Cadastrado
/// </summary>
public class UsuarioCadastradoEvent : BaseEvent
{
    public UsuarioCadastradoEvent(string usuarioId, string nome, string email, string tipo)
    {
        Evento = "usuarios.cadastrado";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            usuarioId,
            nome,
            email,
            tipo
        };
    }
}

/// <summary>
/// Evento: Usuário Fez Login
/// </summary>
public class UsuarioLoginEvent : BaseEvent
{
    public UsuarioLoginEvent(string usuarioId, string email, string tipo, bool sucesso)
    {
        Evento = "usuarios.login";
        Timestamp = DateTime.UtcNow;
        Dados = new
        {
            usuarioId,
            email,
            tipo,
            sucesso
        };
    }
} 