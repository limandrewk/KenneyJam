using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent( typeof(Rigidbody) )]
public class GolfBall : MonoBehaviour
{
	public enum BallSetting
	{
		Direction,
		Height,
		Power
	}

	[Header("Components")]
	
    public Material m_arcShader;
    public SpriteRenderer m_arrow;
    public SpriteRenderer m_arcDisplay;
    public Deck m_deck;
    public GameObject m_ballUI;
    [Header("HUD")]
	
    public TMP_Text m_strokeDisplay;
    public int m_strokeNum = 0;
    
    private bool m_isMoving;
    private Rigidbody m_rigidBody;
    private Vector3 m_checkpoint;

    // Start is called before the first frame update
    void Start()
    {
	    m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    //CardHandler.HandValue val = new CardHandler.HandValue();
	    //UpdateArc( BallSetting.Direction, val );
	    if( !m_isMoving && m_rigidBody.velocity.magnitude > 0.05f )
	    {
		    m_checkpoint = transform.position;
		    m_ballUI.SetActive( false );
		    m_isMoving = true;
	    }

	    if( m_rigidBody.velocity.magnitude < 0.05f )
	    {
		    m_rigidBody.velocity = Vector3.zero;
		    m_rigidBody.angularVelocity = Vector3.zero;
		    //Stationary so reactivate for hitting

		    if( m_isMoving )
		    {
			    m_isMoving = false;

			    m_deck.ClearAllCards();
			    m_deck.m_swingButton.interactable = true;
			    m_deck.m_mulliganButton.interactable = true;
			    m_ballUI.gameObject.transform.position = transform.position;
			    m_ballUI.SetActive( true );

			    UpdateArcRender();			    
			    m_deck.m_shootText.text = "Draw New Cards";
		    }
	    }
    }

    public void UpdateArcRender()
    {
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
	    UpdateArcRender( dirValue, angleValue, powerValue );
    }
    
