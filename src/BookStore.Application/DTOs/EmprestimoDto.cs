namespace BookStore.Application.DTOs;

public class EmprestimoDto
{
    public Guid Id { get; set; }
    public Guid LivroId { get; set; }
    public string TituloLivro { get; set; } = string.Empty;
    public string AutorLivro { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}
