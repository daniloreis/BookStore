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
public class EmprestimosController : ControllerBase
{
    private readonly SolicitarEmprestimoHandler _solicitarHandler;
    private readonly DevolverEmprestimoHandler _devolverHandler;
    private readonly ListarEmprestimosHandler _listarHandler;
    private readonly ObterEmprestimoHandler _obterHandler;

    public EmprestimosController(
        SolicitarEmprestimoHandler solicitarHandler,
        DevolverEmprestimoHandler devolverHandler,
        ListarEmprestimosHandler listarHandler,
        ObterEmprestimoHandler obterHandler)
    {
        _solicitarHandler = solicitarHandler;
        _devolverHandler = devolverHandler;
        _listarHandler = listarHandler;
        _obterHandler = obterHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> SolicitarEmprestimo([FromBody] SolicitarEmprestimoCommand cmd)
    {
        try
        {
            var id = await _solicitarHandler.Handle(cmd);
            return CreatedAtAction(nameof(ObterEmprestimo), new { id }, id);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao solicitar empréstimo", details = ex.Message });
        }
    }

    [HttpPut("{id}/devolver")]
    public async Task<ActionResult<Guid>> DevolverEmprestimo([FromRoute] Guid id)
    {
        try
        {
            var result = await _devolverHandler.Handle(new DevolverEmprestimoCommand(id));
            return Ok(new { message = "Empréstimo devolvido com sucesso", emprestimoId = result });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao devolver empréstimo", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<EmprestimoDto>>> ListarEmprestimos()
    {
        try
        {
            var emprestimos = await _listarHandler.Handle(new ListarEmprestimosQuery());
            return Ok(emprestimos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao listar empréstimos", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmprestimoDto>> ObterEmprestimo([FromRoute] Guid id)
    {
        try
        {
            var emprestimo = await _obterHandler.Handle(new ObterEmprestimoQuery(id));
            if (emprestimo == null)
                return NotFound(new { error = "Empréstimo não encontrado" });

            return Ok(emprestimo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao obter empréstimo", details = ex.Message });
        }
    }
}
