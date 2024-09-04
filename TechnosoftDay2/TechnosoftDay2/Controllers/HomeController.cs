using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TechnosoftDay2.Request;


namespace TechnosoftDay2.Controllers
{
    [RoutePrefix("api/v2/countries")]
    public class HomeController : ApiController
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetList(
            [FromUri] Retrieve.ListQuery query, CancellationToken cancellationToken)
        {

            //var response = await _mediator.Send(query);
            //return Ok(response);
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            return Ok(await _mediator.Send(query ?? new Retrieve.ListQuery(), cancellationToken));
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [FromUri] Retrieve.Query query,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Create(
            [FromBody] Request.Create.Command command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [Route("{id:guid}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update(
            [FromUri] Request.Update.Command command,
            [FromBody] Request.Update.Command command2, CancellationToken cancellationToken)
        {
            command2.Id = command.Id;
            return Ok(await _mediator.Send(command2, cancellationToken));
        }

        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(
            [FromUri] Request.Delete.Command command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
