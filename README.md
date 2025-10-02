# 🏪 SportStore API

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

API RESTful completa para gerenciamento de equipamentos esportivos desenvolvida em **.NET 8** com **Clean Architecture**, **Domain-Driven Design (DDD)** e **Event-Driven Architecture**.

## 🎯 **Sobre o Projeto**

Sistema completo de CRUD para loja de equipamentos esportivos com funcionalidades avançadas de gestão, autenticação segura e controle de estoque em tempo real.

### ✨ **Principais Características**

- ✅ **Autenticação JWT** com controle de permissões
- ✅ **Controle de permissões** baseado em roles (Admin/Vendedor)
- ✅ **Gestão completa de produtos** com validações robustas
- ✅ **Controle de estoque** com histórico de movimentações
- ✅ **Emissão de pedidos** com validação automática de estoque
- ✅ **Event-Driven Architecture** com Kafka
- ✅ **Validações de segurança** e integridade de dados
- ✅ **Documentação automática** com Swagger/OpenAPI
- ✅ **Docker support** para deploy simplificado

## 📋 **Funcionalidades Implementadas**

### 🔐 **Sistema de Autenticação**
- Cadastro de usuários (Administrador/Vendedor)
- Login com JWT Token
- Controle de permissões por endpoint
- Senhas criptografadas com BCrypt

### 📦 **Gestão de Produtos** (Somente Admin)
- CRUD completo de produtos
- Validação de nomes únicos
- Controle de preços com Value Objects
- Ativação/desativação de produtos

### 📊 **Controle de Estoque** (Somente Admin)
- Adição de produtos ao estoque
- Histórico completo de movimentações
- Verificação de quantidade disponível
- Controle por nota fiscal

### 🛒 **Sistema de Pedidos** (Somente Vendedor)
- Criação de pedidos com validação de estoque
- Baixa automática no estoque
- Controle de cliente por CPF
- Histórico de pedidos por vendedor

## 🏗️ **Arquitetura**

### **Clean Architecture + DDD**
```
├── Domain/              # Entidades e regras de negócio
│   ├── Usuario.cs       # Agregado de usuário
│   ├── Produto.cs       # Agregado de produto
│   ├── Estoque.cs       # Agregado de estoque
│   ├── Pedido.cs        # Agregado de pedido
│   └── ValueObjects/    # Objetos de valor (Email, Preco, etc.)
├── Application/         # Camada de aplicação
│   ├── DTOs/           # Data Transfer Objects
│   ├── Interfaces/     # Contratos dos serviços
│   ├── Services/       # Implementação dos serviços
│   └── Events/         # Eventos de domínio
├── Infrastructure/     # Camada de infraestrutura
│   ├── SportStoreContext.cs
│   └── Kafka/          # Integração com Kafka
└── Controllers/        # API Controllers
```

### **Tecnologias Utilizadas**
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados
- **JWT Bearer** - Autenticação
- **BCrypt** - Criptografia de senhas
- **Kafka** - Event streaming
- **Swagger/OpenAPI** - Documentação
- **Docker** - Containerização

## 🚀 **Como Executar**

### **Pré-requisitos**
- .NET 8.0 SDK
- Docker (opcional)

### **Executando Localmente**

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd SportStore
   ```

2. **Restaure as dependências**
   ```bash
   cd SportStore
   dotnet restore
   ```

3. **Execute a aplicação**
   ```bash
   dotnet run
   ```

4. **Acesse a aplicação**
   - API: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

### **Executando com Docker**

1. **Build da imagem**
   ```bash
   docker build -t sportstore-api .
   ```

2. **Execute o container**
   ```bash
   docker run -p 8080:8080 sportstore-api
   ```

3. **Acesse a aplicação**
   - API: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080/swagger`

## 📚 **Documentação da API**

### **Endpoints Disponíveis**

#### 🔐 **Autenticação**
| Método | Endpoint | Descrição | Permissão |
|--------|----------|-----------|-----------|
| `POST` | `/api/usuarios` | Cadastrar usuário | Público |
| `POST` | `/api/usuarios/login` | Realizar login | Público |

#### 👤 **Usuários**
| Método | Endpoint | Descrição | Permissão |
|--------|----------|-----------|-----------|
| `GET` | `/api/usuarios/me` | Obter dados do usuário logado | Autenticado |

#### 📦 **Produtos** (Somente Admin)
| Método | Endpoint | Descrição | Permissão |
|--------|----------|-----------|-----------|
| `GET` | `/api/produtos` | Listar produtos | Admin |
| `GET` | `/api/produtos/{id}` | Obter produto por ID | Admin |
| `POST` | `/api/produtos` | Criar produto | Admin |
| `PUT` | `/api/produtos/{id}` | Atualizar produto | Admin |
| `DELETE` | `/api/produtos/{id}` | Excluir produto | Admin |

