namespace BookStore.Domain.Events;

public class EmprestimoCriadoEvent : DomainEvent
{
    public EmprestimoCriadoEvent(Guid emprestimoId, Guid livroId, string titulo, string autor)
    {
        EmprestimoId = emprestimoId;
        LivroId = livroId;
        Titulo = titulo;
        Autor = autor;
    }

    public Guid EmprestimoId { get; }
    public Guid LivroId { get; }
    public string Titulo { get; }
    public string Autor { get; }
}

public class EmprestimoDevolvidoEvent : DomainEvent
{
    public EmprestimoDevolvidoEvent(Guid emprestimoId, Guid livroId)
    {
        EmprestimoId = emprestimoId;
        LivroId = livroId;
    }

    public Guid EmprestimoId { get; }
    public Guid LivroId { get; }
}

public class LivroAtualizadoEvent : DomainEvent
{
    public LivroAtualizadoEvent(Guid livroId, int qtdeDisponivel)
    {
        LivroId = livroId;
        QtdeDisponivel = qtdeDisponivel;
    }

    public Guid LivroId { get; }
    public int QtdeDisponivel { get; }
}
