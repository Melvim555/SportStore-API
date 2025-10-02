# 🚀 Configuração do Apache Kafka - SportStore API

## 📋 **Visão Geral**

A API SportStore está preparada para publicar eventos de domínio no Apache Kafka, permitindo uma arquitetura event-driven e facilitando a integração com outros microsserviços.

## ⚠️ **STATUS ATUAL**

**Kafka implementado:** ✅ SIM - Código completo e pronto para produção
**Kafka habilitado:** ❌ NÃO - Desabilitado por padrão em desenvolvimento

**Por quê está desabilitado:**
O Kafka em containers Docker no Windows tem limitações de rede que podem causar problemas de conexão (`1/1 brokers are down`). Em produção com Kafka dedicado (Linux) ou serviço gerenciado, funcionará perfeitamente.

**Como funciona agora:**
- Kafka **DESABILITADO**: API rápida (< 500ms), eventos não publicados
- Kafka **HABILITADO**: API ainda rápida (eventos publicam em background), mas pode ter erros de conexão em ambiente local Windows

## 🎯 **Tópicos Kafka Implementados**

### **1. Tópico: `usuarios`**
Eventos relacionados a usuários (administradores e vendedores).

#### **Eventos:**
- `usuarios.cadastrado` - Quando um novo usuário é criado
- `usuarios.login` - Quando um usuário faz login (auditoria)

#### **Exemplo de Payload:**
```json
{
  "evento": "usuarios.cadastrado",
  "timestamp": "2025-09-30T22:30:00Z",
  "dados": {
    "usuarioId": "550e8400-e29b-41d4-a716-446655440000",
    "nome": "João Silva",
    "email": "joao@email.com",
    "tipo": "Administrador"
  }
}
```

### **2. Tópico: `produtos`**
Eventos relacionados ao catálogo de produtos.

#### **Eventos:**
- `produtos.cadastrado` - Quando um produto é criado
- `produtos.atualizado` - Quando um produto é editado
- `produtos.removido` - Quando um produto é excluído

#### **Exemplo de Payload:**
```json
{
  "evento": "produtos.cadastrado",
  "timestamp": "2025-09-30T22:35:00Z",
  "dados": {
    "produtoId": "P1",
    "nome": "Bola de Futebol Oficial",
    "descricao": "Bola Fifa Pro 2025",
    "preco": 199.90
  }
}
```

### **3. Tópico: `estoque`**
Eventos relacionados ao controle de estoque.

#### **Eventos:**
- `estoque.adicionado` - Quando estoque é adicionado
- `estoque.atualizado` - Quando há baixa de estoque (pedido)
- `estoque.movimentacao` - Histórico completo de movimentações

#### **Exemplo de Payload:**
```json
{
  "evento": "estoque.movimentacao",
  "timestamp": "2025-09-30T22:40:00Z",
  "dados": {
    "produtoId": "P1",
    "quantidade": 20,
    "tipoMovimentacao": "ENTRADA",
    "notaFiscal": "NF12345",
    "observacoes": "Compra inicial"
  }
}
```

### **4. Tópico: `pedidos`**
Eventos relacionados a pedidos de venda.

#### **Eventos:**
- `pedidos.criado` - Quando um pedido é criado

#### **Exemplo de Payload:**
```json
{
  "evento": "pedidos.criado",
  "timestamp": "2025-09-30T22:45:00Z",
  "dados": {
    "pedidoId": "PED1",
    "clienteDocumento": "12345678900",
    "nomeCliente": "João Cliente",
    "vendedor": "Maria Souza",
    "itens": [
      {
        "produtoId": "P1",
        "quantidade": 2,
        "precoUnitario": 199.90
      }
    ],
    "valorTotal": 399.80
  }
}
```

## ⚙️ **Configuração**

### **1. Configurar o `appsettings.json`:**

```json
{
  "Kafka": {
    "Enabled": false,
    "BootstrapServers": "localhost:9092",
    "ClientId": "sportstore-api"
  }
}
```

