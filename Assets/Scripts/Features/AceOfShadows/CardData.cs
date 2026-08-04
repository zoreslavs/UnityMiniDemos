namespace UnityMiniDemos.Features.AceOfShadows
{
    public readonly struct CardData
    {
        public CardData(int number, CardSuit suit)
        {
            Number = number;
            Suit = suit;
        }

        public int Number { get; }
        public CardSuit Suit { get; }
    }
}