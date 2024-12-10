using System.Text.Json.Serialization;

namespace Chess.Models
{
    public class League
    {
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string? WennerName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public ICollection<LeagueParticipant> LeagueParticipants { get; set; }
    }
}