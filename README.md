# BookStore - Sistema de Gestão de Bibliotecas Comunitárias

Sistema backend completo em C# .NET 10 para gerenciamento de livros e empréstimos em bibliotecas comunitárias, implementado com arquitetura CQRS, PostgreSQL (escrita) e MongoDB (leitura).

## 📋 Arquitetura

- **Domain Layer**: Entidades, Value Objects e Eventos de Domínio
- **Application Layer**: Commands, Queries, DTOs e Interfaces
- **Infrastructure Layer**: EF Core, MongoDB, Event Bus e Persistência
- **Api Layer**: Controllers REST e Configuração

### Padrões Implementados

- **CQRS**: Separação de Commands (escrita) e Queries (leitura)
- **Event Sourcing**: Sincronização entre PostgreSQL e MongoDB via Eventos
- **Dependency Injection**: Injeção de dependências centralizada
- **Domain-Driven Design**: Regras de negócio no Domain Layer

## 🚀 Como Executar

### Pré-requisitos

- .NET 10 SDK
- .NET EF:
    dotnet tool install --global dotnet-ef;
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL;
    dotnet add package Microsoft.EntityFrameworkCore.Design;
- Docker e Docker Compose
- Git

### 1. Clonar o Repositório

```bash
git clone https://github.com/daniloreis/BookStore.git
cd BookStore
```

### 2. Iniciar Bancos de Dados

```bash
docker-compose up -d
```

Isso inicia:
- **PostgreSQL**: `localhost:5432` (usuário: `postgres`, senha: `postgres`)
- **MongoDB**: `localhost:27017` (usuário: `mongoadmin`, senha: `mongoadmin`)

### 3. Executar Migrations

```bash
cd src/BookStore.Api
dotnet ef migrations add Initial --project ../BookStore.Infrastructure
dotnet ef database update
cd ../../
```
'
### 4. Executar a Aplicação

```bash
dotnet run --project src/BookStore.Api
```

A API estará disponível em: `http://localhost:5054/swagger/index.html`

### 5. Executar Testes

```bash
dotnet test
```

## 📚 Endpoints da API

### Livros

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/livros` | Criar novo livro |
| `GET` | `/api/livros` | Listar todos os livros |
| `GET` | `/api/livros/{id}` | Obter livro por ID |

#### Exemplo - Criar Livro

```bash
curl -X POST https://localhost:5054/api/livros \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "O Cortiço",
    "autor": "Aluísio Azevedo",
    "anoPub": 1890,
    "qtdeDisponivel": 5
  }'
```

### Empréstimos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/emprestimos` | Solicitar empréstimo |
| `PUT` | `/api/emprestimos/{id}/devolver` | Devolver empréstimo |
| `GET` | `/api/emprestimos` | Listar empréstimos |
| `GET` | `/api/emprestimos/{id}` | Obter empréstimo por ID |

#### Exemplo - Solicitar Empréstimo

```bash
curl -X POST https://localhost:5054/api/emprestimos \
  -H "Content-Type: application/json" \
  -d '{
    "livroId": "<uuid-do-livro>"
  }'
```

#### Exemplo - Devolver Empréstimo

```bash
curl -X PUT https://localhost:5054/api/emprestimos/<uuid-emprestimo>/devolver
```

## 🔧 Configuração

### Variáveis de Ambiente

Editar `src/BookStore.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSql": "Host=localhost;Port=5432;Database=bookstore;Username=postgres;Password=postgres",
    "MongoDB": "mongodb://mongoadmin:mongoadmin@localhost:27017/BookstoreDb?authSource=admin"
  }
}
```

## ✅ Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Apenas Testes de Domain

```bash
dotnet test tests/BookStore.Domain.Tests
```

### Executar Apenas Testes de Application

```bash
dotnet test tests/BookStore.Application.Tests
```

## 📁 Estrutura do Projeto

```
BookStore/
├── src/
│   ├── BookStore.Domain/              # Entidades, Eventos, Exceções
│   ├── BookStore.Application/         # Commands, Queries, DTOs
│   ├── BookStore.Infrastructure/      # EF Core, MongoDB, Event Bus
│   └── BookStore.Api/                 # Controllers, Configuração
├── tests/
│   ├── BookStore.Domain.Tests/        # Testes de Domínio
│   └── BookStore.Application.Tests/   # Testes de Aplicação
├── docker-compose.yml                 # Configuração dos containers
├── README.md                          # Este arquivo
└── DECISIONS.md                       # Justificativas arquiteturais
```

## 🔄 Fluxo de Sincronização

1. **Comando (Escrita)**: Usuario faz POST para criar/modificar
2. **Handler (Application)**: Valida regra de negócio no PostgreSQL
3. **Evento**: Handler publica evento via Event Bus
4. **Subscriber**: Event Bus dispara handlers de sincronização
5. **MongoDB**: Dados são sincronizados
6. **Query (Leitura)**: Usuario faz GET, retorna dados do MongoDB

## ⚠️ Considerações Importantes

### Concorrência
- Último exemplar disponível: Validação no PostgreSQL + retry em caso de concorrência

### Inconsistência Eventual
- Janela entre escrita no PostgreSQL e sincronização no MongoDB
- Queries podem retornar dados desatualizados por alguns milissegundos
- Próxima solicitação de empréstimo sempre valida no PostgreSQL

### Recuperação de Falhas
- Event Bus em memória: eventos perdem-se se app reinicia
- Para produção: considerar Outbox Pattern ou Event Store

## 📝 Regras de Domínio Implementadas

### Livro
- ✅ Título obrigatório
- ✅ Autor obrigatório
- ✅ Quantidade disponível >= 0
- ✅ Redução automática ao emprestar
- ✅ Aumento automático ao devolver

### Empréstimo
- ✅ Exige exemplar disponível
- ✅ Não permite devolução dupla
- ✅ Status: Ativo ou Devolvido
- ✅ Data de devolução preenchida apenas após devolução

## 🛠️ Troubleshooting

### PostgreSQL não conecta
```bash
docker-compose logs postgres
docker-compose restart postgres
```

### MongoDB não conecta
```bash
docker-compose logs mongodb
docker-compose restart mongodb
```

### Limpar dados e recomeçar
```bash
docker-compose down -v
docker-compose up -d
```

## 📖 Documentação Adicional

Ver `DECISIONS.md` para análise detalhada das decisões arquiteturais.
