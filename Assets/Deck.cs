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
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
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
    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public HandRank EvaluateHand(List<Card> hand)
    {
        if (IsRoyalFlush(hand)) return HandRank.RoyalFlush;
        if (IsStraightFlush(hand)) return HandRank.StraightFlush;
        if (IsFourOfAKind(hand)) return HandRank.FourOfAKind;
        if (IsFullHouse(hand)) return HandRank.FullHouse;
        if (IsFlush(hand)) return HandRank.Flush;
        if (IsStraight(hand)) return HandRank.Straight;
        if (IsThreeOfAKind(hand)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(hand)) return HandRank.TwoPair;
        if (IsOnePair(hand)) return HandRank.OnePair;
        return HandRank.HighCard;
    }

    private bool IsRoyalFlush(List<Card> hand)
    {
        return IsStraightFlush(hand) && hand.All(card => card.Rank >= Rank.Ten);
    }

    private bool IsStraightFlush(List<Card> hand)
    {
        return IsFlush(hand) && IsStraight(hand);
    }

    public static bool IsFourOfAKind(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Any(group => group.Count() == 4);
    }

    public static bool IsFullHouse(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Any(group => group.Count() == 3) && rankGroups.Any(group => group.Count() == 2);
    }

    public static bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(card => card.Suit).Count() == 1;
    }

    public static bool IsStraight(List<Card> hand)
    {
        var sortedRanks = hand.Select(card => (int)card.Rank).OrderBy(rank => rank).ToList();
        if (sortedRanks.Last() == (int)Rank.Ace && sortedRanks.First() == (int)Rank.Two)
        {
            // Handle A-2-3-4-5 as a valid straight (wheel)
            sortedRanks.Remove(sortedRanks.Last());
            sortedRanks.Insert(0, 1);
        }
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

    private bool IsTwoPair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Rank);
        return rankGroups.Any(group => group.Count() == 2);
    }

    private static void Shuffle<T>(IList<T> list)
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