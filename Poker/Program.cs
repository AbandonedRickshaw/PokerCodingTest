using System;
using System.Collections.Generic;
using System.Linq;
using Armstrong.Poker;
using Armstrong.Poker.Models;

namespace Poker.UI
{
	class Program
    {
		static void Main(string[] args)
		{
			try
			{
				// create a new game with the players
				var game = GameBuilder(args);

				// write the winner(s)' ids to the output
				Console.WriteLine(string.Join(" ", game.Winners.Select(e => e.Id)));
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				Environment.Exit(-1);
			}
		}

		private static Game GameBuilder(string[] input)
		{
			int playerCount = 0;
			var playerInfos = input
				// args can't be an empty array
				.ThrowIfTrue(e => e.Length == 0, new ArgumentException("Length is 0", nameof(input)))

				// first element must be an int (number of players)
				.ThrowIfFalse(e => int.TryParse(input[0], out playerCount), new ArgumentException("First element must be an integer", nameof(input)))

				// total args length must match the number of players + 1
				.ThrowIfTrue(e => e.Length != playerCount + 1, new ArgumentException("Incorrect number of players", nameof(input)))

				// skip to the next element
				.Skip(1)

				// separate each remaining element into an array
				.Select(e => e.Split(' '));

			// playerInfo is an array in the format [playerId] [Card1] [Card2] [CardN].  We convert this into a player and add it to Players
			var players = new List<Player>();
			foreach (var playerInfo in playerInfos)
			{
				players.Add(new Player(int.Parse(playerInfo[0]), playerInfo.Skip(1).Select(e => Card.FromSuitValueString(e))));
			}

			var cardCount = players.First().Hand.Cards.Count();
			return new Game(players)
				// make sure everyone has the same number of cards
				.ThrowIfFalse(e => e.Players.All(player => player.Hand.Cards.Count() == cardCount), new ArgumentException("Everyone must have the same number of cards!", nameof(input)));
		}
	}
}
