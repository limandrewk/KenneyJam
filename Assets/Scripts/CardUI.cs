using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent( typeof(RectTransform) )]
public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Location
    {
        Hand,
        Direction,
        Height,
        Power
    }
    public Image m_cardBack;
    public Image m_cardFront;

    public float m_cardAnimation = -1;
    public float m_flipSpeed = 2;
    
    private float m_cardWidth;

    private bool m_frontFacing = false;
    public bool m_isAnimating = false;

    public Deck m_deck;

    private Vector3 m_initialPos;

    public CardHandler.Card m_card;

    public Location m_location = Location.Hand;
    private RectTransform m_rect;

    // Start is called before the first frame update
    void Start()
    {
        m_cardWidth = Mathf.Max( m_cardBack.rectTransform.rect.width, m_cardFront.rectTransform.rect.width );
        m_cardFront.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 0 );
        m_cardAnimation = -1;
        m_frontFacing = false;
        m_rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if( m_isAnimating )
        {
            float animChange = Time.deltaTime * m_flipSpeed;
            if( m_frontFacing ) m_cardAnimation -= animChange;
            else m_cardAnimation += animChange;

            m_cardAnimation = Mathf.Clamp( m_cardAnimation, -1, 1 );
            
            if( m_cardAnimation > 0 )
            {
                m_cardBack.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 0 );
                m_cardFront.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, m_cardWidth * m_cardAnimation );
            }
            else
            {
                m_cardBack.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, m_cardWidth * -m_cardAnimation );
                m_cardFront.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 0 );
            }

            if( m_cardAnimation >= 1 )
            {
                m_frontFacing = true;
                m_isAnimating = false;
            }
            else if( m_cardAnimation <= -1 )
            {
                m_frontFacing = false;
                m_isAnimating = false;
            }
        }
    }

    void FlipCard()
    {
        m_isAnimating = true;
    }

    public void OnBeginDrag( PointerEventData eventData )
    {
        if( !m_deck.m_directionBoxLocked && m_location == Location.Direction ) return;
        if( !m_deck.m_heightBoxLocked && m_location == Location.Height ) return;
        if( !m_deck.m_powerBoxLocked && m_location == Location.Power ) return;
        
        m_initialPos = transform.position;
    }

    public void OnDrag( PointerEventData eventData )
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag( PointerEventData eventData )
    {
        // If inside a box put it there, otherwise return it to the top
        if( !m_deck.m_directionBoxLocked && m_location == Location.Direction ) return;
        if( !m_deck.m_heightBoxLocked && m_location == Location.Height ) return;
        if( !m_deck.m_powerBoxLocked && m_location == Location.Power ) return;

        Vector2 directionPosition = m_deck.m_directionBox.rectTransform.InverseTransformPoint( Input.mousePosition );
        if ( !m_deck.m_directionBoxLocked && m_deck.m_directionBox.rectTransform.rect.Contains( directionPosition ) )
        {
            if( m_location == Location.Direction ) { transform.position = m_initialPos; return; }
            
            m_deck.m_directionCards.Add( this );
            switch( m_location )
            {
                case Location.Hand:
                    m_deck.m_hand.Remove( this );
                    break;
                case Location.Height:
                    m_deck.m_heightCards.Remove( this );
                    break;
                case Location.Power:
                    m_deck.m_powerCards.Remove( this );
                    break;
            }
            m_location = Location.Direction;
            m_deck.OrganiseCards();
            return;
        }
        
        Vector2 heightPosition = m_deck.m_heightBox.rectTransform.InverseTransformPoint( Input.mousePosition );
        if ( !m_deck.m_heightBoxLocked && m_deck.m_heightBox.rectTransform.rect.Contains( heightPosition ) )
        {
            if( m_location == Location.Height ) { transform.position = m_initialPos; return; }
            
            m_deck.m_heightCards.Add( this );
            switch( m_location )
            {
                case Location.Hand:
                    m_deck.m_hand.Remove( this );
                    break;
                case Location.Direction:
                    m_deck.m_directionCards.Remove( this );
                    break;
                case Location.Power:
                    m_deck.m_powerCards.Remove( this );
                    break;
            }
            m_location = Location.Height;
            m_deck.OrganiseCards();
            return;
        }
        
        Vector2 powerPosition = m_deck.m_powerBox.rectTransform.InverseTransformPoint( Input.mousePosition );
        if ( !m_deck.m_powerBoxLocked && m_deck.m_powerBox.rectTransform.rect.Contains( powerPosition ) )
        {
            if( m_location == Location.Power ) { transform.position = m_initialPos; return; }
            
            m_deck.m_powerCards.Add( this );
            switch( m_location )
            {
                case Location.Hand:
                    m_deck.m_hand.Remove( this );
                    break;
                case Location.Direction:
                    m_deck.m_directionCards.Remove( this );
                    break;
                case Location.Height:
                    m_deck.m_heightCards.Remove( this );
                    break;
            }
            m_location = Location.Power;
            m_deck.OrganiseCards();
            return;
        }

        if( m_location == Location.Hand ) { transform.position = m_initialPos; return; }

        m_deck.m_hand.Add( this );
        switch( m_location )
        {
            case Location.Direction:
                m_deck.m_directionCards.Remove( this );
                break;
            case Location.Height:
                m_deck.m_heightCards.Remove( this );
                break;
            case Location.Power:
                m_deck.m_powerCards.Remove( this );
                break;
        }
        m_location = Location.Hand;
        m_deck.OrganiseCards();
    }


    public void SetPosition( float x, float y )
    {
        m_rect.anchoredPosition = new Vector2( x, y );
    }
    
}
