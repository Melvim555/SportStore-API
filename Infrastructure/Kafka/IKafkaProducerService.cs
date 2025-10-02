using SportStore.Application.Events;

namespace SportStore.Infrastructure.Kafka;

/// <summary>
/// Interface para o serviço de producer Kafka
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    /// Publica um evento no Kafka
    /// </summary>
    /// <param name="topico">Nome do tópico Kafka</param>
    /// <param name="evento">Evento a ser publicado</param>
    /// <returns>Task</returns>
    Task PublicarEventoAsync(string topico, BaseEvent evento);
} 