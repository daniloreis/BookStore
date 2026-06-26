namespace BookStore.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class LivroSemTituloException : DomainException
{
    public LivroSemTituloException() : base("Livro deve ter um título.") { }
}

public class LivroSemAutorException : DomainException
{
    public LivroSemAutorException() : base("Livro deve ter um autor.") { }
}

public class LivroNaoDisponiavelException : DomainException
{
    public LivroNaoDisponiavelException() : base("Não há exemplares disponíveis deste livro.") { }
}

public class EmprestimoJaDevolvidoException : DomainException
{
    public EmprestimoJaDevolvidoException() : base("Este empréstimo já foi devolvido.") { }
}
