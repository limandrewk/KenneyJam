using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public bool playing = true;
	
    [Header("Settings")]
	
    public bool checkpoints = true;
    [Range(2f, 16f)] public float speedMultiplier = 10f;
    [Range(0.1f, 1f)] public float gravityPower = 0.5f;
    [Range(1.25f, 2f)] public float bouncePower = 2f;
	
    [Header("Components")]
	
    public Transform cameraRig;
	
    public Transform target;
    public Transform arrow;
    public Transform arrowHead;
	
    [Header("HUD")]
	
    public TMP_Text m_strokeDisplay;
    public int m_strokeNum = 0;

    
    private Plane plane;
    private CharacterController controller;
	
    private Vector3 velocity, moveDirection, checkpoint;
    private float squareVelocity, gravity, shootCooldown;
	
    private int shots, coins;
	
    private bool shoot = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    
}
