using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Armstrong.Poker.Models
{
	/// <summary>
	/// Represents a playing card
	/// </summary>
	public class Card
		:IComparable<Card>
	{
		/// <summary>
		/// Valid card values (ordered).
		/// </summary>
		private const string Values = "23456789TJQKA";

		/// <summary>
		/// Valid suit values
		/// </summary>
		private const string Suits = "hdsc";

		public Card(char value, char suit)
		{
			Suit = suit
				.ThrowIfFalse(e => Suits.Contains(suit.ToString()), nameof(suit));

			Value = value
				.ThrowIfFalse(e => Values.Contains(value.ToString()), nameof(value));
		}
			

		/// <summary>
		/// The Suit of the card
		/// </summary>
		public char Suit { get; }

		/// <summary>
		/// The value of the card
		/// </summary>
		public char Value { get; }

		/// <summary>
		/// The numeric value of the card
		/// </summary>
		public int NumericValue => 2 + Values.IndexOf(Value);

		/// <summary>
		/// Returns a 2-char string of suit+value
		/// </summary>
		/// <returns></returns>
		public override string ToString() => 
			string.Join("", Value, Suit);

		#region Helpers
		/// <summary>
		/// Converts a string that represent suits and values into a Card object
		/// </summary>
		/// <param name="suitValueString">A 2 char string whose first character represents a suite, 
		/// and whose second character represents a value.<see cref="FromSuitValueChars(char, char)" for more details./>
		/// </param>
		public static Card FromSuitValueString(string suitValueString)
		{
			suitValueString
				.ThrowIfNullOrWhitespace(suitValueString)       //Suit value string must not be empty or whitespace
				.ThrowIfTrue(e => e.Length != 2);               //and must have a length of exactly 2

			return new Card(suitValueString[0], suitValueString[1]);
		}

		#endregion Helpers

		#region IComparable<Card> implementation
		public int CompareTo(Card other) =>
			ValueComparer.Compare(Value, other.Value);
		#endregion IComparable<Card> implementation

		#region Comparers
		/// <summary>
		/// This comparer is used to compare card char values according to <see cref="Values"/>
		/// </summary>
		public static IComparer<char> ValueComparer => new CardValueComparer();

		private class CardValueComparer
			: IComparer<char>
		{
			string order = Values;	// "23456789TJQKA";

			public int Compare(char x, char y) =>
				order.IndexOf(x).CompareTo(order.IndexOf(y));
		}
		#endregion Comparers
	}
}
