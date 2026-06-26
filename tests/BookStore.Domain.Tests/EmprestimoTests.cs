using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Exceptions;
using Xunit;

namespace BookStore.Domain.Tests;

public class EmprestimoTests
{
    [Fact]
    public void Criar_DeveRetornarEmprestimoValido()
    {
        var livroId = Guid.NewGuid();

        var emprestimo = Emprestimo.Criar(livroId);

        Assert.NotEqual(Guid.Empty, emprestimo.Id);
        Assert.Equal(livroId, emprestimo.LivroId);
        Assert.Equal(EmprestimoStatus.Ativo, emprestimo.Status);
        Assert.True(emprestimo.EstaAtivo);
        Assert.Null(emprestimo.DataDevolucao);
    }

    [Fact]
    public void Devolver_ComEmprestimoAtivo_DeveMarcarComoDevolvido()
    {
        var livroId = Guid.NewGuid();
        var emprestimo = Emprestimo.Criar(livroId);

        emprestimo.Devolver();

        Assert.Equal(EmprestimoStatus.Devolvido, emprestimo.Status);
        Assert.False(emprestimo.EstaAtivo);
        Assert.NotNull(emprestimo.DataDevolucao);
    }

    [Fact]
    public void Devolver_ComEmprestimoJaDevolvido_DeveLancarExcecao()
    {
        var livroId = Guid.NewGuid();
        var emprestimo = Emprestimo.Criar(livroId);
        emprestimo.Devolver();

        Assert.Throws<EmprestimoJaDevolvidoException>(() => emprestimo.Devolver());
    }
}
