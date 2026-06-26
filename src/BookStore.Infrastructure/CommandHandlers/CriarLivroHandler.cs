using BookStore.Application.Commands;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Events;
using BookStore.Infrastructure.Persistence;

namespace BookStore.Infrastructure.CommandHandlers;

public class CriarLivroHandler
{
    private readonly AppDbContext _db;
    private readonly IEventBus _eventBus;

    public CriarLivroHandler(AppDbContext db, IEventBus eventBus)
    {
        _db = db;
        _eventBus = eventBus;
    }

    public async Task<Guid> Handle(CriarLivroCommand cmd)
    {
        var livro = Livro.Criar(cmd.Titulo, cmd.Autor, cmd.AnoPub, cmd.QtdeDisponivel);

        _db.Livros.Add(livro);
        await _db.SaveChangesAsync();

        await _eventBus.Publish(new LivroAtualizadoEvent(livro.Id, livro.QtdeDisponivel));

        return livro.Id;
    }
}
