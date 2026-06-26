using BookStore.Application.Commands;
using BookStore.Application.Interfaces;
using BookStore.Domain.Events;
using BookStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using BookStore.Infrastructure.Persistence;

namespace BookStore.Infrastructure.CommandHandlers;

public class DevolverEmprestimoHandler
{
    private readonly AppDbContext _db;
    private readonly IEventBus _eventBus;

    public DevolverEmprestimoHandler(AppDbContext db, IEventBus eventBus)
    {
        _db = db;
        _eventBus = eventBus;
    }

    public async Task<Guid> Handle(DevolverEmprestimoCommand cmd)
    {
        var emprestimo = await _db.Emprestimos.FirstOrDefaultAsync(e => e.Id == cmd.EmprestimoId);

        if (emprestimo == null)
            throw new DomainException("Empréstimo não encontrado.");

        emprestimo.Devolver();

        var livro = await _db.Livros.FirstOrDefaultAsync(l => l.Id == emprestimo.LivroId);
        if (livro != null)
        {
            livro.AumentarDisponibilidade();
        }

        await _db.SaveChangesAsync();

        await _eventBus.Publish(new EmprestimoDevolvidoEvent(emprestimo.Id, emprestimo.LivroId));

        if (livro != null)
            await _eventBus.Publish(new LivroAtualizadoEvent(livro.Id, livro.QtdeDisponivel));

        return emprestimo.Id;
    }
}
