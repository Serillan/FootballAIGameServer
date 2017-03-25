﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Models;
using FootballAIGame.Server.Models;

namespace FootballAIGame.Server
{
    /// <summary>
    /// Responsible for simulating a tournament.
    /// </summary>
    public class TournamentSimulator
    {
        /// <summary>
        /// Gets or sets the running tournaments.
        /// </summary>
        /// <value>
        /// The running tournaments.
        /// </value>
        public static List<TournamentSimulator> RunningTournaments { get; set; } 
            = new List<TournamentSimulator>();

        /// <summary>
        /// Gets or sets the tournament identifier of the tournament that
        /// is simulated by this instance.
        /// </summary>
        /// <value>
        /// The tournament identifier.
        /// </value>
        private int TournamentId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the tournament.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        private DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the list consisting of players that
        /// are currently in the running tournament. It is used for
        /// simulation and is empty when the tournament is not
        /// currently running.
        /// </summary>
        /// <value>
        /// The players.
        /// </value>
        private List<TournamentPlayer> Players { get; set; }

        /// <summary>
        /// Gets the time until start of the tournament.
        /// </summary>
        /// <value>
        /// The time until start.
        /// </value>
        private TimeSpan TimeUntilStart => StartTime - DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSimulator"/> class.
        /// </summary>
        /// <param name="tournament">The tournament for simulation.</param>
        public TournamentSimulator(Tournament tournament)
        {
            TournamentId = tournament.Id;
            StartTime = tournament.StartTime;
        }

        /// <summary>
        /// Plans the unstarted tournaments.
        /// </summary>
        public static void PlanUnstartedTournaments()
        {
            using (var context = new ApplicationDbContext())
            {
                var nextTournaments = context.Tournaments
                    .Where(t => t.TournamentState == TournamentState.Unstarted)
                    .ToList();

                foreach (var nextTournament in nextTournaments)
                {
                    var simulator = new TournamentSimulator(nextTournament);
                    simulator.PlanSimulation();
                }
            }
        }

        /// <summary>
        /// Plans the simulation.
        /// </summary>
        /// <returns></returns>
        public void PlanSimulation()
        {
            Console.WriteLine($"Simulation of tournament {TournamentId} is being planned!");

            Task.Run(async () =>
            {
                // sleep until 5 minutes before tournament
                if (TimeUntilStart.TotalMinutes > 5)
                {
                    await Task.Delay(TimeUntilStart - TimeSpan.FromMinutes(5));
                    Console.WriteLine($"Simulation of tournament {TournamentId} is awaken 5 minutes before start!");
                    KickInactive();
                }

                // when tournament starts
                if (TimeUntilStart.TotalSeconds > 0)
                    await Task.Delay(TimeUntilStart);

                Console.WriteLine($"Simulation of tournament {TournamentId} is being simulated!");
                KickInactive();
                await SimulateAsync();
                Console.WriteLine($"Simulation of tournament {TournamentId} ends!");
            });
        }

