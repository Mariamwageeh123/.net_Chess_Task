using Chess.Models;
using System.Text.Json.Serialization;

public class LeagueParticipant
{
    public int LeagueId { get; set; }
    [JsonIgnore]
    public League League { get; set; }
    public int ParticipantId { get; set; }
    [JsonIgnore]
    public Participant Participant { get; set; }
}
