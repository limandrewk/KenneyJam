using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Card = CardHandler.Card;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour, IPointerClickHandler
{
    [Header( "Card Prefabs" )]
    public List<CardUI> m_cardsClubsPrefabs;
    public List<CardUI> m_cardsDiamondsPrefabs;
    public List<CardUI> m_cardsHeartsPrefabs;
    public List<CardUI> m_cardsSpadesPrefabs;
    
    private List<Card> m_deck = new List<Card>();
    private List<Card> m_discards = new List<Card>();

    [Header("UI elements")]

    public Image m_directionBox;
    public Image m_heightBox;
    public Image m_powerBox;

    public TMP_Text m_errorMessage;
    public TMP_Text m_shootText;
    
    public List<CardUI> m_hand;
    public List<CardUI> m_directionCards;
    public List<CardUI> m_heightCards;
    public List<CardUI> m_powerCards;

    public GolfBall m_ball;

    public bool m_directionBoxLocked = false;
    public bool m_heightBoxLocked = false;
    public bool m_powerBoxLocked = false;

    public float m_timer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        InitialiseDeck();
    }

    public void InitialiseDeck()
    {
        m_deck.Clear();
        foreach( CardHandler.Suit s in Enum.GetValues( typeof(CardHandler.Suit) ) )
        {
            foreach( CardHandler.Value v in Enum.GetValues( typeof(CardHandler.Value) ) )
            {
                m_deck.Add( new Card( s, v ) );
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if( m_timer <= 0.0f ) return;

        m_timer -= Time.deltaTime;
        if( m_timer <= 0.0f )
        {
            m_errorMessage.gameObject.SetActive( false );
            m_timer = 0.0f;
        }
        else if( m_timer <= 2.0f )
        {
            Color c = m_errorMessage.color;
            c.a = m_timer / 2.0f;
            m_errorMessage.color = c;
        }
        


    }

    public CardUI DrawCard()
    {
        int index = Random.Range( 0, m_deck.Count );
        Card c = m_deck[index];
        m_deck.RemoveAt( index );

        CardUI result;
        switch( c.m_suit )
        {
            case CardHandler.Suit.Clubs:
                result = Instantiate( m_cardsClubsPrefabs[(int) c.m_value], transform );
                break;
            case CardHandler.Suit.Diamonds:
                result = Instantiate( m_cardsDiamondsPrefabs[(int) c.m_value], transform );
                break;
            case CardHandler.Suit.Hearts:
                result = Instantiate( m_cardsHeartsPrefabs[(int) c.m_value], transform );
                break;
            default:
                result = Instantiate( m_cardsSpadesPrefabs[(int) c.m_value], transform );
                break;
        }
        
        m_hand.Add( result );
        result.m_deck = this;
        return result;
    }
    
    public void OnPointerClick( PointerEventData eventData )
    {
        if( m_hand.Count == 0 && m_directionCards.Count == 0 && m_heightCards.Count == 0 && m_powerCards.Count == 0 )
        {
            // TODO: Deal a new set of cards if none are around
            for( int i = 0; i < 5; ++i )
            {
                DrawCard();
            }
            
            OrganiseCards();
        }
    }

    public void OrganiseCards()
    {
        float handX = 520;
        const float handY = 640;
        int spacing = 80;
        
        foreach( CardUI card in m_hand )
        {
            card.SetPosition( handX, handY );
            handX += spacing;
        }

        float directionX = 58;
        const float bottomY = 75;
        spacing = m_directionCards.Count > 4 ? 60 : 80;
        
        foreach( CardUI card in m_directionCards )
        {
            card.SetPosition( directionX, bottomY );
            directionX += spacing;
        }
        
        float heightX = 428;
        spacing = m_heightCards.Count > 4 ? 60 : 80;
        foreach( CardUI card in m_heightCards )
        {
            card.SetPosition( heightX, bottomY );
            heightX += spacing;
        }
        
        float powerX = 798;
        spacing = m_powerCards.Count > 4 ? 60 : 80;
        foreach( CardUI card in m_powerCards )
        {
            card.SetPosition( powerX, bottomY );
            powerX += spacing;
        }
    }

    public void ClearAllCards()
    {
        foreach( CardUI cui in m_hand )
        {
            Destroy( cui.gameObject );
        }
        foreach( CardUI cui in m_directionCards )
        {
            Destroy( cui.gameObject );
        }
        foreach( CardUI cui in m_heightCards )
        {
            Destroy( cui.gameObject );
        }
        foreach( CardUI cui in m_powerCards )
        {
            Destroy( cui.gameObject );
        }
        
        m_hand.Clear();
        m_directionCards.Clear();
        m_heightCards.Clear();
        m_powerCards.Clear();

        m_directionBoxLocked = false;
        m_heightBoxLocked = false;
        m_powerBoxLocked = false;
        
        InitialiseDeck();
    }
    
    public void Mulligan()
    {
        ClearAllCards();
        IncrementStrokeCounter();
    }

    public void SwingButton()
    {
        if( m_directionCards.Count != 0 && m_heightCards.Count != 0 && m_powerCards.Count != 0 )
        {
            m_ball.IncrementStrokeCounter();

            List<Card> directionCards = new List<Card>();
            List<Card> heightCards = new List<Card>();
            List<Card> powerCards = new List<Card>();

            foreach( CardUI cui in m_directionCards )
            {
                directionCards.Add( cui.m_card );
            }
            foreach( CardUI cui in m_heightCards )
            {
                heightCards.Add( cui.m_card );
            }
            foreach( CardUI cui in m_powerCards )
            {
                powerCards.Add( cui.m_card );
            }

            CardHandler.HandValue dirValue = CardHandler.EvaluateCards( directionCards );
            CardHandler.HandValue heightValue = CardHandler.EvaluateCards( heightCards );
            CardHandler.HandValue powerValue = CardHandler.EvaluateCards( powerCards );
        
            Debug.Log( "Direction type " + dirValue.m_type + " Value " + dirValue.m_highCard );
            Debug.Log( "Height type " + heightValue.m_type + " Value " + heightValue.m_highCard );
            Debug.Log( "Power type " + powerValue.m_type + " Value " + powerValue.m_highCard );
        }
        else
        {
            if( m_hand.Count != 0 )
            {
                // show error message
                m_timer = 4.0f;
                m_errorMessage.gameObject.SetActive( true );
                Color c = m_errorMessage.color;
                c.a = 1.0f;
                m_errorMessage.color = c;
            }
            else
            {
                int cardDraws = 0;
                // draw new cards and lock the filled slots
                if( m_directionCards.Count != 0 )
                {
                    m_directionBoxLocked = true;
                }
                else ++cardDraws;
                if( m_heightCards.Count != 0 )
                {
                    m_directionBoxLocked = true;
                }
                else ++cardDraws;

                if( m_powerCards.Count != 0 )
                {
                    m_directionBoxLocked = true;
                }
                else ++cardDraws;
                
                for( int i = 0; i < cardDraws; ++i )
                {
                    DrawCard();
                }
                OrganiseCards();
            }
        }
    }

    public void ResetStrokeCounter()
    {
        m_ball.ResetStrokeCounter();
    }
    
    public void IncrementStrokeCounter()
    {
        m_ball.IncrementStrokeCounter();
    }
    
    public void UpdateStrokeCounter()
    {
        m_ball.UpdateStrokeCounter();
    }

    public void UpdateShootButton()
    {
        if( m_directionCards.Count != 0 && m_heightCards.Count != 0 && m_powerCards.Count != 0 )
        {
            // Good to go
            m_shootText.text = "Swing";
        }
        else
        {
            m_shootText.text = m_hand.Count != 0 ? "Swing" : "Draw new cards";
        }
    }
}
