using BookStore.Domain.Exceptions;

namespace BookStore.Domain.Entities;

public class Livro
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = "";
    public string Autor { get; private set; } = "";
    public int? AnoPub { get; private set; }
    public int QtdeDisponivel { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataUltimaAtualizacao { get; private set; }

    private Livro() { }

    public static Livro Criar(string titulo, string autor, int? anoPub = null, int qtdeDisponivel = 1)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new LivroSemTituloException();

        if (string.IsNullOrWhiteSpace(autor))
            throw new LivroSemAutorException();

        return new Livro
        {
            Id = Guid.NewGuid(),
            Titulo = titulo,
            Autor = autor,
            AnoPub = anoPub,
            QtdeDisponivel = qtdeDisponivel,
            DataCriacao = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow
        };
    }

    public void ReducirDisponibilidade()
    {
        if (QtdeDisponivel <= 0)
            throw new LivroNaoDisponiavelException();

        QtdeDisponivel--;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    public void AumentarDisponibilidade()
    {
        QtdeDisponivel++;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }
}
