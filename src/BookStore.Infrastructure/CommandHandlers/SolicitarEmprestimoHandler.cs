using BookStore.Application.Commands;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Events;
using BookStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using BookStore.Infrastructure.Persistence;

namespace BookStore.Infrastructure.CommandHandlers;

public class SolicitarEmprestimoHandler
{
    private readonly AppDbContext _db;
    private readonly IEventBus _eventBus;

    public SolicitarEmprestimoHandler(AppDbContext db, IEventBus eventBus)
    {
        _db = db;
        _eventBus = eventBus;
    }

    public async Task<Guid> Handle(SolicitarEmprestimoCommand cmd)
    {
        var livro = await _db.Livros.FirstOrDefaultAsync(l => l.Id == cmd.LivroId);

        if (livro == null)
            throw new DomainException("Livro não encontrado.");

        livro.ReducirDisponibilidade();

        var emprestimo = Emprestimo.Criar(cmd.LivroId);
        _db.Emprestimos.Add(emprestimo);

        await _db.SaveChangesAsync();

        await _eventBus.Publish(new EmprestimoCriadoEvent(
            emprestimo.Id,
            livro.Id,
            livro.Titulo,
            livro.Autor
        ));

        await _eventBus.Publish(new LivroAtualizadoEvent(livro.Id, livro.QtdeDisponivel));

        return emprestimo.Id;
    }
}
