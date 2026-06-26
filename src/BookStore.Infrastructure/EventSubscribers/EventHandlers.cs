using BookStore.Application.Interfaces;
using BookStore.Domain.Events;
using BookStore.Infrastructure.Persistence;
using MongoDB.Driver;

namespace BookStore.Infrastructure.EventSubscribers;

public class EmprestimoCriadoEventHandler : IEventHandler<EmprestimoCriadoEvent>
{
    private readonly MongoDbContext _mongoDb;

    public EmprestimoCriadoEventHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
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
            System.Diagnostics.Debug.WriteLine($"Erro ao sincronizar EmprestimoCriadoEvent: {ex.Message}");
        }
    }
}

public class EmprestimoDevolvidoEventHandler : IEventHandler<EmprestimoDevolvidoEvent>
{
    private readonly MongoDbContext _mongoDb;

    public EmprestimoDevolvidoEventHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
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
            System.Diagnostics.Debug.WriteLine($"Erro ao sincronizar EmprestimoDevolvidoEvent: {ex.Message}");
        }
    }
}

public class LivroAtualizadoEventHandler : IEventHandler<LivroAtualizadoEvent>
{
    private readonly MongoDbContext _mongoDb;

    public LivroAtualizadoEventHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
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
            System.Diagnostics.Debug.WriteLine($"Erro ao sincronizar LivroAtualizadoEvent: {ex.Message}");
        }
    }
}
