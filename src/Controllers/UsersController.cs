using Microsoft.AspNetCore.Mvc;
using UserManagement.CQRS.Commands;
using UserManagement.CQRS.Interfaces;
using UserManagement.CQRS.Queries;
using UserManagement.Dtos;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IQueryHandler<SearchUsersQuery, PagedResult<UserResponse>> _search;
        private readonly IQueryHandler<GetUserByIdQuery, UserResponse?> _getById;
        private readonly ICommandHandler<CreateUserCommand, UserResponse> _create;
        private readonly ICommandHandler<UpdateUserCommand, UserResponse?> _update;
        private readonly ICommandHandler<DeleteUserCommand, bool> _delete;

        public UsersController(
            IQueryHandler<SearchUsersQuery, PagedResult<UserResponse>> search,
            IQueryHandler<GetUserByIdQuery, UserResponse?> getById,
            ICommandHandler<CreateUserCommand, UserResponse> create,
            ICommandHandler<UpdateUserCommand, UserResponse?> update,
            ICommandHandler<DeleteUserCommand, bool> delete)
        {
            _search = search;
            _getById = getById;
            _create = create;
            _update = update;
            _delete = delete;
        }

        /// <summary>List users with optional search and pagination.</summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserResponse>>> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _search.HandleAsync(new SearchUsersQuery(search, page, pageSize));
            return Ok(result);
        }

        /// <summary>Create a new user.</summary>
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest request)
        {
            try
            {
                var created = await _create.HandleAsync(new CreateUserCommand(request));
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (DuplicateEmailException)
            {
                return Conflict(new { message = "Email already exists" });
            }
        }

        /// <summary>Get by id (helper for CreatedAtAction).</summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponse>> GetById([FromRoute] Guid id)
        {
            var found = await _getById.HandleAsync(new GetUserByIdQuery(id));
            if (found == null) return NotFound();
            return Ok(found);
        }

        /// <summary>Update user.</summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserResponse>> Update([FromRoute] Guid id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var updated = await _update.HandleAsync(new UpdateUserCommand(id, request));
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (DuplicateEmailException)
            {
                return Conflict(new { message = "Email already exists" });
            }
        }

        /// <summary>Delete user (soft by default, hard delete with ?hard=true).</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] bool hard = false)
        {
            var result = await _delete.HandleAsync(new DeleteUserCommand(id, hard));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
