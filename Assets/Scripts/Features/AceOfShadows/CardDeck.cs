using System.Collections.Generic;
using System;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public static class CardDeck
    {
        public const int SuitCount = 4;
        public const int CardsPerSuit = 36;
        public const int Count = SuitCount * CardsPerSuit;

        public static List<CardData> Create()
        {
            var deck = new List<CardData>(Count);

            for (var suitIndex = 0; suitIndex < SuitCount; suitIndex++)
            {
                for (var number = 1; number <= CardsPerSuit; number++)
                    deck.Add(new CardData(number, (CardSuit)suitIndex));
            }

            return deck;
        }

        public static void Shuffle(IList<CardData> deck, Random random = null)
        {
            random ??= new Random();

            for (var index = deck.Count - 1; index > 0; index--)
            {
                var swapIndex = random.Next(index + 1);
                (deck[index], deck[swapIndex]) = (deck[swapIndex], deck[index]);
            }
        }
    }
}