    public void UpdateArcRender( CardHandler.HandValue directionValue, CardHandler.HandValue heightValue, CardHandler.HandValue powerValue )
    {
	    // Reset arrow
	    m_arrow.transform.rotation = Quaternion.Euler( 90, 0, 0 );
	    
	    int arcRange = 140;
	    switch( directionValue.m_type )
	    {
		    case CardHandler.HandType.Pair:
			    arcRange = 110;
			    break;
		    case CardHandler.HandType.TwoPair:
			    arcRange = 80;
			    break;
		    case CardHandler.HandType.ThreeOfAKind:
			    arcRange = 50;
			    break;
		    case CardHandler.HandType.Straight:
			    break;
		    case CardHandler.HandType.Flush:
			    arcRange = 25;
			    break;
		    case CardHandler.HandType.FullHouse:
			    arcRange = 10;
			    break;
		    case CardHandler.HandType.FourOfAKind:
			    arcRange = 2;
			    break;
		    case CardHandler.HandType.StraightFlush:
			    break;
		    case CardHandler.HandType.RoyalFlush:
			    break;
	    }

	    float dirAngle = -180;
	    switch( directionValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
			    break;
		    case CardHandler.Value.Three:
			    dirAngle = -150; 
			    break;
		    case CardHandler.Value.Four:
			    dirAngle = -120; 
			    break;
		    case CardHandler.Value.Five:
			    dirAngle = -90; 
			    break;
		    case CardHandler.Value.Six:
			    dirAngle = -60; 
			    break;
		    case CardHandler.Value.Seven:
			    dirAngle = -30; 
			    break;
		    case CardHandler.Value.Eight:
			    dirAngle = 0; 
			    break;
		    case CardHandler.Value.Nine:
			    dirAngle = 30; 
			    break;
		    case CardHandler.Value.Ten:
			    dirAngle = 60; 
			    break;
		    case CardHandler.Value.Jack:
			    dirAngle = 90; 
			    break;
		    case CardHandler.Value.Queen:
			    dirAngle = 120; 
			    break;
		    case CardHandler.Value.King:
			    dirAngle = 150; 
			    break;
		    default:
			    //TODO: Make this a free shoot with a worse range
			    dirAngle = 180;
			    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
			    break;
	    }

	    float heightAngle = 0;
	    switch( heightValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
		    case CardHandler.Value.Three:
		    case CardHandler.Value.Four:
		    case CardHandler.Value.Five:
			    break;
		    case CardHandler.Value.Six:
			    heightAngle = 5; 
			    break;
		    case CardHandler.Value.Seven:
			    heightAngle = 10; 
			    break;
		    case CardHandler.Value.Eight:
			    heightAngle = 15; 
			    break;
		    case CardHandler.Value.Nine:
			    heightAngle = 20; 
			    break;
		    case CardHandler.Value.Ten:
			    heightAngle = 25; 
			    break;
		    case CardHandler.Value.Jack:
			    heightAngle = 30; 
			    break;
		    case CardHandler.Value.Queen:
			    heightAngle = 35; 
			    break;
		    case CardHandler.Value.King:
			    heightAngle = 40; 
			    break;
		    default:
			    //TODO: Make this a free shoot with a worse range
			    heightAngle = 45;
			    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
			    break;
	    }
	    
	    float force = 50;
	    switch( powerValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
			    break;
		    case CardHandler.Value.Three:
			    force = 60; 
			    break;
		    case CardHandler.Value.Four:
			    force = 70; 
			    break;
		    case CardHandler.Value.Five:
			    force = 80; 
			    break;
		    case CardHandler.Value.Six:
			    force = 90; 
			    break;
		    case CardHandler.Value.Seven:
			    force = 100; 
			    break;
		    case CardHandler.Value.Eight:
			    force = 110; 
			    break;
		    case CardHandler.Value.Nine:
			    force = 120; 
			    break;
		    case CardHandler.Value.Ten:
			    force = 130; 
			    break;
		    case CardHandler.Value.Jack:
			    force = 145; 
			    break;
		    case CardHandler.Value.Queen:
			    force = 160; 
			    break;
		    case CardHandler.Value.King:
			    force = 180; 
			    break;
		    default:
			    force = 200; 
			    break;
	    }
	    float multiplier = 1;
	    switch( powerValue.m_type )
	    {
		    case CardHandler.HandType.Pair:
			    multiplier = 1.3f;
			    break;
		    case CardHandler.HandType.TwoPair:
			    multiplier = 1.5f;
			    break;
		    case CardHandler.HandType.ThreeOfAKind:
			    multiplier = 1.8f;
			    break;
		    case CardHandler.HandType.Straight:
			    break;
		    case CardHandler.HandType.Flush:
			    multiplier = 2.5f;
			    break;
		    case CardHandler.HandType.FullHouse:
			    multiplier = 3.0f;
			    break;
		    case CardHandler.HandType.FourOfAKind:
			    multiplier = 5.0f;
			    break;
		    case CardHandler.HandType.StraightFlush:
			    break;
		    case CardHandler.HandType.RoyalFlush:
			    break;
	    }

	    float power = force * multiplier * 0.005f;
	    if( power < 0.5f ) power = 0.5f;
	    
	    m_arrow.transform.rotation = Quaternion.Euler( 90 - heightAngle, dirAngle, 0 );
	    m_arrow.size = new Vector2( m_arrow.size.x, power );
	    
	    m_arcShader.SetFloat("_Arcrange", arcRange );
	    m_arcDisplay.transform.rotation = Quaternion.Euler( 90 - heightAngle, dirAngle - arcRange * 0.5f + 90.0f, 0 );
    }
    
