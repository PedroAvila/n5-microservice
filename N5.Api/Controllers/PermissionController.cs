
namespace N5.Api.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _mediator.Send(new GetPermissionsQuery());
                return Ok(new CustomResponse<GetPermissionsResult>((int)HttpStatusCode.OK, result));
            }
            catch (BusinessException e)
            {
                return StatusCode(e.Status, new CustomResponse<string>(e.Status, e.Mensaje!));
            }
            catch (Exception e)
            {
                return new ObjectResult(new CustomResponse<string>(500, e.Message)) { StatusCode = 500 };
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Single(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetPermissionByIdQuery(id));
                return Ok(new CustomResponse<GetPermissionByIdResult>((int)HttpStatusCode.OK, result));
            }
            catch (BusinessException e)
            {
                return StatusCode(e.Status, new CustomResponse<string>(e.Status, e.Mensaje!));
            }
            catch (Exception e)
            {
                return new ObjectResult(new CustomResponse<string>(500, e.Message)) { StatusCode = 500 };
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionCommand dto)
        {
            try
            {
                var result = await _mediator.Send(dto);
                var response = new CustomResponse<CreatePermissionsResult>((int)HttpStatusCode.Created, result);
                return Ok(response);
            }
            catch (BusinessException e)
            {
                return StatusCode(e.Status, new CustomResponse<string>(e.Status, e.Mensaje!));
            }
            catch (Exception e)
            {
                return new ObjectResult(new CustomResponse<string>(500, e.Message)) { StatusCode = 500 };
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionCommand dto)
        {
            try
            {
                var command = new UpdatePermissionCommandWithId(id, dto);
                var result = await _mediator.Send(command);
                var response = new CustomResponse<UpdatePermissionsResult>((int)HttpStatusCode.NoContent, result);
                return new ObjectResult(response);
            }
            catch (BusinessException e)
            {
                return StatusCode(e.Status, new CustomResponse<string>(e.Status, e.Mensaje!));
            }
            catch (Exception e)
            {
                return new ObjectResult(new CustomResponse<string>(500, e.Message)) { StatusCode = 500 };
            }
        }
    }
}
