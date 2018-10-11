using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NW_Bullet : MonoBehaviour {
    //-------------------------------------------
    //variables
    public float range = 20;
    public float speed = 10;
    public float damage = 20;
    private Rigidbody rb_Bullet;

    //-------------------------------------------
	void Start () {
        //destroy when range limit reached
        Destroy(gameObject, range / speed);

        rb_Bullet = GetComponent<Rigidbody>();
        rb_Bullet.velocity = speed * transform.TransformDirection(Vector3.forward);
		
	}//--------------


    // ---------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        collision.collider.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }//---------------
}//==============================
