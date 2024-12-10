
using System.ComponentModel.DataAnnotations;

public class CongratulationMail
{
    [Key]
    public int MailId { get; set; }
    public int ChampionId { get; set; }
    public int LeagueId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
    public string EmailContent { get; set; }
}
