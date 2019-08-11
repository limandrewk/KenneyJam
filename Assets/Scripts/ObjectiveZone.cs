using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveZone : MonoBehaviour
{
    public GameObject m_winUI;
    private void OnTriggerStay( Collider other )
    {
        if( other.CompareTag( "Player" ) )
        {
            GolfBall ball = other.gameObject.GetComponent<GolfBall>();
            if( !ball.IsMoving() )
            {
                m_winUI.SetActive( true );
            }
        }
    }

}
