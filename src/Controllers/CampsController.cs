using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper,
            LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampModel>>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsAsync(includeTalks);

                return Ok(_mapper.Map<IEnumerable<CampModel>>(results));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker, includeTalks);

                if (result == null) return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CampModel>>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return Ok(_mapper.Map<IEnumerable<CampModel>>(results));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existingCamp = await _campRepository.GetCampAsync(model.Moniker);

                if(existingCamp != null) { return BadRequest("Moniker in use");}

                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker.");
                }

                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);

                if (oldCamp == null) { return NotFound("Moniker do not exist."); }

                _mapper.Map(model, oldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);

                if (oldCamp == null) { return NotFound("Moniker do not exist."); }

                _campRepository.Delete(oldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed to delete the camp.");
        }
    }
}
