using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Animator leAnimator;
    Rigidbody2D leBody;
    BoxCollider2D leCollider;

    [SerializeField] BoxCollider2D patas;
    [SerializeField] GameObject bullet;
    [SerializeField] int speed;
    [SerializeField] int dashForce;
    [SerializeField] float jumpOnDashForce;
    [SerializeField] float maxDashTime;
    [SerializeField] float minDashTime;
    [SerializeField] float minTimeBetDash;
    [SerializeField] float minTimeBetshoot;
    [SerializeField] float maxPointingTime;
    [SerializeField] int jumpForce;

    private float dashTime;
    private float shootTime;
    private float timeBetDash;
    private float pointingTime;
    private float maOrientation;
    private bool canDash;

    void Start()
    {
        leAnimator = GetComponent<Animator>();
        leBody = GetComponent<Rigidbody2D>();
        leCollider = GetComponent<BoxCollider2D>();

        maOrientation = 1;
        canDash = true;
        pointingTime = 0;
        timeBetDash = 0;

    }

    // Update is called once per frame
    void Update()
    {
        XMovement();
        YMovement();
        //RaycastThing();
        isFalling();
        Fire();
        Chech4Dash();
    }

    void XMovement(){
        float movement = Input.GetAxis("Horizontal");
        maOrientation = Mathf.Sign(movement);

        if (movement != 0 && !leAnimator.GetBool("justDash"))
        {
            transform.localScale = new Vector2(maOrientation, 1);
            leAnimator.SetBool("isRunning", true);

            transform.Translate(new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0));
            
        }
        else
        {
            leAnimator.SetBool("isRunning", false);
        }

    }

    void Fire(){
        shootTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && shootTime <= 0){
            pointingTime = maxPointingTime;
            GameObject leBullet = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
            leBullet.GetComponent<Rigidbody2D>().velocity = dashForce * (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
            leAnimator.SetLayerWeight(1, 1);
            shootTime = minTimeBetshoot;
        }

        if (pointingTime > 0){
            pointingTime -= Time.deltaTime;
        }
        else{
            leAnimator.SetLayerWeight(1, 0);
        }
    }

    void Chech4Dash(){
        timeBetDash -= Time.deltaTime;
        if (!leAnimator.GetBool("justDash")){
            if (Input.GetKey(KeyCode.Z) && canDash && timeBetDash <= 0)
            {
                dashTime = maxDashTime;
                leAnimator.SetBool("justDash", true);
                leAnimator.SetBool("isJumping", false);
                leAnimator.SetBool("isFalling", false);
                canDash = false;
                leBody.gravityScale = 0f;
                //leBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else{
            dashTime -= Time.deltaTime;
            if (!leAnimator.GetBool("isJumping")){
                leBody.velocity = dashForce * (transform.localScale.x > 0 ? Vector2.right : Vector2.left);


            }

            if (dashTime <= 0 || (!Input.GetKey(KeyCode.Z) && (maxDashTime - dashTime) > minDashTime)){
                dashTime = 0;
                Vector2 maVel = leBody.velocity;
                maVel.x = 0;
                leBody.velocity = maVel;
                leAnimator.SetBool("justDash", false);
                leBody.gravityScale = 6f;

                timeBetDash = minTimeBetDash;
            }
            
        }

    }

    void YMovement(){
        if (patas.IsTouchingLayers(LayerMask.GetMask("Structure"))){
            leAnimator.SetBool("isFalling", false);
            leAnimator.SetBool("isJumping", false);
            canDash = true;
            if (Input.GetKeyDown(KeyCode.Space)){
                leAnimator.SetTrigger("tookOff");
                leAnimator.SetBool("isJumping", true);

                if (leAnimator.GetBool("justDash")) { 
                    leBody.AddForce(new Vector2(0,jumpForce * jumpOnDashForce), ForceMode2D.Impulse);
                }
                else{
                    leBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                }
            }
        }
    }

    bool RaycastThing(){
        RaycastHit2D patasRayCast = Physics2D.Raycast(leCollider.bounds.center, Vector2.down,(leCollider.bounds.extents.y * 1.2f), LayerMask.GetMask("Structure"));
        Debug.Log(patasRayCast.collider != null);
        return patasRayCast.collider != null;
    }

    void AfterTakeOffEvent(){
        leAnimator.SetBool("isJumping", false);
        leAnimator.SetBool("isFalling", true);

        Debug.Log("jiumped");
    }

    void isFalling(){
        if((leBody.velocity.y < 0 && !leAnimator.GetBool("isJumping")) && !leAnimator.GetBool("justDash"))
            leAnimator.SetBool("isFalling", true);
    }
}
