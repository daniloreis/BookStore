using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BookStore.Infrastructure.Persistence;

public class MongoLivro
{
    [BsonId]
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public int? AnoPub { get; set; }
    public int QtdeDisponivel { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}

public class MongoEmprestimo
{
    [BsonId]
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

public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);
    }

    public IMongoCollection<MongoLivro> Livros => _db.GetCollection<MongoLivro>("Livros");
    public IMongoCollection<MongoEmprestimo> Emprestimos => _db.GetCollection<MongoEmprestimo>("Emprestimos");
}
