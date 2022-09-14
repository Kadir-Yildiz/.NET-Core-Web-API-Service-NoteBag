using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBag.ModelDTO;

namespace NoteBag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public NotesController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Note>> GetNotes()
        {
            return  _db.Notes.OrderByDescending(x=> x.CreatedTime).ToList();
        }
        [HttpGet("{id}")]
        public ActionResult<Note> GetNote(int id)
        {
            var note = _db.Notes.Find(id);
            if (note == null)
            {
                return NotFound();
            }
            return note;
        }
        [HttpPut("{id}")]
        public IActionResult PutNote(int id, PutNoteDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }
            Note note = _db.Notes.Find(id);
            note.Title = dto.Title;
            note.Content = dto.Content;
            note.ModifiedTime = DateTimeOffset.Now;
            _db.Notes.Update(note);
            try
            {
                 _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Notes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(note);
        }

        [HttpPost]
        public ActionResult<Note> PostNote(PostNoteDto dto)
        {
            Note note = new Note();
            note.Title = dto.Title;
            note.Content = dto.Content;
            _db.Notes.Add(note);
            _db.SaveChanges();

            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNote(int id)
        {
            var note =  _db.Notes.Find(id);
            if (note == null)
            {
                return NotFound();
            }

            _db.Notes.Remove(note);
            _db.SaveChanges();

            return Ok();
        }

    }
}
