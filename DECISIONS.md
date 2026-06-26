# DECISIONS.md

## 1. Adequação da arquitetura

### ✅ Decisão
Implementar CQRS com PostgreSQL para escrita e MongoDB para leitura.

**Cenário**: Sistema de biblioteca com múltiplas consultas de disponibilidade e poucos empréstimos/devoluções por hora.

**Quando usar CQRS**:
- ✅ Muitas leituras, poucas escritas (este caso)
- ✅ Modelos de escrita e leitura complexos e diferentes
- ✅ Escalabilidade independente de read/write
- ✅ Auditoria e event sourcing desejáveis

**Quando NÃO usar CQRS**:
- ❌ Sistema CRUD simples (ex: formulários básicos)
- ❌ Modelo de dados idêntico para leitura/escrita
- ❌ Taxa baixa de requisições
- ❌ Equipe pequena ou imatura

**Conclusão**: CQRS é **adequado** para biblioteca porque:
1. Usuários consultam disponibilidade constantemente (muitas leituras)
2. Empréstimos/devoluções são esporádicos (poucas escritas)
3. Consultas podem ter modelos desnormalizados diferentes
4. Sincronização eventual é tolerável (usuário não vê inconsistência imediata)

### ✅ Decisão
PostgreSQL como banco relacional (master), MongoDB como projeção desnormalizada (read replica).

### Justificativa

**PostgreSQL (Escrita)**:
- ✅ ACID garantido para empréstimos (não permite overbooking)
- ✅ Integridade referencial (Empréstimo.LivroId existe em Livros)
- ✅ Suporta transações complexas
- ❌ Consultas complexas podem ser lentas

**MongoDB (Leitura)**:
- ✅ Queries rápidas em dados desnormalizados
- ✅ Sem joins, direto ao documento
- ✅ Flexibilidade para adicionar campos sem migration
- ❌ Sem garantias ACID (aceitável para leitura)

---

## 2. Consistência eventual

**Cenário**: User A empresta último exemplar. User B tenta emprestar antes do sync.

```
T0: A → POST /emprestimos/livro-1       (qtde = 1)
T1: Validação: OK (qtde > 0)
T2: PostgreSQL: qtde = 0 ✓
T3: Evento publicado
    (MongoDB ainda vê qtde = 1) ⚠️
T4: B → GET /livros/livro-1              ← Vê qtde=1 (desatualizado)
T5: B → POST /emprestimos/livro-1
T6: Validação no PostgreSQL: FAIL        ← Protege contra overbooking
    "Livro não disponível" ✓
T7: MongoDB sync completa (qtde = 0)
```

**Conclusão**: A fonte da verdade é o PostgreSQL. MongoDB pode estar desatualizado, mas:
- A próxima escrita sempre é validada no PostgreSQL
- Usuário vê o erro corretamente quando tenta efetivar
- Sem duplicação de empréstimos

---

## 3. Localização das regras

As regras se encontram na camada de domínio na própria entidade, pois o DDD é contra entidades anêmicas e promove dominios ricos.
O objetivo é centralizar as regras isolando-as em uma camada única.


### 🎯 Regra: "Reduzir QtdeDisponivel ao emprestar"

**Local**: `SolicitarEmprestimoHandler.Handle()` (Infrastructure/CommandHandlers)

**Motivo**:
- Contexto transacional necessário (PostgreSQL)
- Múltiplas solicitações simultâneas → precisa lock/retry
- Não pertence a Domain (não é invariante universal)

```csharp
public async Task<Guid> Handle(SolicitarEmprestimoCommand cmd)
{
    var livro = await _db.Livros.FirstOrDefaultAsync(...);
    livro.ReduzirDisponibilidade();  // ← Domain valida (qtde > 0)
    
    var emprestimo = Emprestimo.Criar(cmd.LivroId);
    _db.Emprestimos.Add(emprestimo);
    await _db.SaveChangesAsync();  // ← Handler gerencia transação
}
```

### 🎯 Regra: "Não permitir criar livro sem título/autor"

**Local**: `Livro.Criar()` (Domain Layer)

**Motivo**: 
- Invariante fundamental (vale em qualquer contexto)
- Construtor privado + factory method garante validade
- Exceção específica `LivroSemTituloException`

```csharp
public static Livro Criar(string titulo, string autor, ...)
{
    if (string.IsNullOrWhiteSpace(titulo))
        throw new LivroSemTituloException();  // ← Domain Layer
    // ...
}
```

### 🎯 Regra: "Não devolver livro já devolvido"

**Local**: `Emprestimo.Devolver()` (Domain Layer)

**Motivo**:
- Invariante: status nunca pode violar transições
- Exceção específica `EmprestimoJaDevolvidoException`

---

## 4. Concorrência: Último Exemplar Simultâneo

### ⚠️ Problema

```
User A: POST /emprestimos/livro-1 (único exemplar)
User B: POST /emprestimos/livro-1 (mesmo instante)
```

### ✅ Implementação: Retry + DbUpdateConcurrencyException

```csharp
public async Task<Guid> Handle(SolicitarEmprestimoCommand cmd)
{
    try
    {
        var livro = await _db.Livros.FirstOrDefaultAsync(l => l.Id == cmd.LivroId);
        livro.ReduzirDisponibilidade();  // Lança exceção se qtde=0
        
        _db.Emprestimos.Add(...);
        await _db.SaveChangesAsync();    // Lança exceção se der conflito
    }
    catch (DbUpdateConcurrencyException)
    {
        // Retry ou erro ao usuário
        throw new DomainException("Livro não estava disponível");
    }
}
```

### Por que essa é boa para Biblioteca?

1. **Raro**: Última cópia emprestada simultaneamente é raro
2. **Aceitável UX**: Usuário vê "não disponível" em 99% dos casos
3. **Simples**: Sem infraestrutura extra (Redis, Outbox table)
4. **Sem Overbooking**: PostgreSQL garante qtde >= 0

---

## 5. Falha de sincronização 

### ✅ Decisão
Event Bus dispara subscribers de forma assíncrona, não aguardando MongoDB.
A sincronização com o Mongo é feita em handlers de evento em EventHandlers.cs.

**Detecção de falha**
O evento que atualiza o Mongo é tratado por LivroAtualizadoEventHandler.Handle(...).
Se ocorrer qualquer exceção no Mongo, o handler captura com try/catch

**Isso significa que**:
A falha não é propagada para o publicador do evento;
O fluxo de comando continua como se o evento tivesse sido processado com sucesso;
 
### Impactos

- ❌ GET /api/emprestimos retorna dados antigos
- ✅ POST /emprestimos valida em PostgreSQL (protegido)


---

## 6. Uso de IA
 Utilizei para criação da estrutura de fluxo de eventos. Ela sugeriu utilizar "Event Bus in-memory" por ser mais simples e eficaz, então eu aceitei.
 Testes unitários também foram gerados pela IA e revisados por mim.

---

## 7. Trade-offs gerais 

Não utilizei fila morta (DLQ) por entender que é uma fila simples que uma vez implementada não haverá risco de mensagem com erro. 
Mas eu poderia colocar para nos casos de falha garantir a resiliência.

---



