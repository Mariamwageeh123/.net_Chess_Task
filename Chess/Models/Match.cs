using System.Text.Json.Serialization;

public class Match
{
    public int MatchId { get; set; }
    public int Player1Id { get; set; }
    [JsonIgnore]
    public Participant Player1 { get; set; }
    public int Player2Id { get; set; }
    [JsonIgnore]
    public Participant Player2 { get; set; }
    public DateTime MatchTime { get; set; }
    public int? WinnerId { get; set; }
    [JsonIgnore]
    public Participant Winner { get; set; }
    public int RoundNumber { get; set; }
    public int Result { get; set; }
}