    public void UpdateArc( BallSetting setting, CardHandler.HandValue cardValue )
    {
	    switch( setting )
	    {
		    case BallSetting.Direction:
		    {
			    int arcRange = 140;
			    switch( cardValue.m_type )
			    {
				    case CardHandler.HandType.Pair:
					    arcRange = 110;
					    break;
				    case CardHandler.HandType.TwoPair:
					    arcRange = 80;
					    break;
				    case CardHandler.HandType.ThreeOfAKind:
					    arcRange = 50;
					    break;
				    case CardHandler.HandType.Straight:
					    break;
				    case CardHandler.HandType.Flush:
					    arcRange = 25;
					    break;
				    case CardHandler.HandType.FullHouse:
					    arcRange = 10;
					    break;
				    case CardHandler.HandType.FourOfAKind:
					    arcRange = 2;
					    break;
				    case CardHandler.HandType.StraightFlush:
					    break;
				    case CardHandler.HandType.RoyalFlush:
					    break;
			    }
			    
			    float angle = -180;
			    switch( cardValue.m_highCard )
			    {
				    case CardHandler.Value.Two:
					    break;
				    case CardHandler.Value.Three:
					    angle = -150; 
					    break;
				    case CardHandler.Value.Four:
					    angle = -120; 
					    break;
				    case CardHandler.Value.Five:
					    angle = -90; 
					    break;
				    case CardHandler.Value.Six:
					    angle = -60; 
					    break;
				    case CardHandler.Value.Seven:
					    angle = -30; 
					    break;
				    case CardHandler.Value.Eight:
					    angle = 0; 
					    break;
				    case CardHandler.Value.Nine:
					    angle = 30; 
					    break;
				    case CardHandler.Value.Ten:
					    angle = 60; 
					    break;
				    case CardHandler.Value.Jack:
					    angle = 90; 
					    break;
				    case CardHandler.Value.Queen:
					    angle = 120; 
					    break;
				    case CardHandler.Value.King:
					    angle = 150; 
					    break;
				    default:
					    //TODO: Make this a free shoot with a worse range
					    angle = 180;
					    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
					    break;
			    }

//			    m_arc.SetFloat("_Arcrange", arcRange );

//			    _Radius("Radius", Range( 0 , 1)) = 0.25
//			    _Arcrange("Arc range", Range( 0 , 360)) = 360
//			    _Rotation("Rotation", Range( 0 , 360)) = 0
			    break;
		    }

		    case BallSetting.Height:
		    {
			    // range is purely an adder on a floor
//			    int arcRange = 45;
//			    switch( cardValue.m_type )
//			    {
//				    case CardHandler.HandType.Pair:
//					    arcRange = 35;
//					    break;
//				    case CardHandler.HandType.TwoPair:
//					    arcRange = 25;
//					    break;
//				    case CardHandler.HandType.ThreeOfAKind:
//					    arcRange = 20;
//					    break;
//				    case CardHandler.HandType.Straight:
//					    break;
//				    case CardHandler.HandType.Flush:
//					    arcRange = 10;
//					    break;
//				    case CardHandler.HandType.FullHouse:
//					    arcRange = 5;
//					    break;
//				    case CardHandler.HandType.FourOfAKind:
//					    arcRange = 2;
//					    break;
//				    case CardHandler.HandType.StraightFlush:
//					    break;
//				    case CardHandler.HandType.RoyalFlush:
//					    break;
//			    }
			    
			    float angle = 0;
			    switch( cardValue.m_highCard )
			    {
				    case CardHandler.Value.Two:
				    case CardHandler.Value.Three:
				    case CardHandler.Value.Four:
				    case CardHandler.Value.Five:
					    break;
				    case CardHandler.Value.Six:
					    angle = 5; 
					    break;
				    case CardHandler.Value.Seven:
					    angle = 10; 
					    break;
				    case CardHandler.Value.Eight:
					    angle = 15; 
					    break;
				    case CardHandler.Value.Nine:
					    angle = 20; 
					    break;
				    case CardHandler.Value.Ten:
					    angle = 25; 
					    break;
				    case CardHandler.Value.Jack:
					    angle = 30; 
					    break;
				    case CardHandler.Value.Queen:
					    angle = 35; 
					    break;
				    case CardHandler.Value.King:
					    angle = 40; 
					    break;
				    default:
					    //TODO: Make this a free shoot with a worse range
					    angle = 45;
					    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
					    break;
			    }
			    
			    break;
		    }
		    default:
		    {
			    float force = 50;
			    switch( cardValue.m_highCard )
			    {
				    case CardHandler.Value.Two:
					    break;
				    case CardHandler.Value.Three:
					    force = 60; 
					    break;
				    case CardHandler.Value.Four:
					    force = 70; 
					    break;
				    case CardHandler.Value.Five:
					    force = 80; 
					    break;
				    case CardHandler.Value.Six:
					    force = 90; 
					    break;
				    case CardHandler.Value.Seven:
					    force = 100; 
					    break;
				    case CardHandler.Value.Eight:
					    force = 110; 
					    break;
				    case CardHandler.Value.Nine:
					    force = 120; 
					    break;
				    case CardHandler.Value.Ten:
					    force = 130; 
					    break;
				    case CardHandler.Value.Jack:
					    force = 145; 
					    break;
				    case CardHandler.Value.Queen:
					    force = 160; 
					    break;
				    case CardHandler.Value.King:
					    force = 180; 
					    break;
				    default:
					    force = 200; 
					    break;
			    }
			    float multiplier = 1;
			    switch( cardValue.m_type )
			    {
				    case CardHandler.HandType.Pair:
					    multiplier = 1.3f;
					    break;
				    case CardHandler.HandType.TwoPair:
					    multiplier = 1.5f;
					    break;
				    case CardHandler.HandType.ThreeOfAKind:
					    multiplier = 1.8f;
					    break;
				    case CardHandler.HandType.Straight:
					    break;
				    case CardHandler.HandType.Flush:
					    multiplier = 2.5f;
					    break;
				    case CardHandler.HandType.FullHouse:
					    multiplier = 3.0f;
					    break;
				    case CardHandler.HandType.FourOfAKind:
					    multiplier = 5.0f;
					    break;
				    case CardHandler.HandType.StraightFlush:
					    break;
				    case CardHandler.HandType.RoyalFlush:
					    break;
			    }

			    break;
		    }
	    }
    }
    
