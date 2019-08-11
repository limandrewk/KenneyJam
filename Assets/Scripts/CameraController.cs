using System.Linq;
using UnityEngine;

[RequireComponent( typeof(Camera) )]
public class CameraController : MonoBehaviour
{
    public Canvas m_pauseMenu;
    private Camera m_camera;

    public float initialSpeed = 10f;
    public float increaseSpeed = 1.25f;

    public float m_cursorSensitivity = 0.025f;

    private float currentSpeed = 0f;
    private bool moving = false;
    private bool togglePressed = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if( m_pauseMenu != null && m_pauseMenu.isActiveAndEnabled ) return;

        bool lastMoving = moving;
        Vector3 deltaPosition = Vector3.zero;

        if( moving )
            currentSpeed += increaseSpeed * Time.deltaTime;

        moving = false;

        if( Sinput.GetButtonRaw( "Up" ) )
        {
            DoMove( ref deltaPosition, transform.forward );
        }

        if( Sinput.GetButtonRaw( "Down" ) )
        {
            DoMove( ref deltaPosition, -transform.forward );
        }

        if( Sinput.GetButtonRaw( "Right" ) )
        {
            DoMove( ref deltaPosition, transform.right );
        }

        if( Sinput.GetButtonRaw( "Left" ) )
        {
            DoMove( ref deltaPosition, -transform.right );
        }
        
        if( Sinput.GetButtonRaw( "FlyUp" ) )
        {
            DoMove( ref deltaPosition, transform.up );
        }

        if( Sinput.GetButtonRaw( "FlyDown" ) )
        {
            DoMove( ref deltaPosition, -transform.up );
        }


        if( moving )
        {
            if( moving != lastMoving )
                currentSpeed = initialSpeed;

            transform.position += deltaPosition * currentSpeed * Time.deltaTime;
        }
        else currentSpeed = 0f;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp( pos.x, -30f, 30f);
        pos.y = Mathf.Clamp( pos.y, 1f, 15f );
        pos.z = Mathf.Clamp( pos.z, -20f, 60f);
        transform.position = pos;

        if( Sinput.GetButtonRaw( "Fire2" ) )
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x += -Sinput.GetAxis( "Look Vertical" ) * m_cursorSensitivity;
            eulerAngles.y += Sinput.GetAxis( "Look Horizontal" ) * m_cursorSensitivity;
            transform.eulerAngles = eulerAngles;
        }
    }

    private void DoMove( ref Vector3 deltaPosition, Vector3 directionVector )
    {
        moving = true;
        deltaPosition += directionVector;
    }
}