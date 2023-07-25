using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Armstrong.Poker.Models
{
	/// <summary>
	/// Represents a collection of players, ranked by winner
	/// </summary>
	public class Game
	{
		/// <summary>
		/// Creates a new game with the given players 
		/// </summary>
		/// <param name="players"></param>
		public Game(IEnumerable<Player> players) =>
			Players = players;

		/// <summary>
		/// Da playas
		/// </summary>
		public IEnumerable<Player> Players { get; }

		/// <summary>
		/// Returns the winner(s) with the highest ranked hand(s).
		/// </summary>
		public IEnumerable<Player> Winners =>
			Players
				.GroupBy(e => e.Hand, Hand.EqualityComparer)
				.OrderByDescending(e => e.Key, Hand.Comparer)
				.First();
	}
}
