using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardHandler : MonoBehaviour
{
    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
    
    public enum Value
    {
        Two, 
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

    [Serializable]
    public struct Card
    {
        public Suit m_suit;
        public Value m_value;

        public Card( Suit s, Value v)
        {
            m_suit = s;
            m_value = v;
        }
    }

    public struct HandValue
    {
        public HandType m_type;
        public Value m_highCard;
    }
    
    [TestButton("Evaluate Hand", "EvaluateHand", isActiveInEditor = true)]
    [ProgressBar(hideWhenZero = true, label = "procGenFeedback")]
    public float procgenProgress = -1;

    public int m_test = 0;
    public List<Suit> m_suits = new List<Suit>();
    public List<Value> m_values = new List<Value>();

    
    private void Start()
    {
        // Initialise the deck
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void DrawCards()
    {
    }

    public void EvaluateHand()
    {
        Debug.Log( "Attempting to evaluate static hand"  );
        
        List<Card> cards = new List<Card>();
        for( int i = 0; i < m_suits.Count; ++i )
        {
            if( i < m_values.Count )
            {
                cards.Add( new Card( m_suits[i], m_values[i]) );
            }
        }

        EvaluateCards( cards );
    }

    public static HandValue EvaluateCards( List<Card> cards )
    {
        HandValue result = new HandValue();
        Debug.Log( "Attempting to evaluate static hand of size " + cards.Count );

        List<Card> m_pairs = new List<Card>();
        List<Card> m_threes = new List<Card>();
        List<Card> m_fours = new List<Card>();

        int index = 0;
        // Only check for n of a kind
        while( index < cards.Count - 1 )
        {
            Card c = cards[index];
            int sameCount = 1;
            for( int i = index + 1; i < cards.Count; ++i )
            {
                if( cards[i].m_value == c.m_value )
                {
                    ++sameCount;
                }
            }

            if( sameCount > 1 )
            {
                ref List<Card> cardList = ref m_pairs;
                if( sameCount == 3 ) cardList = ref m_threes;
                else if( sameCount == 4 ) cardList = ref m_fours;

                cardList.AddRange( cards.Where( card => card.m_value == c.m_value ) );
                cards.RemoveAll( card => card.m_value == c.m_value );
            }

            ++index;
        }

        bool isTwoPair = m_pairs.Count == 4;
        bool isFullHouse = m_pairs.Count == 2 && m_threes.Count == 3;

//        if( isFullHouse ) Debug.Log( "Full House " + m_threes[0].m_value + " " + m_pairs[0].m_value );
//        else if( isTwoPair ) Debug.Log( "Two Pair " + m_pairs[0].m_value + " " + m_pairs[2].m_value );
//        else if( m_fours.Count > 0 )  Debug.Log( "Four of a Kind " + m_fours[0].m_value );
//        else if( m_threes.Count > 0 )  Debug.Log( "Three of a Kind " + m_threes[0].m_value );
//        else if( m_pairs.Count > 0 )  Debug.Log( "Two of a Kind " + m_pairs[0].m_value );

        if( isFullHouse ) 
        {
            result.m_type = HandType.FullHouse;
            result.m_highCard = m_threes[0].m_value;
        }
        else if( isTwoPair ) 
        {
            result.m_type = HandType.TwoPair;
            result.m_highCard = m_pairs[0].m_value > m_pairs[2].m_value ? m_pairs[0].m_value : m_pairs[2].m_value;
        }
        else if( m_fours.Count > 0 ) 
        {
            result.m_type = HandType.FourOfAKind;
            result.m_highCard = m_fours[0].m_value;
        }
        else if( m_threes.Count > 0 ) 
        {
            result.m_type = HandType.ThreeOfAKind;
            result.m_highCard = m_threes[0].m_value;
        }
        else if( m_pairs.Count > 0 ) 
        {
            result.m_type = HandType.Pair;
            result.m_highCard = m_pairs[0].m_value;
        }

        if( cards.Count == 0 ) return result;

        // Get high card value
        Card firstCard = cards[0];
        Value highCard = firstCard.m_value;
        for( int i = 1; i < cards.Count; ++i )
        {
            if( cards[i].m_value > highCard ) highCard = cards[i].m_value;
        }
        
        result.m_type = HandType.HighCard;
        result.m_highCard = highCard;
        
        if( cards.Count < 5 ) return result;
        // Check for 5 of one suit
        bool isFlush = true;
        firstCard = cards[0];
        highCard = firstCard.m_value;
        for( int i = 1; i < cards.Count; ++i )
        {
            if( cards[i].m_suit != firstCard.m_suit )
            {
                isFlush = false;
                break;
            }
        }
        
//        if( isFlush )  Debug.Log( "Flush " + firstCard.m_suit );
        if( isFlush )
        {
            result.m_type = HandType.Flush;
            result.m_highCard = highCard;
        }

        result.m_type = HandType.HighCard;
        result.m_highCard = highCard;

        return result;
        
        // TODO: Implement straights

//        // Check for run of 5 cards
//        bool isStraight = false;
//        List<Card> orderedHand = new List<Card>();
//        while( cards.Count > 1 )
//        {
//            int index = 0;
//            Card c = cards[index];
//            for( int i = 1; i < cards.Count; ++i )
//            {
//                if( cards[i].m_value < c.m_value )
//                {
//                    index = i;
//                }
//            }
//
//            orderedHand.Add( cards[index] );
//            cards.RemoveAt( index );
//        }

    }
    
    
}