    public float GenerateBallDirection( CardHandler.HandValue cardValue )
    {
	    int arcRange = 140;
	    switch( cardValue.m_type )
	    {
		    case CardHandler.HandType.Pair:
			    arcRange = 110;
			    break;
		    case CardHandler.HandType.TwoPair:
			    arcRange = 80;
			    break;
		    case CardHandler.HandType.ThreeOfAKind:
			    arcRange = 50;
			    break;
		    case CardHandler.HandType.Straight:
			    break;
		    case CardHandler.HandType.Flush:
			    arcRange = 25;
			    break;
		    case CardHandler.HandType.FullHouse:
			    arcRange = 10;
			    break;
		    case CardHandler.HandType.FourOfAKind:
			    arcRange = 2;
			    break;
		    case CardHandler.HandType.StraightFlush:
			    break;
		    case CardHandler.HandType.RoyalFlush:
			    break;
	    }
			    
	    float angle = -180;
	    switch( cardValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
			    break;
		    case CardHandler.Value.Three:
			    angle = -150; 
			    break;
		    case CardHandler.Value.Four:
			    angle = -120; 
			    break;
		    case CardHandler.Value.Five:
			    angle = -90; 
			    break;
		    case CardHandler.Value.Six:
			    angle = -60; 
			    break;
		    case CardHandler.Value.Seven:
			    angle = -30; 
			    break;
		    case CardHandler.Value.Eight:
			    angle = 0; 
			    break;
		    case CardHandler.Value.Nine:
			    angle = 30; 
			    break;
		    case CardHandler.Value.Ten:
			    angle = 60; 
			    break;
		    case CardHandler.Value.Jack:
			    angle = 90; 
			    break;
		    case CardHandler.Value.Queen:
			    angle = 120; 
			    break;
		    case CardHandler.Value.King:
			    angle = 150; 
			    break;
		    default:
			    //TODO: Make this a free shoot with a worse range
			    angle = 180;
			    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
			    break;
	    }

	    float result = angle + Random.Range( arcRange * -0.5f, arcRange * 0.5f );
	    return result;
    }

    public float GenerateBallHeight( CardHandler.HandValue cardValue )
    {
	    int arcRange = 45;
	    switch( cardValue.m_type )
	    {
		    case CardHandler.HandType.Pair:
			    arcRange = 35;
			    break;
		    case CardHandler.HandType.TwoPair:
			    arcRange = 25;
			    break;
		    case CardHandler.HandType.ThreeOfAKind:
			    arcRange = 20;
			    break;
		    case CardHandler.HandType.Straight:
			    break;
		    case CardHandler.HandType.Flush:
			    arcRange = 10;
			    break;
		    case CardHandler.HandType.FullHouse:
			    arcRange = 5;
			    break;
		    case CardHandler.HandType.FourOfAKind:
			    arcRange = 2;
			    break;
		    case CardHandler.HandType.StraightFlush:
			    break;
		    case CardHandler.HandType.RoyalFlush:
			    break;
	    }
			    
	    float angle = 0;
	    switch( cardValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
		    case CardHandler.Value.Three:
		    case CardHandler.Value.Four:
		    case CardHandler.Value.Five:
			    break;
		    case CardHandler.Value.Six:
			    angle = 5; 
			    break;
		    case CardHandler.Value.Seven:
			    angle = 10; 
			    break;
		    case CardHandler.Value.Eight:
			    angle = 15; 
			    break;
		    case CardHandler.Value.Nine:
			    angle = 20; 
			    break;
		    case CardHandler.Value.Ten:
			    angle = 25; 
			    break;
		    case CardHandler.Value.Jack:
			    angle = 30; 
			    break;
		    case CardHandler.Value.Queen:
			    angle = 35; 
			    break;
		    case CardHandler.Value.King:
			    angle = 40; 
			    break;
		    default:
			    //TODO: Make this a free shoot with a worse range
			    angle = 45;
			    //arcRange = Mathf.FloorToInt( arcRange * 1.2f );
			    break;
	    }
	    
	    float result = angle + Random.Range( 0.0f, arcRange );
	    return result;
    }

