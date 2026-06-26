using BookStore.Domain.Entities;
using BookStore.Domain.Exceptions;
using Xunit;

namespace BookStore.Domain.Tests;

public class LivroTests
{
    [Fact]
    public void Criar_ComTituloAutorValidos_DeveRetornarLivroValido()
    {
        var titulo = "O Senhor dos Anéis";
        var autor = "J.R.R. Tolkien";
        var anoPub = 1954;

        var livro = Livro.Criar(titulo, autor, anoPub, 5);

        Assert.NotEqual(Guid.Empty, livro.Id);
        Assert.Equal(titulo, livro.Titulo);
        Assert.Equal(autor, livro.Autor);
        Assert.Equal(anoPub, livro.AnoPub);
        Assert.Equal(5, livro.QtdeDisponivel);
    }

    [Fact]
    public void Criar_SemTitulo_DeveLancarExcecao()
    {
        Assert.Throws<LivroSemTituloException>(() =>
            Livro.Criar("", "J.R.R. Tolkien", 1954, 5));
    }

    [Fact]
    public void Criar_SemAutor_DeveLancarExcecao()
    {
        Assert.Throws<LivroSemAutorException>(() =>
            Livro.Criar("O Senhor dos Anéis", "", 1954, 5));
    }

    [Fact]
    public void ReducirDisponibilidade_ComExemplarDisponivel_DeveReduzirQtde()
    {
        var livro = Livro.Criar("O Senhor dos Anéis", "J.R.R. Tolkien", 1954, 5);

        livro.ReducirDisponibilidade();

        Assert.Equal(4, livro.QtdeDisponivel);
    }

    [Fact]
    public void ReducirDisponibilidade_SemExemplarDisponivel_DeveLancarExcecao()
    {
        var livro = Livro.Criar("O Senhor dos Anéis", "J.R.R. Tolkien", 1954, 0);

        Assert.Throws<LivroNaoDisponiavelException>(() => livro.ReducirDisponibilidade());
    }

    [Fact]
    public void AumentarDisponibilidade_DeveAumentarQtde()
    {
        var livro = Livro.Criar("O Senhor dos Anéis", "J.R.R. Tolkien", 1954, 5);

        livro.AumentarDisponibilidade();

        Assert.Equal(6, livro.QtdeDisponivel);
    }
}
