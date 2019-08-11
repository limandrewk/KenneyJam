using System;
using System.Collections.Generic;
using System.Linq;
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

    public Button m_swingButton;
    public Button m_mulliganButton;
    
    public List<CardUI> m_hand;
    public List<CardUI> m_directionCards;
    public List<CardUI> m_heightCards;
    public List<CardUI> m_powerCards;

    public GolfBall m_ball;

    public bool m_directionBoxLocked;
    public bool m_heightBoxLocked;
    public bool m_powerBoxLocked;
    
    public Image m_directionBoxLockedImage;
    public Image m_heightBoxLockedImage;
    public Image m_powerBoxLockedImage;

    public float m_timer;

    private bool m_lateOrganise; 
    
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

    private void LateUpdate()
    {
        if( m_lateOrganise )
        {
            m_lateOrganise = false;
            OrganiseCards();

            foreach( CardUI c in m_hand.Where( card => !card.m_frontFacing ) )
            {
                c.FlipCard();
            }
        }
    }

    public CardUI DrawCard()
    {
        int index = Random.Range( 0, m_deck.Count );
        Card c = m_deck[index];
        m_deck.RemoveAt( index );

        CardUI result;
        Transform t = transform;
        switch( c.m_suit )
        {
            case CardHandler.Suit.Clubs:
                result = Instantiate( m_cardsClubsPrefabs[(int) c.m_value], t.position, Quaternion.identity, t.parent );
                break;
            case CardHandler.Suit.Diamonds:
                result = Instantiate( m_cardsDiamondsPrefabs[(int) c.m_value], t.position, Quaternion.identity, t.parent );
                break;
            case CardHandler.Suit.Hearts:
                result = Instantiate( m_cardsHeartsPrefabs[(int) c.m_value], t.position, Quaternion.identity, t.parent );
                break;
            default:
                result = Instantiate( m_cardsSpadesPrefabs[(int) c.m_value], t.position, Quaternion.identity, t.parent );
                break;
        }
        
        m_hand.Add( result );
        result.m_deck = this;
        return result;
    }
    
    public void OnPointerClick( PointerEventData eventData )
    {
        NewHand();
    }

    public void NewHand()
    {
        if( m_hand.Count == 0 && m_directionCards.Count == 0 && m_heightCards.Count == 0 && m_powerCards.Count == 0 )
        {
            for( int i = 0; i < 5; ++i )
            {
                DrawCard();
            }

            m_lateOrganise = true;
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

        CardHandler.HandValue dirValue = new CardHandler.HandValue
        {
            m_type = CardHandler.HandType.HighCard, m_highCard = CardHandler.Value.Eight
        };
        CardHandler.HandValue angleValue = new CardHandler.HandValue
        {
            m_type = CardHandler.HandType.HighCard, m_highCard = CardHandler.Value.Two
        };
        CardHandler.HandValue powerValue = new CardHandler.HandValue        
        {
            m_type = CardHandler.HandType.HighCard, m_highCard = CardHandler.Value.Two
        };
        
        if( m_directionCards.Count != 0 )
        {
            dirValue = EvaluateDirection();
        }
        if( m_heightCards.Count != 0 )
        {
            angleValue = EvaluateHeight();
        }
        if( m_powerCards.Count != 0 )
        {
            powerValue = EvaluatePower();
        }
        
        m_ball.UpdateArcRender( dirValue, angleValue, powerValue );
        UpdateShootButton();
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

        m_directionBoxLockedImage.gameObject.SetActive( false );
        m_heightBoxLockedImage.gameObject.SetActive( false );
        m_powerBoxLockedImage.gameObject.SetActive( false );
        
        InitialiseDeck();
    }
    
    public void Mulligan()
    {
        ClearAllCards();
        IncrementStrokeCounter();
    }

    public CardHandler.HandValue EvaluateDirection()
    {
        List<Card> directionCards = m_directionCards.Select( cui => cui.m_card ).ToList();

        CardHandler.HandValue dirValue = CardHandler.EvaluateCards( directionCards );
        //Debug.Log( "Direction type " + dirValue.m_type + " Value " + dirValue.m_highCard );
        return dirValue;
    }
    
    public CardHandler.HandValue EvaluateHeight()
    {
        List<Card> heightCards = m_heightCards.Select( cui => cui.m_card ).ToList();

        CardHandler.HandValue heightValue = CardHandler.EvaluateCards( heightCards );
        //Debug.Log( "Height type " + heightValue.m_type + " Value " + heightValue.m_highCard );
        return heightValue;
    }

    public CardHandler.HandValue EvaluatePower()
    {
        List<Card> powerCards = m_powerCards.Select( cui => cui.m_card ).ToList();

        CardHandler.HandValue powerValue = CardHandler.EvaluateCards( powerCards );
        //Debug.Log( "Power type " + powerValue.m_type + " Value " + powerValue.m_highCard );
        return powerValue;
    }

    public void SwingButton()
    {
        if( m_hand.Count == 0 && m_directionCards.Count == 0 && m_heightCards.Count == 0 && m_powerCards.Count == 0)
        {
            NewHand();
            return;
        }

        if( m_directionCards.Count != 0 && m_heightCards.Count != 0 && m_powerCards.Count != 0 )
        {
            m_ball.IncrementStrokeCounter();

            CardHandler.HandValue direction = EvaluateDirection();
            CardHandler.HandValue height = EvaluateHeight();
            CardHandler.HandValue power = EvaluatePower();

            m_swingButton.interactable = false;
            m_mulliganButton.interactable = false;
            
            m_ball.StrikeBall( direction, height, power );
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
                    m_directionBoxLockedImage.gameObject.SetActive( true );
                    m_directionBoxLocked = true;
                }
                else ++cardDraws;
                if( m_heightCards.Count != 0 )
                {
                    m_heightBoxLockedImage.gameObject.SetActive( true );
                    m_heightBoxLocked = true;
                }
                else ++cardDraws;

                if( m_powerCards.Count != 0 )
                {
                    m_powerBoxLockedImage.gameObject.SetActive( true );
                    m_powerBoxLocked = true;
                }
                else ++cardDraws;
                
                for( int i = 0; i < cardDraws; ++i )
                {
                    DrawCard();
                }

                m_lateOrganise = true;
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
            m_shootText.text = m_hand.Count != 0 ? "Swing" : "Lock in cards and Draw";
        }
    }
}
