namespace SportStore.Application.Events;

/// <summary>
/// Classe base para todos os eventos de domínio
/// </summary>
public abstract class BaseEvent
{
    public string Evento { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public object Dados { get; set; } = null!;
} 