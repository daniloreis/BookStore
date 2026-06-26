namespace BookStore.Application.DTOs;

public class LivroDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int? AnoPub { get; set; }
    public int QtdeDisponivel { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}
