using BookStore.Infrastructure.CommandHandlers;
using BookStore.Application.Interfaces;
using BookStore.Infrastructure.QueryHandlers;
using BookStore.Infrastructure.EventBus;
using BookStore.Infrastructure.EventSubscribers;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BookStore.Domain.Events;

var builder = WebApplication.CreateBuilder(args);

var pgConnection = builder.Configuration.GetConnectionString("PostgreSql")
    ?? "Host=localhost;Port=5432;Database=bookstore;Username=postgres;Password=postgres";

var mongoConnection = builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://localhost:27017";

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "BookStore API",
        Version = "v1",
        Description = "API para gerenciamento de livros e empréstimos"
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(pgConnection)
);

builder.Services.AddSingleton(new MongoDbContext(mongoConnection, "BookstoreDb"));

var eventBus = new EventBus();
builder.Services.AddSingleton<IEventBus>(eventBus);

builder.Services.AddScoped<CriarLivroHandler>();
builder.Services.AddScoped<SolicitarEmprestimoHandler>();
builder.Services.AddScoped<DevolverEmprestimoHandler>();

builder.Services.AddScoped<ListarLivrosHandler>();
builder.Services.AddScoped<ObterLivroHandler>();
builder.Services.AddScoped<ListarEmprestimosHandler>();
builder.Services.AddScoped<ObterEmprestimoHandler>();

builder.Services.AddScoped<EmprestimoCriadoEventHandler>();
builder.Services.AddScoped<EmprestimoDevolvidoEventHandler>();
builder.Services.AddScoped<LivroAtualizadoEventHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var emprestimoCriadoHandler = scope.ServiceProvider.GetRequiredService<EmprestimoCriadoEventHandler>();
    var emprestimoDevolvidoHandler = scope.ServiceProvider.GetRequiredService<EmprestimoDevolvidoEventHandler>();
    var livroAtualizadoHandler = scope.ServiceProvider.GetRequiredService<LivroAtualizadoEventHandler>();

    eventBus.Subscribe<EmprestimoCriadoEvent>(emprestimoCriadoHandler);
    eventBus.Subscribe<EmprestimoDevolvidoEvent>(emprestimoDevolvidoHandler);
    eventBus.Subscribe<LivroAtualizadoEvent>(livroAtualizadoHandler);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API v1");
    });

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

app.Run();
