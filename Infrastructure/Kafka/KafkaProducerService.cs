using Confluent.Kafka;
using SportStore.Application.Events;
using System.Text.Json;

namespace SportStore.Infrastructure.Kafka;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string>? _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly bool _kafkaEnabled;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        _kafkaEnabled = configuration.GetValue<bool>("Kafka:Enabled");

        if (_kafkaEnabled)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = configuration["Kafka:BootstrapServers"],
                    ClientId = configuration["Kafka:ClientId"] ?? "sportstore-api",
                    Acks = Acks.All,
                    MessageTimeoutMs = 30000,
                    EnableIdempotence = true,
                    RequestTimeoutMs = 30000,
                    SocketTimeoutMs = 60000,
                    RetryBackoffMs = 1000
                };

                _producer = new ProducerBuilder<string, string>(config).Build();
                
                // Aguardar um pouco para o Kafka estar pronto
                Thread.Sleep(2000);
                
                _logger.LogInformation("✅ Kafka Producer inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar Kafka. Kafka desabilitado.");
                _kafkaEnabled = false;
                _producer = null;
            }
        }
        else
        {
            _logger.LogWarning("Kafka desabilitado.");
            _producer = null;
        }
    }

    public async Task PublicarEventoAsync(string topico, BaseEvent evento)
    {
        if (!_kafkaEnabled || _producer == null)
        {
            _logger.LogInformation($"[KAFKA OFF] Evento '{evento.Evento}' ignorado");
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                var json = JsonSerializer.Serialize(evento, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                var message = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json,
                    Timestamp = new Timestamp(evento.Timestamp)
                };

                var result = await _producer.ProduceAsync(topico, message);
                _logger.LogInformation($"OK Kafka: {evento.Evento} -> {topico} [Offset: {result.Offset}]");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro Kafka: {evento.Evento} - {ex.Message}");
            }
        });

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}

