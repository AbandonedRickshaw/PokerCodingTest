using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Armstrong.Poker.Models
{
	/// <summary>
	/// Represents a player and their hand
	/// </summary>
	public class Player
	{
		public Player(int id, IEnumerable<Card> dealtCards) =>
			(Id, Hand) = (id, new Hand(dealtCards));

		/// <summary>
		/// Player's id
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The player's hand
		/// </summary>
		public Hand Hand { get; }
	}
}
