using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PegglePipe : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public float speed=1;
    public float amplitude=6;

    //private bool ballIsInPipe;
    //private PeggleManager peggleManager;


    void Start()
    {
        transform.position=Vector3.zero;
        //peggleManager=FindObjectOfType<PeggleManager>(); //si ponemos el private PeggleManager peggleManager aqui tenemos que inicializarlo. Si la ponemos publica lo que hay que hacer es arrastrarlo en unity a la pestaña pblica que se crea
    }

    /*void Update() //Update es el motor visual, y FixedUpdate es para que lo haga el motor fisico
    {
        transform.position=new Vector3(x:Mathf.Sin(Time.time*speed)*amplitude,y:0,z:0); //time.time es el tiempo que pasa desde que iniciamos el juego, no para
    }*/

    void FixedUpdate() //Update es el motor visual, y FixedUpdate es para que lo haga el motor fisico
    {
        //rigidbody.position=new Vector3(x:Mathf.Sin(Time.time*speed)*amplitude,y:0,z:0); //time.time es el tiempo que pasa desde que iniciamos el juego, no para
        //rigidbody.AddForce(Vector3.right*speed*Time.fixedDeltaTime);

        rigidbody.MovePosition(new Vector3(x:Mathf.Sin(Time.time*speed)*amplitude,y:0,z:0));


    }

    
    //isIn=false;
    /*private void OnTriggerEnter(Collider other)
    {
       Debug.Log(message: "Ha entrado " +other.name);
       //ballIsInPipe=true;
   
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            //peggleManager.RecoverBall();
            PeggleManager.instance.RecoverBall();
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.name=="Ball") //
        {
            peggleManager.ballsCurrentAmount++;
       
        }

    }*/

}
