using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    Rigidbody2D leBody;
    Animator leAnimator;

    [SerializeField] int bulletForce;

    // Start is called before the first frame update
    void Start(){
        leAnimator = GetComponent<Animator>();
        leBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //leBody.velocity = bulletForce * (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
    }

    void AfterExplosion(){
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        leBody.velocity = Vector2.zero;
        leAnimator.SetTrigger("explosion");
    }

}
