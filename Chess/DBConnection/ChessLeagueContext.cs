using Chess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class ChessLeagueContext : DbContext
{
    public ChessLeagueContext(DbContextOptions<ChessLeagueContext> options) : base(options) { }

    public DbSet<Participant> Participants { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<LeagueParticipant> LeagueParticipants { get; set; }
    public DbSet<CongratulationMail> CongratulationMails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeagueParticipant>()
            .HasKey(lp => new { lp.LeagueId, lp.ParticipantId });

        modelBuilder.Entity<LeagueParticipant>()
            .HasOne(lp => lp.League)
            .WithMany(l => l.LeagueParticipants)
            .HasForeignKey(lp => lp.LeagueId);

        modelBuilder.Entity<LeagueParticipant>()
            .HasOne(lp => lp.Participant)
            .WithMany(p => p.LeagueParticipants)
            .HasForeignKey(lp => lp.ParticipantId);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Player1)
            .WithMany()
            .HasForeignKey(m => m.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Player2)
            .WithMany()
            .HasForeignKey(m => m.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Winner)
            .WithMany(p => p.MatchesWon)
            .HasForeignKey(m => m.WinnerId);
    }
}