        /// <summary>
        /// Player will leave a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public static void LeaveRunningTournament(string playerName)
        {
            TournamentSimulator tournamentSimulator;

            lock (RunningTournaments)
            {
                tournamentSimulator = RunningTournaments
                    .SingleOrDefault(t => t.Players.Any(p => p.Player.Name == playerName));
            }

            if (tournamentSimulator == null)
                return;

            lock (tournamentSimulator.Players)
            {
                tournamentSimulator.Players.RemoveAll(p => p.Player.Name == playerName);
            }

            // improve position for players that have already ended
            using (var context = new ApplicationDbContext())
            {
                var playersWithPosition = context.TournamentPlayers
                    .Where(tp => tp.TournamentId == tournamentSimulator.TournamentId && tp.PlayerPosition != null);
                foreach (var tournamentPlayer in playersWithPosition)
                {
                    tournamentPlayer.PlayerPosition--;
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Starts the tournament simulation.
        /// </summary>
        private async Task SimulateAsync()
        {
            Tournament tournament;

            using (var context = new ApplicationDbContext())
            {
                // get tournament
                 tournament = context.Tournaments
                    .Include(t => t.Players.Select(tp => tp.Player))
                    .Include(t => t.RecurringTournament)
                    .SingleOrDefault(t => t.Id == TournamentId);

                if (tournament == null)
                {
                    Console.WriteLine($"ERROR: Cannot find tournament with id {TournamentId} in database.");
                    return;
                }

                // set state -> check if there is enough players
                if (tournament.Players.Count < tournament.MinimumNumberOfPlayers)
                {
                    tournament.TournamentState = TournamentState.NotEnoughtPlayersClosed;
                    if (tournament.RecurringTournament != null)
                    {
                        var nextTournament = CreateNextRecurring(tournament.RecurringTournament);
                        var simulator = new TournamentSimulator(nextTournament);
                        simulator.PlanSimulation();
                    }
                    context.SaveChanges();
                    return;
                }

                tournament.TournamentState = TournamentState.Running;
                lock (RunningTournaments)
                {
                    RunningTournaments.Add(this);
                }

                // get players and update their states
                Players = new List<TournamentPlayer>(); // players in tournament
                lock (Players)
                {
                    foreach (var tournamentPlayer in tournament.Players)
                        if (tournamentPlayer.Player.PlayerState == PlayerState.Idle)
                        {
                            tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentWaiting;
                            Players.Add(tournamentPlayer);
                        }
                }

                context.SaveChanges();
            }

            // while there is more than 1 player -> round simulation
            int n;
            lock (Players)
            {
                n = Players.Count;
            }

            while (n > 1)
            {
                Players = await SimulateRoundAsync(tournament);
                lock (Players)
                {
                    n = Players.Count;
                }
            }

            if (n == 1)
            {
                TournamentPlayer player;
                lock (Players)
                {
                    player = Players.FirstOrDefault();
                }

                if (player != null)
                {
                    player.PlayerPosition = 1;
                    player.Player.WonTournaments++;
                    player.Player.PlayerState = PlayerState.Idle;
                }

                await SavePlayersAsync(Players);
            }

            // set state to finished
            using (var context = new ApplicationDbContext())
            {
                tournament = context.Tournaments
                    .Include(t => t.RecurringTournament)
                    .Single(t => t.Id == TournamentId);
                tournament.TournamentState = TournamentState.Finished;
                context.SaveChanges();
            }

            // if it's recurring then plan new one
            if (tournament.RecurringTournament != null)
            {
                var nextTournament = CreateNextRecurring(tournament.RecurringTournament);
                var simulator = new TournamentSimulator(nextTournament);
                simulator.PlanSimulation();
            }
        }

        /// <summary>
        /// Simulates the round. Returns the list of advancing players.
        /// </summary>
        /// <param name="tournament">The tournament.</param>
        /// <returns>The list of advancing players.</returns>
        private async Task<List<TournamentPlayer>> SimulateRoundAsync(Tournament tournament)
        {
            Console.WriteLine($"Tournament {TournamentId} : Simulating round.");

            var advancingPlayers = new List<TournamentPlayer>();
            var fightingPlayers = new List<TournamentPlayer>();
            var simulationTasks = new List<Task<MatchSimulator>>();

            lock (Players)
            {
                // first round byes (some players may proceed directly to second round 
                // if number of players is not a power of two)
                var isNumOfPlayersTwoPower = (Players.Count & (Players.Count - 1)) == 0;
                if (!isNumOfPlayersTwoPower)
                {
                    var nextPowerOfTwo = tournament.Players.Count;
                    while ((nextPowerOfTwo & (nextPowerOfTwo - 1)) != 0) // while it's not a power of two
                        nextPowerOfTwo++;

                    // number of players without opponent (best will be chosen)
                    var numOfSkippingPlayers = nextPowerOfTwo - Players.Count;

                    var skippingPlayers = Players
                        .OrderByDescending(p => p.Player.Score)
                        .Take(numOfSkippingPlayers);

                    advancingPlayers.AddRange(skippingPlayers);
                }

                fightingPlayers.AddRange(Players.Where(p => !advancingPlayers.Contains(p)));

                TournamentPlayer firstPlayer = null;

                foreach (var tournamentPlayer in fightingPlayers)
                {
                    if (firstPlayer == null)
                        firstPlayer = tournamentPlayer;
                    else // start new match
                    {
                        Task<MatchSimulator> simulationTask;

                        SimulationManager.Instance.StartMatch(firstPlayer.Player.Name, firstPlayer.PlayerAi,
                            tournamentPlayer.Player.Name, tournamentPlayer.PlayerAi, out simulationTask, tournament.Id);
                        
                        // update player states
                        firstPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                        tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                        
                        // add match to matches
                        simulationTasks.Add(simulationTask);
                        firstPlayer = null;
                    }
                }
            }

            await SavePlayersAsync(fightingPlayers); // update states

            await Task.WhenAll(simulationTasks);

            var matches = new List<MatchSimulator>();
            foreach (var simulationTask in simulationTasks)
            {
                matches.Add(await simulationTask);
            }

            lock (Players)
            {
                Players.ForEach(p => p.Player.PlayerState = PlayerState.PlayingTournamentWaiting);

                // get looser position number
                var exp = 1;
                while (exp*2 < Players.Count)
                    exp *= 2;
                var looserPos = exp + 1; // ex. from 8. (2^3) to 5. (2^2+1) player they will have position 5

                foreach (var matchSimulator in matches)
                {
                    TournamentPlayer winner, looser;

                    if (matchSimulator.MatchInfo.Winner != null)
                    {
                        string winnerPlayerName;
                        string looserPlayerName;

                        if (matchSimulator.MatchInfo.Winner == Team.FirstPlayer)
                        {
                            winnerPlayerName = matchSimulator.AI1Communicator.PlayerName;
                            looserPlayerName = matchSimulator.AI2Communicator.PlayerName;
                        }
                        else
                        {
                            winnerPlayerName = matchSimulator.AI2Communicator.PlayerName;
                            looserPlayerName = matchSimulator.AI1Communicator.PlayerName;
                        }


                        winner = Players.FirstOrDefault(p => p.Player.Name == winnerPlayerName);
                        looser = Players.FirstOrDefault(p => p.Player.Name == looserPlayerName);
                    }
                    else
                    {
                        var player1 =
                            Players.FirstOrDefault(p => p.Player.Name == matchSimulator.AI1Communicator.PlayerName);
                        var player2 =
                            Players.FirstOrDefault(p => p.Player.Name == matchSimulator.AI2Communicator.PlayerName);
                        var rndWinner = MatchSimulator.Random.Next(2);

                        // in tournament winner is chosen randomly in case of draw !
                        winner = rndWinner == 0 ? player1 : player2;
                        looser = rndWinner == 0 ? player2 : player1;
                    }

                    if (winner == null && looser != null)
                    {
                        winner = looser;
                        looser = null;
                    }

                    if (winner != null)
                        advancingPlayers.Add(winner);
                    if (looser == null)
                        continue;
                    looser.PlayerPosition = looserPos;
                    looser.Player.PlayerState = PlayerState.Idle;
                }

                // remove all skipping players that left during round
                // don't save those that have already left
                fightingPlayers.RemoveAll(p => !Players.Contains(p));
            }

            await SavePlayersAsync(fightingPlayers);

            return advancingPlayers;
        }

        /// <summary>
        /// Creates the next tournament from the specified <see cref="RecurringTournament"/>.
        /// </summary>
        /// <param name="tournament">The recurring tournament.</param>
        /// <returns>The created tournament.</returns>
        private static Tournament CreateNextRecurring(RecurringTournament tournament)
        {
            using (var context = new ApplicationDbContext())
            {
                var reccuringTournament = context.RecurringTournaments
                    .Include(t => t.Tournaments)
                    .SingleOrDefault(t => t.Id == tournament.Id);

                if (reccuringTournament == null)
                    return null;

                var lastTournamentTime = reccuringTournament.Tournaments.Max(t => t.StartTime);
                var nextTournament = new Tournament(reccuringTournament,
                    lastTournamentTime + TimeSpan.FromMinutes(reccuringTournament.RecurrenceInterval));
                context.Tournaments.Add(nextTournament);
                context.SaveChanges();

                return nextTournament;
            }
        }

        /// <summary>
        /// Saves players states and tournament positions.
        /// </summary>
        /// <param name="players">The players.</param>
        private async Task SavePlayersAsync(IEnumerable<TournamentPlayer> players)
        {
            using (var context = new ApplicationDbContext())
            {
                var dbPlayers = await context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(t => t.TournamentId == TournamentId).ToListAsync();

                lock (Players)
                {
                    foreach (var player in players)
                    {
                        var dbPlayer = dbPlayers.Single(p => p.UserId == player.UserId);
                        dbPlayer.Player.PlayerState = player.Player.PlayerState;
                        dbPlayer.PlayerPosition = player.PlayerPosition;
                        dbPlayer.Player.WonTournaments = player.Player.WonTournaments;
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Kicks the inactive players from the tournament.
        /// </summary>
        private void KickInactive()
        {
            using (var context = new ApplicationDbContext())
            {
                var tournamentPlayers = context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(tp => tp.TournamentId == TournamentId)
                    .ToList();
                var playersToBeRemoved = new List<TournamentPlayer>();

                foreach (var tournamentPlayer in tournamentPlayers)
                {
                    if (tournamentPlayer.Player.ActiveAis == null)
                    {
                        playersToBeRemoved.Add(tournamentPlayer);
                        continue;
                    }
                    var activeAis = tournamentPlayer.Player.ActiveAis.Split(';');
                    if (!activeAis.Contains(tournamentPlayer.PlayerAi))
                        playersToBeRemoved.Add(tournamentPlayer);
                }

                foreach (var tournamentPlayer in playersToBeRemoved)
                {
                    Console.WriteLine($"Removing {tournamentPlayer.Player.Name} from tournament " +
                                        $"{TournamentId}, because he doesn't have {tournamentPlayer.PlayerAi} active!");
                }

                context.TournamentPlayers.RemoveRange(playersToBeRemoved);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Closes the running tournaments.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void CloseRunningTournaments(ApplicationDbContext context)
        {
            lock (RunningTournaments)
            {
                var runningTournaments = context.Tournaments
                    .Include(t => t.RecurringTournament)
                    .Where(t => t.TournamentState == TournamentState.Running);
                    
                foreach (var runningTournament in runningTournaments)
                {
                    runningTournament.TournamentState = TournamentState.ErrorClosed;
                    if (runningTournament.RecurringTournament != null)
                        CreateNextRecurring(runningTournament.RecurringTournament);
                }
            }
        }

    }
}