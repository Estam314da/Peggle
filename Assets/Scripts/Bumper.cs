using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random=UnityEngine.Random;

public class Bumper : MonoBehaviour
    {
    public delegate void BumperHit(Bumper bumper);
    public static event BumperHit OnBumperHit;
    //LOS EVENTOS SE LLAMAN ASI:
    public delegate void BumperActivated(Bumper bumper);  //el delegado averigua que recibe una funcion y que devuelve, podemos elegir el numero de cosas que queremos pasar, y da igual el tipo
    public static event BumperActivated OnBumperActivated; //OnBumperActivated es como una lista de todos los sitios interesados en saber este evento (los que se hayan suscrito a este evento). Siempre son estaticos

    public delegate void BumperDestroyed (Bumper bumper);
    public static event BumperDestroyed OnBumperDestroyed;

    public int score;
    private new MeshRenderer renderer;
    [Header("Animation Values")]
    public AnimationCurve generationCurve; //curva de generacion para inicializar los bumpers
    public AnimationCurve generationPositionCurve;
    public AnimationCurve destroyCurve;
    public float generationAnimationTime=0.75f;
    public float startPositionOffset=0.4f;
    public float maxRandomDelay=0.25f;
    //private PeggleManager peggleManager;

    //public float pushForce=1f;

    enum BumperState
    {
        generate,
        ready,
        activated,
        destroy
    }
    private BumperState currentState;

    public Color scoreBasedColor {get; private set;} //asi hacemos que el color sea publico pero solo de lectura
    public Color activatedColor = Color.green;
    private float destroyDelay;
    private Vector3 startPosition;


    private void Awake()
    {
        startPosition=transform.position;
        score=Random.Range(10,20);
        if(score<15)
            scoreBasedColor=Color.yellow;
        else
            scoreBasedColor= new Color(0.92f,0.35f,0);
        renderer=GetComponent<MeshRenderer>();
        renderer.material.color=scoreBasedColor; 
        //peggleManager= FindObjectOfType<PeggleManager>(); //busca el objeto en la escena en la que esta, y entre los <> se pone el tipo de objeto
    
        StartCoroutine(FSM());
    }

    IEnumerator FSM() //lo que hace esto es que n
    {
        while(true)
        {
            yield return StartCoroutine(currentState.ToString()); //al poner este yield return hace que se espere hasta que la corrutina termine
        }
    }

    void Update()
    {
        transform.position=Vector3.Lerp(transform.position,startPosition,Time.deltaTime*5);

        /*if(Keyboard.current.rKey.wasPressedThisFrame)
            currentState=BumperState.generate;*/
    }


    IEnumerator generate()
    {
        Vector3 targetScale=Vector3.one*0.4f; // (Aitor 0.25f)escala final que queremos que tengan los bumpers(tenemos 0.4)        currentState=BumperState.active;
        Vector3 startPosition=transform.position+Vector3.up*startPositionOffset;
        Vector3 targetPosition=transform.position;

        float elapsedTime=0;
        transform.localScale=Vector3.zero;
        yield return new WaitForSeconds(Random.value*maxRandomDelay); // esta espera hace que no todos los bumpers aparezcan al mismo tiempo
        while(elapsedTime<generationAnimationTime)
        {
            float t=generationCurve.Evaluate(elapsedTime/generationAnimationTime);
            float tp=generationPositionCurve.Evaluate(elapsedTime/generationAnimationTime);
            transform.position= Vector3.LerpUnclamped(startPosition,targetPosition,tp);
            transform.localScale=Vector3.LerpUnclamped(Vector3.zero, targetScale, t); //lerp lo suaviza segun lo pongamos pero entre 0 y 1, LerpUnclamped no lo limita a 0 y 1
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        transform.localScale=targetScale;
        transform.position=targetPosition;
        currentState=BumperState.ready;
    }
    IEnumerator ready()
    {
        while(currentState==BumperState.ready)
        {
            yield return 0;
        }
    }
    
    IEnumerator activated()
    {
        renderer.material.color=activatedColor;
        if (OnBumperActivated!=null)
            OnBumperActivated(this);
        transform.localScale=Vector3.one*0.5f;
        while(currentState==BumperState.activated)
        {
            transform.localScale=Vector3.Lerp(transform.localScale,Vector3.one*0.25f, Time.deltaTime*5);
            yield return 0;
        }
    }
    IEnumerator destroy()
    {
        GetComponent<Collider>().enabled=false;
        yield return new WaitForSeconds(destroyDelay);
        if(OnBumperDestroyed!=null)
            OnBumperDestroyed(this);
        float animationTime=0.3f;
        float elapsedTime=0;
        while (elapsedTime<animationTime)
        {
            float t=destroyCurve.Evaluate(elapsedTime/animationTime);
            transform.localScale=Vector3.Lerp(Vector3.one*0.25f,Vector3.zero,t);
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        Destroy(gameObject);
    }
    
    public void Destroy(float ttd) //ttd= time to destroy
    {
        destroyDelay=ttd;
        currentState=BumperState.destroy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag== "Player")
        {
            if(currentState==BumperState.activated)
                if(OnBumperHit!=null)
                    OnBumperHit(this);

            //Debug.DrawRay(transform.position, collision.relativeVelocity, Color.red, 1);
            transform.position+= Vector3.ClampMagnitude(collision.relativeVelocity, 0.25f) ; //para que los bumpers se muevan cuando chocan contra ellos
            
            
            if(currentState== BumperState.ready)
                currentState= BumperState.activated;

            //OnBumperActivated(this); //OnBumperActivated es quienes estan suscritos, y lo que les pasa a ellos es el gameObject

            //peggleManager.BumperActivated(gameObject);
        }
   
    }

    

    /*private void OnDestroy()
    {
        if(OnBumperDestroyed != null)
            OnBumperDestroyed(this);
    }*/


}











