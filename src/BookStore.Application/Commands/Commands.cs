namespace BookStore.Application.Commands;

public record CriarLivroCommand(string Titulo, string Autor, int? AnoPub = null, int QtdeDisponivel = 1);

public record SolicitarEmprestimoCommand(Guid LivroId);

public record DevolverEmprestimoCommand(Guid EmprestimoId);
