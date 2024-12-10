

using Chess.Models;
using System.Text.RegularExpressions;

public class Participant
{
    public int ParticipantId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public ICollection<LeagueParticipant> LeagueParticipants { get; set; }
    public ICollection<Match> MatchesWon { get; set; }
}
