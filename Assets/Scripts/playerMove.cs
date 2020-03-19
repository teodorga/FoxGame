using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerMove : MonoBehaviour
{

    private Rigidbody2D player;
    private Animator animation;
    private Collider2D playerCol;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Text cherryCounter;
    [SerializeField] private Text died;
    [SerializeField] private float hurtForce = 100f;

    public int cherry = 0;
    public float speed = 2f;
    public string diedText = "You died!";
    
    private enum States {idle, running, jumping, falling, crouch, hurt};
    private States state = States.idle;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animator>();
        playerCol = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.y < -4)
            Application.LoadLevel(Application.loadedLevel);
        if (state != States.hurt)
        {
            movement();
        }
        playerState();
        animation.SetInteger("state", (int)state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "collectable")
        {
            Destroy(collision.gameObject);
            cherry++;
            cherryCounter.text = cherry.ToString();

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "enemy")
        {
            if (state == States.falling)
            {
                Destroy(other.gameObject);
                player.velocity = new Vector2(player.velocity.x, hurtForce * 4);
            }
            else
            {
                state = States.hurt;

                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    player.velocity = new Vector2(-hurtForce * 5, player.velocity.y);
                }
                else
                {
                    player.velocity = new Vector2(hurtForce * 5, player.velocity.y);
                }
            }
        }
    }

    private void movement()
    {
        float directionH = Input.GetAxis("Horizontal");

            if (directionH < 0)
            {
                player.velocity = new Vector2(-speed, player.velocity.y);
                transform.localScale = new Vector2(-1, 1);
            }

            else if (directionH > 0)
            {
                player.velocity = new Vector2(speed, player.velocity.y);
                transform.localScale = new Vector2(1, 1);
            }


            if (Input.GetButtonDown("Jump") && playerCol.IsTouchingLayers(ground))
            {
                player.velocity = new Vector2(player.velocity.x, speed * 5);
                state = States.jumping;
            }
    }

    private void playerState()
    {
        
        if (state == States.jumping)
        {
            if (player.velocity.y < .1f)
                state = States.falling;
        }
        else if (state == States.falling)
        {
            if (playerCol.IsTouchingLayers(ground))
            {
                state = States.idle;
                
            }
        }
        else if (state == States.hurt)
        {
            if(Mathf.Abs(player.velocity.x) < .1f)
            {
                state = States.idle;
            }
        }


        else if (Mathf.Abs(player.velocity.x) > 2f)
        {
            state = States.running;
        }
        else
        {
            state = States.idle;
        }




    }

}
