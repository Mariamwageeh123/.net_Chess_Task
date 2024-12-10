using Chess.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ChessLeagueContext _context;

        public ParticipantsController(ChessLeagueContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<IActionResult> GetParticipants()
        {
            var participants = await _context.Participants.ToListAsync();
            if (participants == null || participants.Count == 0)
            {
                return Ok(new { message = "No participants found." });
            }
            return Ok(new { message = "Participants retrieved successfully.", data = participants });
        }

        // POST: api/Participants
        [HttpPost("AddParticipant")]
        public async Task<IActionResult> AddParticipant([FromForm] AddParticipantDto Dto)
        {

            var Participant = new Participant
            {
                Email = Dto.Email,
                Name = Dto.Name,
            };

            _context.Participants.Add(Participant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParticipant), new { id = Participant.ParticipantId }, new { message = "Participant added successfully.", data = Participant });
        }
        [HttpPut("AddParticipantToLeague/{id}")]
        public async Task<IActionResult> AddParticipantToLeague(int id, [FromForm] LeagueNameDto Dto)
        {

            var participant = await _context.Participants.FindAsync(id);
            if (participant is null) return NotFound();

            var league = await _context.Leagues.SingleOrDefaultAsync(g => g.LeagueName == Dto.LeagueName && g.IsActive == true);
            if (league is null) return NotFound("Not Found or the league are closed");

            var participantLeague = await _context.LeagueParticipants.SingleOrDefaultAsync(P => P.ParticipantId == id && league.LeagueName == Dto.LeagueName);
            if (participant is null) return NotFound();

            var LeagueParticipant = new LeagueParticipant
            {
                LeagueId = league.LeagueId,
                ParticipantId = participant.ParticipantId
            };

            _context.Update(participant);
            _context.LeagueParticipants.Add(LeagueParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParticipant), new { id = participant.ParticipantId }, new { message = "Participant added successfully.", data = participant });
        }

        // GET: api/Participants/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParticipant(int id)
        {

            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
                return NotFound(new { message = "Participant not found." });
            
            return Ok(new { message = "Participant retrieved successfully.", data = participant  });
        }

        // PUT: api/Participants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParticipant(int id, [FromForm] AddParticipantDto Dto)
        {

            var participant = await _context.Participants.FindAsync(id);
            if (participant is null) return NotFound();


            participant.Name = Dto.Name;
            participant.Email = Dto.Email;

            _context.Update(participant);
            await _context.SaveChangesAsync();

            _context.Entry(participant).State = EntityState.Modified;

            return Ok(new { message = "Participant updated successfully." });
        }

        // DELETE: api/Participants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound(new { message = "Participant not found." });
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Participant deleted successfully." });
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.ParticipantId == id);
        }
    }
}
