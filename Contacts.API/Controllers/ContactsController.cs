using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Contacts.Application.Commands;
using Contacts.Application.Models;
using Contacts.Application.Queries;
using Contacts.Application.Queries.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ContactsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReadContactQueryResponse>> GetContact(Guid id,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var res = await _mediator.Send(new ReadContactQuery { Id = id, Etag = etag },
            cancellationToken);
        var output = _mapper.Map<ReadContactDto>(res);
        Response.Headers.Add("ETag", res.Etag);
        return Ok(output);
    }

    [HttpGet]
    public async Task<ActionResult<ReadAllContactsQueryResponse>> GetAllContacts(
        CancellationToken cancellationToken,
        [FromHeader(Name = "x-continuation-token")]
        string continuationToken = null, [FromQuery(Name = "pageSize")] int pageSize = 10)
    {
        var res = await _mediator.Send(new ReadAllContactsQuery
            { PageSize = pageSize, ContinuationToken = continuationToken }, cancellationToken);
        var output = _mapper.Map<ReadAllContactsDto>(res);
        Response.Headers.Add("X-Continuation-Token", res.ContinuationToken);
        return Ok(output);
    }

    [HttpPost]
    public async Task<ActionResult> CreateContact([FromBody] CreateContactDto dto,
        CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<CreateContactCommand>(dto);
        var res = await _mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetContact), new { id = res.Id }, null);
    }

    [HttpPost("{id:guid}/update-name")]
    public async Task<ActionResult> UpdateContactName(Guid id, [FromBody] UpdateContactNameDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = _mapper.Map<UpdateContactNameCommand>(dto);

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-description")]
    public async Task<ActionResult> UpdateContactDescription(Guid id, [FromBody] UpdateContactDescriptionDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = _mapper.Map<UpdateContactDescriptionCommand>(dto);

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-email")]
    public async Task<ActionResult> UpdateContactEmail(Guid id, [FromBody] UpdateContactEmailDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = _mapper.Map<UpdateContactEmailCommand>(dto);

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-company")]
    public async Task<ActionResult> UpdateContactCompany(Guid id, [FromBody] UpdateContactCompanyDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = _mapper.Map<UpdateContactCompanyCommand>(dto);

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteContact(Guid id,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var res = await _mediator.Send(new DeleteContactCommand { Id = id, Etag = etag },
            cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return Ok();
    }
}