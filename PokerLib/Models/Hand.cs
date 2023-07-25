using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Armstrong.Poker.Models
{
	/// <summary>
	/// Represents a ranked hand
	/// </summary>
	public class Hand
	{
		/// <summary>
		/// Hand types in ascending order of strength based on rules found at <see href="https://en.wikipedia.org/wiki/Three_card_poker"/>
		/// </summary>
		public enum HandTypeEnum { HighCard, Pair, Flush, Straight, ThreeOfAKind, StraightFlush }

		/// <summary>
		/// Set of cards that proves the evaluated _handType
		/// </summary>
		private IEnumerable<Card> _evidence = default;

		/// <summary>
		/// The rank (type) of the hand evaluated
		/// </summary>
		private HandTypeEnum _handType = default;

		/// <summary>
		/// A shared instance of the Card ValueComparer so we don't have to keep instantiating it
		/// </summary>
		private static readonly IComparer<char> _cardValueComparer = Card.ValueComparer;

		/// <summary>
		/// Evaluates the dealt cards and creates a ranked hand
		/// </summary>
		/// <param name="dealtCards"></param>
		public Hand(IEnumerable<Card> dealtCards)
		{
			// save the dealt cards
			Cards = dealtCards;

			// now let's evaluate the hand
			// 1. We need to check for both straight and flush so we can determine if we have a straight flush
			if (TryStraight() & TryFlush())
			{
				// set the rank to straight flush and return.
				_handType = HandTypeEnum.StraightFlush;
				return;
			}

			// if it was a straight or flush, stop evaluating
			if (_handType >= HandTypeEnum.Flush)
				return;

			// 2. try the other types (in descending order of rank) and stop once we have a successful evaluation (TryHighCard always succeeds)
			_ = TryThreeOfAKind() || TryPair() || TryHighCard();
		}

		/// <summary>
		/// Set of cards that were dealt
		/// </summary>
		public IEnumerable<Card> Cards = default;

		#region Evaluators
		/// <summary>
		/// Sets up the hand and returns true if there are three of a kind
		/// </summary>
		private bool TryThreeOfAKind()
		{
			// Picks the best triples in a hand
			var bestTriplet = GetBestGroup(3);

			if (bestTriplet is null)
				return false;

			_handType = HandTypeEnum.ThreeOfAKind;
			_evidence = bestTriplet;
			return true;
		}

		/// <summary>
		/// Sets up the hand and returns true if this is a straight
		/// </summary>
		private bool TryStraight()
		{
			var cards = Cards.OrderBy(e => e.Value, _cardValueComparer);
			int prev = cards.First().NumericValue - 1;
			foreach (var card in cards)
			{
				if (card.NumericValue != ++prev)
					return false;
			}
			_handType = HandTypeEnum.Straight;
			_evidence = cards;
			return true;
		}

		/// <summary>
		/// Sets up the hand and returns true if this is a flush
		/// </summary>
		private bool TryFlush()
		{
			if (!Cards.All(e => e.Suit == Cards.First().Suit))
				return false;

			_handType = HandTypeEnum.Flush;

			_evidence = Cards.OrderBy(e => e.Value, _cardValueComparer);
			return true;
		}

		/// <summary>
		/// Sets up the hand and returns true if there is at least one pair
		/// </summary>
		private bool TryPair()
		{
			// Picks the best pair in a hand
			var bestPair = GetBestGroup(2);
			if (bestPair is null)
				return false;

			_handType = HandTypeEnum.Pair;
			_evidence = bestPair;
			return true;
		}

		private bool TryHighCard()
		{
			_handType = HandTypeEnum.HighCard;
			_evidence = new List<Card>(new Card[] { Cards.Max() });
			return true;
		}

		/// <summary>
		/// Creates groups of a set number of cards having the same value, then returns the group whose key is the highest value.
		/// </summary>
		/// <param name="cards">Cards to group</param>
		/// <param name="groupSize">The number of cards required to have the same value in order to make a group</param>
		/// <remarks>
		/// This is a bit over-engineered for 3-card, but it's reusable for both pairs and triplets.  Also it would handle more than 3 cards,
		/// with the possibility of multiple pairs, triplets, quads, etc.
		/// </remarks>
		private IEnumerable<Card> GetBestGroup(int groupSize)
		{
			// If there are [groupSize] cards with the same value in [cards],
			// group them together, then return the group with the highest value.
			return Cards.Where(e => Cards.Count(f => f.Value == e.Value) == groupSize)
				.GroupBy(e => e.Value)
				.OrderByDescending(e => e.Key, _cardValueComparer)
				.FirstOrDefault();
		}
		#endregion Evaluators

		#region Comparers
		/// <summary>
		/// Returns an instance of HandComparer
		/// </summary>
		public static IComparer<Hand> Comparer => new HandComparer();

		/// <summary>
		/// Returns an instance of HandEqualityComparer
		/// </summary>
		public static IEqualityComparer<Hand> EqualityComparer => new HandEqualityComparer();

		/// <summary>
		/// This comparer is used to compare two hands and determine which is better based on the rules at
		/// <see href="https://en.wikipedia.org/wiki/Three_card_poker"/>
		/// </summary>
		private class HandComparer
			: IComparer<Hand>
		{
			public int Compare(Hand x, Hand y)
			{
				// compare ranks
				var typeCompare = x._handType.CompareTo(y._handType);

				// if one out-ranks the other, we're done
				if (typeCompare != 0)
					return typeCompare;

				// For everything else, we compare highest values
				for (int i = x._evidence.Count()-1; i >= 0; --i)
				{
					var comp = _cardValueComparer.Compare(x._evidence.ElementAt(i).Value, y._evidence.ElementAt(i).Value);
					if (comp != 0)
						return comp;
				}

				// if we're here we've tied.  If we tied on pairs we need to check the non-pair cards for a tie breaker.
				// NOTE:  In 3-Card, there really can only ever be 1 card not in evidence, but if we ever add more
				// than 3 cards this would handle it
				if (x._handType == HandTypeEnum.Pair)
				{
					var x1 = x.Cards.Except(x._evidence).OrderByDescending(e => e.Value, _cardValueComparer);
					var y1 = y.Cards.Except(y._evidence).OrderByDescending(e => e.Value, _cardValueComparer);
					for (int i = 0; i < x1.Count(); i++)
					{
						var comp = _cardValueComparer.Compare(x1.ElementAt(i).Value, y1.ElementAt(i).Value);
						if (comp != 0)
							return comp;
					}
				}
				// we have a tie
				return 0;
			}
		}

		/// <summary>
		/// This comparer is used to determine if two hands of the same type are ties
		/// </summary>
		private class HandEqualityComparer
			:IEqualityComparer<Hand>
		{
			public bool Equals(Hand x, Hand y) =>
				Comparer.Compare(x, y) == 0;

			public int GetHashCode(Hand obj)
			{
				unchecked
				{
					int result = obj._handType.GetHashCode();
					foreach (var card in obj.Cards.OrderByDescending(e=>e.Value, _cardValueComparer))
					{
						result = (result * 397) ^ card.Value;
					}
					return result;
				}
			}
		}
		#endregion Comparers
	}
}
