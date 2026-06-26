namespace BookStore.Application.DTOs;

public class LivroDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public int? AnoPub { get; set; }
    public int QtdeDisponivel { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}
