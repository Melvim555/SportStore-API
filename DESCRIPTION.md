# SportStore API - Descrição Técnica

## 📋 **Resumo Executivo**

API RESTful completa para gerenciamento de equipamentos esportivos desenvolvida em **.NET 8** com **Clean Architecture**, **DDD** e **Event-Driven Architecture**. Sistema de CRUD completo com autenticação JWT, controle de permissões e gestão de estoque em tempo real.

## 🎯 **Principais Funcionalidades**

### **Sistema de Autenticação & Autorização**
- JWT Token com controle de permissões
- Roles: Administrador e Vendedor
- Senhas criptografadas com BCrypt
- Validações de segurança robustas

### **Gestão de Produtos** (Admin)
- CRUD completo com validações
- Nomes únicos e preços controlados
- Ativação/desativação de produtos
- Value Objects para integridade

### **Controle de Estoque** (Admin)
- Movimentações com histórico completo
- Controle por nota fiscal
- Verificação de disponibilidade
- Baixa automática em pedidos

### **Sistema de Pedidos** (Vendedor)
- Criação com validação de estoque
- Controle de cliente por CPF
- Histórico por vendedor
- Eventos Kafka para auditoria

## 🏗️ **Arquitetura Técnica**

### **Stack Tecnológica**
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados
- **JWT Bearer** - Autenticação
- **Kafka** - Event streaming
- **Docker** - Containerização

### **Padrões Arquiteturais**
- **Clean Architecture** - Separação de responsabilidades
- **Domain-Driven Design** - Entidades e regras de negócio
- **Event-Driven Architecture** - Eventos de domínio
- **Repository Pattern** - Acesso a dados
- **CQRS** - Separação de comandos e consultas

## 📊 **Endpoints da API**

### **Autenticação**
- `POST /api/usuarios` - Cadastro
- `POST /api/usuarios/login` - Login

### **Produtos** (Admin)
- `GET /api/produtos` - Listar
- `POST /api/produtos` - Criar
- `PUT /api/produtos/{id}` - Atualizar
- `DELETE /api/produtos/{id}` - Excluir

### **Estoque** (Admin)
- `POST /api/estoque/adicionar` - Adicionar
- `GET /api/estoque/historico` - Histórico
- `GET /api/estoque/disponivel/{id}` - Disponibilidade

### **Pedidos** (Vendedor)
- `POST /api/pedidos` - Criar
- `GET /api/pedidos` - Listar
- `GET /api/pedidos/{id}` - Detalhar

## 🔧 **Configuração & Deploy**

### **Execução Local**
```bash
git clone <repo>
cd SportStore
dotnet restore
dotnet run
```

### **Docker**
```bash
docker build -t sportstore-api .
docker run -p 8080:8080 sportstore-api
```

### **Acesso**
- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

## 🛡️ **Segurança**

- Autenticação JWT obrigatória
- Autorização baseada em roles
- Validação de dados com Data Annotations
- Senhas criptografadas
- CORS configurado
- Validação de nomes únicos

## 📈 **Benefícios Técnicos**

- **Escalabilidade** - Arquitetura limpa e modular
- **Manutenibilidade** - Código bem estruturado
- **Testabilidade** - Separação de responsabilidades
- **Documentação** - Swagger automático
- **Deploy** - Docker support
- **Monitoramento** - Logs estruturados

## 🎯 **Casos de Uso**

1. **Administrador** cadastra produtos e gerencia estoque
2. **Vendedor** cria pedidos com validação automática
3. **Sistema** controla disponibilidade em tempo real
4. **Eventos** são disparados para auditoria via Kafka
5. **API** documentada automaticamente via Swagger

## 📋 **Status do Projeto**

✅ **Completo e Funcional**
- Todas as funcionalidades implementadas
- Testes unitários configurados
- Documentação completa
- Docker support
- Pronto para produção

---

**Desenvolvido com .NET 8, Clean Architecture e DDD**
