using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext dbContext, IMapper mapper)
        {
            this._logger = logger;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = dbContext.Genres.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var genres = await queryable.OrderBy(x=>x.Name).Paginate(paginationDTO).ToListAsync();
            //var genres= await dbContext.Genres.ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
        }
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<GenreDTO>> Get(int Id)
        {
            var genre = await dbContext.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if(genre == null)
            {
                return NotFound();
            }
            return mapper.Map<GenreDTO>(genre);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre= await dbContext.Genres.FirstOrDefaultAsync(x=>x.Id == id);
            if(genre==null)
            {
                return NotFound();
            }
            genre = mapper.Map(genreCreationDTO, genre);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre= mapper.Map<Genre>(genreCreationDTO);
            dbContext.Add(genre);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await dbContext.Genres.AnyAsync(x => x.Id == id);
            if(!exists)
            {
                return NotFound();
            }
            dbContext.Remove(new Genre() { Id = id });
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
