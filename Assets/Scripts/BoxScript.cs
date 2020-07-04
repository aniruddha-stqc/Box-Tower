using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    // moving limits right and left
    private float min_X = -2.2f, max_X = 2.2f;

    private bool canMove;

    private float moveSpeed = 2f;

    private Rigidbody2D myBody;

    private bool gameOver;

    private bool ignoreCollision;

    private bool ignoreTrigger;

    private void Awake()
    {
        // get the rigidbody 2d component
        myBody = GetComponent<Rigidbody2D>();
        // don't fall initially
        myBody.gravityScale = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;

        //50-50 chance
        if(Random.Range(0, 2) > 0)
        {
            //randomize the move speed 
            moveSpeed *= -1.0f;
        }

        GameplayController.instance.currentBox = this;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBox();   
    }

    void MoveBox()
    {
        if (canMove)
        {
            //Get the current position of the box
            Vector3 temp = transform.position;

            //Move the box with move speed in the current direction
            temp.x += moveSpeed * Time.deltaTime;

            if(temp.x > max_X)
            {
                //change direction if too right
                moveSpeed *= -1.0f;
            }
            else if (temp.x < min_X)
            {
                //change direction if too left
                moveSpeed *= -1.0f;
            }
            transform.position = temp;
        }
    }

    public void DropBox()
    {
        //stop the movement
        canMove = false;
        //add pull down
        myBody.gravityScale = Random.Range(2, 4);
    }

    void Landed()
    {
        if (gameOver)
        {
            return;
        }

        ignoreCollision = true;
        ignoreTrigger = true;

        GameplayController.instance.SpawnNewBox();
        GameplayController.instance.moveCamera();

    }

    void RestartGame()
    {
        GameplayController.instance.Restart();
    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        if (ignoreCollision)
        {
            return;
        }   

        if(target.gameObject.tag == "Platform")
        {
            Invoke("Landed", 2.0f);
            ignoreCollision = true;
        }

        if(target.gameObject.tag == "Box")
        {
            Invoke("Landed", 2.0f);
            ignoreCollision = true;
        }
            
    }


    private void OnTriggerEnter2D(Collider2D target)
    {
        if (ignoreTrigger)
        {
            return;
        }

        if(target.gameObject.tag == "GameOver")
        {
            CancelInvoke("Landed");
            gameOver = true;
            ignoreTrigger = true;
            Invoke("RestartGame", 2.0f);
        }
    }
}
