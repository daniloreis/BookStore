using BookStore.Application.Interfaces;
using BookStore.Domain.Events;
using BookStore.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BookStore.Infrastructure.EventSubscribers;

public class EmprestimoCriadoEventHandler : IEventHandler<EmprestimoCriadoEvent>
{
    private readonly MongoDbContext _mongoDb;
    private readonly ILogger<EmprestimoCriadoEventHandler> _logger;

    public EmprestimoCriadoEventHandler(MongoDbContext mongoDb, ILogger<EmprestimoCriadoEventHandler> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    public async Task Handle(EmprestimoCriadoEvent @event)
    {
        try
        {
            var emprestimo = new MongoEmprestimo
            {
                Id = @event.EmprestimoId,
                LivroId = @event.LivroId,
                TituloLivro = @event.Titulo,
                AutorLivro = @event.Autor,
                DataEmprestimo = @event.OccurredOn,
                DataDevolucao = null,
                Status = "Ativo",
                DataCriacao = @event.OccurredOn,
                DataUltimaAtualizacao = @event.OccurredOn
            };

            var options = new ReplaceOptions { IsUpsert = true };
            await _mongoDb.Emprestimos.ReplaceOneAsync(
                e => e.Id == emprestimo.Id,
                emprestimo,
                options
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar EmprestimoCriadoEvent");
        }
    }
}

public class EmprestimoDevolvidoEventHandler : IEventHandler<EmprestimoDevolvidoEvent>
{
    private readonly MongoDbContext _mongoDb;
    private readonly ILogger<EmprestimoDevolvidoEventHandler> _logger;

    public EmprestimoDevolvidoEventHandler(MongoDbContext mongoDb, ILogger<EmprestimoDevolvidoEventHandler> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    public async Task Handle(EmprestimoDevolvidoEvent @event)
    {
        try
        {
            var update = Builders<MongoEmprestimo>.Update
                .Set(e => e.Status, "Devolvido")
                .Set(e => e.DataDevolucao, @event.OccurredOn)
                .Set(e => e.DataUltimaAtualizacao, @event.OccurredOn);

            await _mongoDb.Emprestimos.UpdateOneAsync(
                e => e.Id == @event.EmprestimoId,
                update
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar EmprestimoDevolvidoEvent");
        }
    }
}

public class LivroAtualizadoEventHandler : IEventHandler<LivroAtualizadoEvent>
{
    private readonly MongoDbContext _mongoDb;
    private readonly ILogger<LivroAtualizadoEventHandler> _logger;

    public LivroAtualizadoEventHandler(MongoDbContext mongoDb, ILogger<LivroAtualizadoEventHandler> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    public async Task Handle(LivroAtualizadoEvent @event)
    {
        try
        {
            var update = Builders<MongoLivro>.Update
                .Set(l => l.QtdeDisponivel, @event.QtdeDisponivel)
                .Set(l => l.DataUltimaAtualizacao, @event.OccurredOn);

            await _mongoDb.Livros.UpdateOneAsync(
                l => l.Id == @event.LivroId,
                update,
                new UpdateOptions { IsUpsert = true }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar LivroAtualizadoEvent");
        }
    }
}
