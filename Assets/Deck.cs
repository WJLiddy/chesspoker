using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck
{
    public enum Suit
    {
        Hearts = 0,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        // maybe 7?
        Eight = 8,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }

    public static bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(card => card.Suit).Count() == 1;
    }

    public static bool IsStraight(List<Card> hand)
    {
        var sortedRanks = hand.Select(card => (int)card.Rank).OrderBy(rank => rank).ToList();
        // no need for wheel in short deck
        for (int i = 1; i < sortedRanks.Count; i++)
        {
            if (sortedRanks[i] != sortedRanks[i - 1] + 1)
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsThreeOfAKind(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Any(group => group.Count() == 3);
    }

    public static bool IsTwoPair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }
    public static bool IsOnePair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Any(group => group.Count() == 2);
    }
    /**
private bool IsRoyalFlush(List<Card> hand)
{
return IsStraightFlush(hand) && hand.All(card => card.Rank >= Rank.Ten);
}

private bool IsStraightFlush(List<Card> hand)
{
return IsFlush(hand) && IsStraight(hand);
}    public static bool IsFourOfAKind(List<Card> hand)
{
    var rankGroups = hand.GroupBy(card => card.Rank);
    return rankGroups.Any(group => group.Count() == 4);
}

public static bool IsFullHouse(List<Card> hand)
{
    var rankGroups = hand.GroupBy(card => card.Rank);
    return rankGroups.Any(group => group.Count() == 3) && rankGroups.Any(group => group.Count() == 2);
}
    */




    public static void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<Card> GetDeck()
    {
        var deck = new List<Card>();
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                deck.Add(new Card(suit, rank));
            }
        }
        Shuffle(deck);
        return deck;
    }
}