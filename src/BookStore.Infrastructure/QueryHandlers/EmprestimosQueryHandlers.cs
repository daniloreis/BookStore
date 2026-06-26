using BookStore.Application.DTOs;
using BookStore.Application.Queries;
using MongoDB.Driver;
using BookStore.Infrastructure.Persistence;

namespace BookStore.Infrastructure.QueryHandlers;

public class ListarEmprestimosHandler
{
    private readonly MongoDbContext _mongoDb;

    public ListarEmprestimosHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
    }

    public async Task<List<EmprestimoDto>> Handle(ListarEmprestimosQuery query)
    {
        var emprestimos = await _mongoDb.Emprestimos
            .Find(_ => true)
            .ToListAsync();

        return emprestimos.Select(e => new EmprestimoDto
        {
            Id = e.Id,
            LivroId = e.LivroId,
            TituloLivro = e.TituloLivro,
            AutorLivro = e.AutorLivro,
            DataEmprestimo = e.DataEmprestimo,
            DataDevolucao = e.DataDevolucao,
            Status = e.Status,
            DataCriacao = e.DataCriacao,
            DataUltimaAtualizacao = e.DataUltimaAtualizacao
        }).ToList();
    }
}

public class ObterEmprestimoHandler
{
    private readonly MongoDbContext _mongoDb;

    public ObterEmprestimoHandler(MongoDbContext mongoDb)
    {
        _mongoDb = mongoDb;
    }

    public async Task<EmprestimoDto> Handle(ObterEmprestimoQuery query)
    {
        var emprestimo = await _mongoDb.Emprestimos
            .Find(e => e.Id == query.Id)
            .FirstOrDefaultAsync();

        if (emprestimo == null)
            return null;

        return new EmprestimoDto
        {
            Id = emprestimo.Id,
            LivroId = emprestimo.LivroId,
            TituloLivro = emprestimo.TituloLivro,
            AutorLivro = emprestimo.AutorLivro,
            DataEmprestimo = emprestimo.DataEmprestimo,
            DataDevolucao = emprestimo.DataDevolucao,
            Status = emprestimo.Status,
            DataCriacao = emprestimo.DataCriacao,
            DataUltimaAtualizacao = emprestimo.DataUltimaAtualizacao
        };
    }
}