#### 📊 **Estoque** (Somente Admin)
| Método | Endpoint | Descrição | Permissão |
|--------|----------|-----------|-----------|
| `POST` | `/api/estoque/adicionar` | Adicionar produtos ao estoque | Admin |
| `GET` | `/api/estoque/historico` | Obter histórico de movimentações | Admin |
| `GET` | `/api/estoque/disponivel/{produtoId}` | Verificar quantidade disponível | Admin |

#### 🛒 **Pedidos** (Somente Vendedor)
| Método | Endpoint | Descrição | Permissão |
|--------|----------|-----------|-----------|
| `POST` | `/api/pedidos` | Criar pedido | Vendedor |
| `GET` | `/api/pedidos` | Listar pedidos | Vendedor |
| `GET` | `/api/pedidos/{id}` | Obter pedido por ID | Vendedor |

## 🧪 **Exemplos de Uso**

### **1. Cadastrar Administrador**
```json
POST /api/usuarios
{
  "nome": "João Admin",
  "email": "joao@admin.com",
  "senha": "123456",
  "tipoUsuario": 1
}
```

### **2. Login**
```json
POST /api/usuarios/login
{
  "email": "joao@admin.com",
  "senha": "123456"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "id": 1,
    "nome": "João Admin",
    "email": "joao@admin.com",
    "tipoUsuario": 1
  }
}
```

### **3. Criar Produto** (com token JWT)
```json
POST /api/produtos
Authorization: Bearer <token>
{
  "nome": "Bola de Futebol Oficial",
  "descricao": "Bola de futebol oficial FIFA",
  "preco": 89.90
}
```

### **4. Adicionar Estoque**
```json
POST /api/estoque/adicionar
Authorization: Bearer <token>
{
  "produtoId": 1,
  "quantidade": 50,
  "notaFiscal": "NF001",
  "observacoes": "Compra inicial"
}
```

### **5. Criar Pedido** (com token de vendedor)
```json
POST /api/pedidos
Authorization: Bearer <token>
{
  "documentoCliente": "12345678901",
  "nomeCliente": "Cliente Exemplo",
  "itens": [
    {
      "produtoId": 1,
      "quantidade": 2
    }
  ]
}
```

## 🔧 **Configurações**

### **Banco de Dados**
- **Tipo**: SQLite
- **Arquivo**: `SportStore.db` (criado automaticamente)
- **Migrations**: Aplicadas automaticamente na primeira execução

### **JWT**
- **Algoritmo**: HMAC SHA256
- **Expiração**: 60 minutos
- **Issuer**: SportStore
- **Audience**: SportStoreUsers

### **Kafka** (Opcional)
- Configuração para Event-Driven Architecture
- Eventos de domínio para auditoria
- Documentação em `KAFKA_SETUP.md`

## 🛡️ **Segurança**

- **Autenticação JWT** obrigatória para endpoints protegidos
- **Autorização baseada em roles** (Administrador/Vendedor)
- **Senhas criptografadas** com BCrypt
- **Validação de dados** com Data Annotations customizadas
- **CORS configurado** para desenvolvimento
- **Validação de nomes únicos** para produtos
- **Controle de estoque** com validação de disponibilidade

## 🧪 **Testes**

### **Executando Testes Unitários**
```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 **Estrutura do Projeto**

```
SportStore/
├── Controllers/          # Controllers da API
│   ├── BaseController.cs
│   ├── ProdutosController.cs
│   ├── EstoqueController.cs
│   ├── PedidosController.cs
│   └── UsuariosController.cs
├── Domain/              # Entidades e regras de negócio
│   ├── Usuario.cs
│   ├── Produto.cs
│   ├── Estoque.cs
│   ├── Pedido.cs
│   ├── ItemPedido.cs
│   └── ValueObjects/
│       ├── Email.cs
│       ├── Preco.cs
│       └── Documento.cs
├── Application/         # Camada de aplicação
│   ├── DTOs/           # Data Transfer Objects
│   ├── Interfaces/     # Contratos dos serviços
│   ├── Services/       # Implementação dos serviços
│   ├── Events/         # Eventos de domínio
│   └── Validation/     # Validações customizadas
├── Infrastructure/     # Camada de infraestrutura
│   ├── SportStoreContext.cs
│   └── Kafka/          # Integração com Kafka
├── Program.cs          # Configuração da aplicação
├── appsettings.json    # Configurações
└── Dockerfile         # Configuração Docker
```

## 🔍 **Monitoramento e Logs**

- **Logs estruturados** com diferentes níveis
- **Health checks** para monitoramento
- **Eventos de domínio** para auditoria
- **Swagger UI** para documentação interativa

## 🤝 **Contribuição**

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 **Licença**

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👥 **Equipe**

- **Desenvolvimento**: SportStore Team
- **Contato**: contato@sportstore.com

## 🎯 **Roadmap**

- [ ] Implementação de testes de integração
- [ ] Cache com Redis
- [ ] Métricas com Prometheus
- [ ] CI/CD com GitHub Actions
- [ ] API Gateway
- [ ] Microserviços com Docker Compose

---

**Desenvolvido com ❤️ usando .NET 8 e Clean Architecture**