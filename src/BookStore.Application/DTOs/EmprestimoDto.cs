namespace BookStore.Application.DTOs;

public class EmprestimoDto
{
    public Guid Id { get; set; }
    public Guid LivroId { get; set; }
    public string TituloLivro { get; set; }
    public string AutorLivro { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}
