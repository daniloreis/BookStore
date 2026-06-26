using BookStore.Domain.Enums;
using BookStore.Domain.Exceptions;

namespace BookStore.Domain.Entities;

public class Emprestimo
{
    public Guid Id { get; private set; }
    public Guid LivroId { get; private set; }
    public DateTime DataEmprestimo { get; private set; }
    public DateTime? DataDevolucao { get; private set; }
    public EmprestimoStatus Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataUltimaAtualizacao { get; private set; }

    private Emprestimo() { }

    public static Emprestimo Criar(Guid livroId)
    {
        return new Emprestimo
        {
            Id = Guid.NewGuid(),
            LivroId = livroId,
            DataEmprestimo = DateTime.UtcNow,
            DataDevolucao = null,
            Status = EmprestimoStatus.Ativo,
            DataCriacao = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow
        };
    }

    public void Devolver()
    {
        if (Status == EmprestimoStatus.Devolvido)
            throw new EmprestimoJaDevolvidoException();

        DataDevolucao = DateTime.UtcNow;
        Status = EmprestimoStatus.Devolvido;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    public bool EstaAtivo => Status == EmprestimoStatus.Ativo;
}