    public float GenerateBallPower( CardHandler.HandValue cardValue )
    {
	    float force = 50;
	    switch( cardValue.m_highCard )
	    {
		    case CardHandler.Value.Two:
			    break;
		    case CardHandler.Value.Three:
			    force = 60; 
			    break;
		    case CardHandler.Value.Four:
			    force = 70; 
			    break;
		    case CardHandler.Value.Five:
			    force = 80; 
			    break;
		    case CardHandler.Value.Six:
			    force = 90; 
			    break;
		    case CardHandler.Value.Seven:
			    force = 100; 
			    break;
		    case CardHandler.Value.Eight:
			    force = 110; 
			    break;
		    case CardHandler.Value.Nine:
			    force = 120; 
			    break;
		    case CardHandler.Value.Ten:
			    force = 130; 
			    break;
		    case CardHandler.Value.Jack:
			    force = 145; 
			    break;
		    case CardHandler.Value.Queen:
			    force = 160; 
			    break;
		    case CardHandler.Value.King:
			    force = 180; 
			    break;
		    default:
			    force = 200; 
			    break;
	    }
	    float multiplier = 1;
	    switch( cardValue.m_type )
	    {
		    case CardHandler.HandType.Pair:
			    multiplier = 1.3f;
			    break;
		    case CardHandler.HandType.TwoPair:
			    multiplier = 1.5f;
			    break;
		    case CardHandler.HandType.ThreeOfAKind:
			    multiplier = 1.8f;
			    break;
		    case CardHandler.HandType.Straight:
			    break;
		    case CardHandler.HandType.Flush:
			    multiplier = 2.5f;
			    break;
		    case CardHandler.HandType.FullHouse:
			    multiplier = 3.0f;
			    break;
		    case CardHandler.HandType.FourOfAKind:
			    multiplier = 5.0f;
			    break;
		    case CardHandler.HandType.StraightFlush:
			    break;
		    case CardHandler.HandType.RoyalFlush:
			    break;
	    }

	    return force * multiplier * 2.0f;
    }

    public void StrikeBall( CardHandler.HandValue directionValue, CardHandler.HandValue heightValue, CardHandler.HandValue powerValue )
    {
	    if( m_isMoving ) return;
	    
	    float dirAngle = GenerateBallDirection( directionValue );
	    float heightAngle = GenerateBallHeight( heightValue );
	    float power = GenerateBallPower( powerValue );
	    
	    Vector3 forceVector = Vector3.forward;
	    transform.InverseTransformPoint( forceVector );
	    forceVector = Quaternion.Euler( 0, dirAngle, 0 ) * forceVector;
	    forceVector = Quaternion.Euler( heightAngle, 0, 0 ) * forceVector;
	    forceVector *= power;
	    
	    
	    m_rigidBody.AddForce( forceVector.x, forceVector.y, forceVector.z );
    }
    
    public void ResetStrokeCounter()
    {
	    m_strokeNum = 0;
	    UpdateStrokeCounter();
    }
    
    public void IncrementStrokeCounter()
    {
	    ++m_strokeNum;
	    UpdateStrokeCounter();
    }
    
    public void UpdateStrokeCounter()
    {
	    m_strokeDisplay.text = m_strokeNum.ToString();
    }


    public void KillBall()
    {
	    transform.position = m_checkpoint;
	    
	    m_rigidBody.velocity = Vector3.zero;
	    m_rigidBody.angularVelocity = Vector3.zero;
	    //Stationary so reactivate for hitting

	    if( m_isMoving )
	    {
		    m_isMoving = false;

		    m_deck.ClearAllCards();
		    m_deck.m_swingButton.interactable = true;
		    m_deck.m_mulliganButton.interactable = true;
		    m_ballUI.gameObject.transform.position = transform.position;
		    m_ballUI.SetActive( true );

		    m_deck.m_shootText.text = "Draw New Cards";
	    }
    }

    public bool IsMoving()
    {
	    return m_isMoving;
    }
}
