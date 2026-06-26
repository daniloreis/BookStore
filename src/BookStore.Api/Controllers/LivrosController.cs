using BookStore.Application.Commands;
using BookStore.Infrastructure.CommandHandlers;
using BookStore.Application.DTOs;
using BookStore.Application.Queries;
using BookStore.Infrastructure.QueryHandlers;
using BookStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly CriarLivroHandler _criarLivroHandler;
    private readonly ListarLivrosHandler _listarLivrosHandler;
    private readonly ObterLivroHandler _obterLivroHandler;

    public LivrosController(
        CriarLivroHandler criarLivroHandler,
        ListarLivrosHandler listarLivrosHandler,
        ObterLivroHandler obterLivroHandler)
    {
        _criarLivroHandler = criarLivroHandler;
        _listarLivrosHandler = listarLivrosHandler;
        _obterLivroHandler = obterLivroHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CriarLivro([FromBody] CriarLivroCommand cmd)
    {
        try
        {
            var id = await _criarLivroHandler.Handle(cmd);
            return CreatedAtAction(nameof(ObterLivro), new { id }, id);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<LivroDto>>> ListarLivros()
    {
        try
        {
            var livros = await _listarLivrosHandler.Handle(new ListarLivrosQuery());
            return Ok(livros);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao listar livros", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LivroDto>> ObterLivro([FromRoute] Guid id)
    {
        try
        {
            var livro = await _obterLivroHandler.Handle(new ObterLivroQuery(id));
            if (livro == null)
                return NotFound(new { error = "Livro não encontrado" });

            return Ok(livro);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao obter livro", details = ex.Message });
        }
    }
}
