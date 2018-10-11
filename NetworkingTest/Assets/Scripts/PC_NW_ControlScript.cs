using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PC_NW_ControlScript : NetworkBehaviour { //needs NetworkBehaviour instead of Mono to work in networking
    //---------------------------------------------------
    // Movement
    public float speed = 3;
    private float initialSpeed;
    private CharacterController myPC;
    private Vector3 moveDirection;
    public float jumpForce = 20;
    public float gravity = 20;
    private bool climbing;

    //combat
    public GameObject projectile;
    public float cooldown = 1;
    private float nextShotTime;
    //----------------------------------------------------
	void Start () {
        if (!isLocalPlayer) //if not the local player, built in property
        {
            // Remove camera from other NW players
            //Destroy(transform.Find("PC_Cam").gameObject);
            Destroy(transform.GetComponentInChildren<Camera>().gameObject);

            //Colour other NW players red
            transform.Find("Pawn").gameObject.GetComponent<Renderer>().material.color = Color.red;
            return;
        }
        // Get local ref and set walkingspeed
        myPC = GetComponent<CharacterController>();
        initialSpeed = speed;
	}//-------------
	
	void Update () {
        //These functions are only called on local player
        if (isLocalPlayer)
        {
            MovePC();
            MouseLook();

            if(Input.GetButtonDown("Fire1") && Time.time > nextShotTime)
            {
                CmdFireBullet();
            }
        }
		
	}//-----------------------

    void MovePC()
    { 
        //sprint when holding shift
        if (Input.GetKey(KeyCode.LeftShift)) speed = initialSpeed * 2; else speed = initialSpeed;

        if (myPC.isGrounded)//if on the ground can do this
        {
            //add x and z mocement to player
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //apply movespeed
            moveDirection = speed * transform.TransformDirection(moveDirection);
            //jump
            if (Input.GetButton("Jump")) moveDirection.y = jumpForce;
        }
        //add gravity to the direction vector
        moveDirection.y -= gravity * Time.deltaTime;

        //move character controller with direction vector
        myPC.Move(moveDirection * Time.deltaTime);
    }//-------------------------------

    //---------------------------------------------
    // cam pos
    public float minCamHeight = -1f;
    public float maxCamHeight = 3f;
    public float camDistance = 2.5f;
    public float mouseTurnRate = 90;
    public GameObject cam;

    //Mouse look =============================================
    void MouseLook()
    {
        //Zoom in and out with scroll wheel
        if (Input.mouseScrollDelta.y > 0 && camDistance > 0.5f) camDistance -= 0.2f;
        if (Input.mouseScrollDelta.y < 0 && camDistance < 3) camDistance += 0.2f;

        //Mouse Rotate
        transform.Rotate(0, 3 * mouseTurnRate * Time.deltaTime * Input.GetAxis("Mouse X"), 0);

        //look at PC Obj
        cam.transform.LookAt(transform.position + new Vector3(0, 1, 0));

        //Move cam
        cam.transform.localPosition = new Vector3(0, cam.transform.localPosition.y, -camDistance);

        //if (Input.GetAxis("Mouse Y") > 0 && cam.transform.localPosition.y > minCamHeight) cam.transform.Translate(0, Input.GetAxis("Mouse Y"), 0);
        if (Input.GetAxis("Mouse Y") > 0 && cam.transform.localPosition.y > minCamHeight) cam.transform.Translate(0, -0.2f, 0);

       // if (Input.GetAxis("Mouse Y") < 0 && cam.transform.localPosition.y < maxCamHeight) cam.transform.Translate(0, Input.GetAxis("Mouse Y"), 0);
        if (Input.GetAxis("Mouse Y") < 0 && cam.transform.localPosition.y < maxCamHeight) cam.transform.Translate(0, 0.2f, 0);
    }//-----------

    [Command] //send commands to the server objects
    void CmdFireBullet()
    {
        //Create a bullet on the server and reset the shot timer
        var _bullet = Instantiate(projectile, transform.position + transform.TransformDirection(new Vector3(0, 0.5f, 1.5f)), transform.rotation);
        nextShotTime = Time.time + cooldown;

        NetworkServer.Spawn(_bullet); // The object to spawn must be specified in the NW Manager
    }

}
