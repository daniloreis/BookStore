using BookStore.Application.DTOs;
using BookStore.Application.Queries;
using MongoDB.Driver;
using BookStore.Infrastructure.Persistence;

namespace BookStore.Infrastructure.QueryHandlers;

public class ListarLivrosHandler
{
    private readonly MongoDbContext _mongoDb;

    public ListarLivrosHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
    }

    public async Task<List<LivroDto>> Handle(ListarLivrosQuery query)
    {
        var livros = await _mongoDb.Livros
            .Find(_ => true)
            .ToListAsync();

        return livros.Select(l => new LivroDto
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Autor = l.Autor,
            AnoPub = l.AnoPub,
            QtdeDisponivel = l.QtdeDisponivel,
            DataCriacao = l.DataCriacao,
            DataUltimaAtualizacao = l.DataUltimaAtualizacao
        }).ToList();
    }
}

public class ObterLivroHandler
{
    private readonly MongoDbContext _mongoDb;

    public ObterLivroHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
    }

    public async Task<LivroDto> Handle(ObterLivroQuery query)
    {
        var livro = await _mongoDb.Livros
            .Find(l => l.Id == query.Id)
            .FirstOrDefaultAsync();

        if (livro == null)
            return null;

        return new LivroDto
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Autor = livro.Autor,
            AnoPub = livro.AnoPub,
            QtdeDisponivel = livro.QtdeDisponivel,
            DataCriacao = livro.DataCriacao,
            DataUltimaAtualizacao = livro.DataUltimaAtualizacao
        };
    }
}
