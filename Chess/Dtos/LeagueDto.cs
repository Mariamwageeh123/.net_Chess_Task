namespace Chess.Dtos
{
    public class LeagueDto
    {
        public int LeagueId { get; set; }

        public List<LeagueParticipantDto> LeagueParticipants { get; set; }
    }
}
