using BookStore.Application.Commands;
using BookStore.Application.Interfaces;
using BookStore.Domain.Events;
using BookStore.Domain.Exceptions;
using BookStore.Infrastructure.CommandHandlers;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookStore.Application.Tests;

public class CommandHandlersTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CriarLivroHandler_DeveSalvarLivroESolicitarEvento()
    {
        var db = CreateInMemoryDbContext();
        var eventBusMock = new Mock<IEventBus>();
        var handler = new CriarLivroHandler(db, eventBusMock.Object);

        var cmd = new CriarLivroCommand("1984", "George Orwell", 1949, 10);
        var id = await handler.Handle(cmd);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Single(db.Livros);
        eventBusMock.Verify(x => x.Publish(It.IsAny<LivroAtualizadoEvent>()), Times.Once);
    }

    [Fact]
    public async Task SolicitarEmprestimoHandler_ComLivroDisponivel_DeveCriarEmprestimo()
    {
        var db = CreateInMemoryDbContext();
        var eventBusMock = new Mock<IEventBus>();

        var livro = Domain.Entities.Livro.Criar("1984", "George Orwell", 1949, 1);
        db.Livros.Add(livro);
        await db.SaveChangesAsync();

        var handler = new SolicitarEmprestimoHandler(db, eventBusMock.Object);
        var cmd = new SolicitarEmprestimoCommand(livro.Id);

        var emprestimoId = await handler.Handle(cmd);

        Assert.NotEqual(Guid.Empty, emprestimoId);
        Assert.Single(db.Emprestimos);
        Assert.Equal(0, db.Livros.First().QtdeDisponivel);
        eventBusMock.Verify(x => x.Publish(It.IsAny<EmprestimoCriadoEvent>()), Times.Once);
    }

    [Fact]
    public async Task SolicitarEmprestimoHandler_SemExemplarDisponivel_DeveLancarExcecao()
    {
        var db = CreateInMemoryDbContext();
        var eventBusMock = new Mock<IEventBus>();

        var livro = Domain.Entities.Livro.Criar("1984", "George Orwell", 1949, 0);
        db.Livros.Add(livro);
        await db.SaveChangesAsync();

        var handler = new SolicitarEmprestimoHandler(db, eventBusMock.Object);
        var cmd = new SolicitarEmprestimoCommand(livro.Id);

        await Assert.ThrowsAsync<LivroNaoDisponiavelException>(() => handler.Handle(cmd));
    }

    [Fact]
    public async Task DevolverEmprestimoHandler_DeveDevolverEmprestimoEAumentarDisponibilidade()
    {
        var db = CreateInMemoryDbContext();
        var eventBusMock = new Mock<IEventBus>();

        var livro = Domain.Entities.Livro.Criar("1984", "George Orwell", 1949, 0);
        db.Livros.Add(livro);

        var emprestimo = Domain.Entities.Emprestimo.Criar(livro.Id);
        db.Emprestimos.Add(emprestimo);
        await db.SaveChangesAsync();

        var handler = new DevolverEmprestimoHandler(db, eventBusMock.Object);
        var cmd = new DevolverEmprestimoCommand(emprestimo.Id);

        await handler.Handle(cmd);

        Assert.Equal(1, db.Livros.First().QtdeDisponivel);
        Assert.Equal("Devolvido", db.Emprestimos.First().Status.ToString());
        eventBusMock.Verify(x => x.Publish(It.IsAny<EmprestimoDevolvidoEvent>()), Times.Once);
    }
}
