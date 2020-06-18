using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/[controller]")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper,
            LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TalksModel>>> Get(string moniker)
        {
            try
            {
                var talks = await _campRepository.GetTalksByMonikerAsync(moniker);

                if (talks == null) return NotFound();
                return Ok(_mapper.Map<IEnumerable<TalksModel>>(talks));
            }
            catch (System.Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalksModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, id);

                if (talk == null) return NotFound();
                return Ok(_mapper.Map<TalksModel>(talk));
            }
            catch (System.Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

        }
    }
}
