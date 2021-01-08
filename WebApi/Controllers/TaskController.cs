using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Abstractions.Services;
using Domain.Commands;
using Domain.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskServices _taskService;

        public TasksController(ITaskServices taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateTaskCommandResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _taskService.CreateTaskCommandHandler(command);

                return Created($"/api/tasks/{result.Payload.Id}", result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //[HttpPut("{id}")]
        //[ProducesResponseType(typeof(UpdateMemberCommandResult), StatusCodes.Status200OK)]
        //public async Task<IActionResult> Update(Guid id, UpdateMemberCommand command)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var result = await _memberService.UpdateMemberCommandHandler(command);

        //        return Ok(result);
        //    }
        //    catch (NotFoundException<Guid>)
        //    {
        //        return NotFound();
        //    }            
        //}

        [HttpGet]
        [ProducesResponseType(typeof(GetAllTasksQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _taskService.GetAllTasksQueryHandler();

            return Ok(result);
        }
    }
}
