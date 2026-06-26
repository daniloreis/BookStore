using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace BookStore.Infrastructure.Persistence;

public class MongoLivro
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int? AnoPub { get; set; }
    public int QtdeDisponivel { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}

public class MongoEmprestimo
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid LivroId { get; set; }
    public string TituloLivro { get; set; } = string.Empty;
    public string AutorLivro { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}

public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(string connectionString, string databaseName)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var settings = MongoClientSettings.FromConnectionString(connectionString);
        var client = new MongoClient(settings);
        _db = client.GetDatabase(databaseName);
    }

    public IMongoCollection<MongoLivro> Livros => _db.GetCollection<MongoLivro>("Livros");
    public IMongoCollection<MongoEmprestimo> Emprestimos => _db.GetCollection<MongoEmprestimo>("Emprestimos");
}
