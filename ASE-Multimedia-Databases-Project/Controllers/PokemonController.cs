using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using ASE_Multimedia_Databases_Project.Contexts;
using ASE_Multimedia_Databases_Project.Models;
using ASE_Multimedia_Databases_Project.Repositories;

namespace ASE_Multimedia_Databases_Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PokemonController : ControllerBase
    {
        IPokemonRepository pokemonRepository;

        public PokemonController(IPokemonRepository pokemonRepository)
        {
            this.pokemonRepository = pokemonRepository;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                List<PokemonModel> pokemons = pokemonRepository.FindMany();

                return Ok(pokemons);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    message = e.Message
                });
            }

        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if (id <= 0) return BadRequest();

            try
            {
                PokemonModel pokemon = pokemonRepository.FindOne(id);

                if (pokemon == null)
                {
                    return NotFound(new {
                        message = "Pokemon with id " + id + " does not exist" 
                    });
                }

                return Ok(pokemon);
            } catch(Exception e)
            {
                return StatusCode(500, new
                {
                    message = e.Message
                });
            }
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetImage(int id)
        {
            if (id <= 0) return BadRequest();

            try
            {
                byte[] imageBytes = await pokemonRepository.FindImage(id);
                return File(imageBytes, "image/jpeg");
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    message = e.Message
                });
            }
        }

        [HttpGet("sound/{id}")]
        public async Task<ActionResult> GetSound(int id)
        {
            if(id <= 0) return BadRequest();

            try
            {
                byte[] soundBytes = await pokemonRepository.FindSound(id);
                return File(soundBytes, "audio/mp3");
            } catch(Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PokemonDTO pokemonDto)
        {
            if (pokemonDto.Name.Trim().Length == 0 || pokemonDto.Generation <= 0) 
                return BadRequest();

            int id;

            try
            {
                id = await pokemonRepository.Create(pokemonDto);
            } catch(Exception e)
            {
                return StatusCode(500, new {
                    message = e.Message
                });
            }

            return StatusCode(201, new { id });
        }

        [HttpPost("similar")]
        public async Task<ActionResult> GetSimilarPokemons([FromForm] FindSimilarPokemonsDTO dto)
        {
            try
            {
                List<PokemonModel> pokemons = await pokemonRepository.FindSimilarPokemons(dto);
                return Ok(pokemons);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