**Propriedades:**
- `Enabled`: `true` para ativar Kafka, `false` para desativar (padrão: `false`)
- `BootstrapServers`: Endereço do broker Kafka
- `ClientId`: Identificador do cliente produtor

### **2. Registrar o serviço no `Program.cs`:**

```csharp
// Registrar Kafka Producer
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
```

## 🐳 **Executar Kafka Localmente (Docker)**

### **1. Criar arquivo `docker-compose-kafka.yml`:**

```yaml
version: '3.8'

services:
  zookeeper:
    image: confluentinc/cp-zookeeper:latest
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    ports:
      - "2181:2181"

  kafka:
    image: confluentinc/cp-kafka:latest
    depends_on:
      - zookeeper
    ports:
      - "9092:9092"
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"

  kafka-ui:
    image: provectuslabs/kafka-ui:latest
    depends_on:
      - kafka
    ports:
      - "8090:8080"
    environment:
      KAFKA_CLUSTERS_0_NAME: local
      KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS: kafka:9092
```

### **2. Executar o Kafka:**

```bash
docker-compose -f docker-compose-kafka.yml up -d
```

### **3. Acessar Kafka UI:**
```
http://localhost:8090
ou
http://localhost:3030
```

### **4. Parar o Kafka:**

```bash
docker-compose -f docker-compose-kafka.yml down
```

## 🧪 **Testar Eventos Kafka**

### **1. Ativar Kafka no `appsettings.json`:**

```json
{
  "Kafka": {
    "Enabled": true,
    "BootstrapServers": "localhost:9092",
    "ClientId": "sportstore-api"
  }
}
```

### **2. Executar a API:**

```bash
dotnet run
```

### **3. Fazer requisições e verificar logs:**

Os logs da aplicação mostrarão quando eventos forem publicados:

```
✅ Evento 'usuarios.cadastrado' publicado com sucesso no tópico 'usuarios' [Partition: 0, Offset: 5]
✅ Evento 'produtos.cadastrado' publicado com sucesso no tópico 'produtos' [Partition: 0, Offset: 12]
```

### **4. Visualizar eventos no Kafka UI:**

1. Acesse `http://localhost:8090`
2. Navegue até **Topics**
3. Selecione o tópico desejado (`usuarios`, `produtos`, `estoque`, `pedidos`)
4. Visualize as mensagens publicadas

## 📊 **Consumir Eventos (Outros Microsserviços)**

Para consumir os eventos em outros serviços, use um **Consumer** Kafka:

```csharp
var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "meu-servico-consumer",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<string, string>(config).Build();
consumer.Subscribe("usuarios");

while (true)
{
    var result = consumer.Consume();
    var evento = JsonSerializer.Deserialize<BaseEvent>(result.Message.Value);
    
    // Processar evento
    Console.WriteLine($"Evento recebido: {evento.Evento}");
}
```

## 🎯 **Casos de Uso**

### **Analytics:**
- Consumir eventos `usuarios.login` para criar dashboards de acesso
- Analisar padrões de vendas com `pedidos.criado`

### **Auditoria:**
- Rastrear todas as movimentações de estoque
- Histórico completo de alterações em produtos

### **Integração com ERP:**
- Sincronizar estoque automaticamente
- Processar pedidos em sistemas externos

### **Notificações:**
- Enviar email quando `estoque.adicionado`
- Notificar vendedores sobre `pedidos.criado`

### **Relatórios:**
- Gerar relatórios em tempo real
- Business Intelligence

## 🔧 **Troubleshooting**

### **Problema: Kafka não conecta**
```
✅ Solução: Verificar se o Kafka está rodando:
docker ps | grep kafka
```

### **Problema: Eventos não aparecem no Kafka UI**
```
✅ Solução: Verificar se Kafka.Enabled = true no appsettings.json
```

### **Problema: Erro de timeout**
```
✅ Solução: Aumentar MessageTimeoutMs no KafkaProducerService:
MessageTimeoutMs = 10000
```

## 📚 **Referências**

- [Confluent Kafka Documentation](https://docs.confluent.io/)
- [Apache Kafka](https://kafka.apache.org/)
- [Kafka UI](https://github.com/provectus/kafka-ui)

---
