using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//public class PeggleManager : MonoBehaviour
public class PeggleManager : Singleton<PeggleManager>

{   

    /*static private PeggleManager _instance;
    static public PeggleManager instance
    {
        get // el get no se puede buscar desde fuera, solo se puede leer
        {
            if (_instance==null)
            {
                _instance=FindObjectOfType<PeggleManager>();
                if(_instance==null)
                {
                    GameObject go= new GameObject("PeggleManager");
                    _instance=go.AddComponent<PeggleManager>();

                }
                _instance.Init();
            }
            return _instance;
        }
    }
    */
    public delegate void NewRound();
    public static event NewRound OnNewRound;



    [Header("Levels settings")]
    public int ballsStartAmount;
    public int ballsCurrentAmount;


    [Header("Launcher settings")]
    public Transform launcher;
    public float launcherRotationSpeed;
    public Transform ballLaunchPoint;
    public float launchForce=1;
    public AnimationCurve launcherXAnimation;
    public AnimationCurve launcherYAnimation;

    [Header("Ball settings")]
    public Transform ballTransform;
    public Rigidbody ballRigidbody;
    public int score;

    //public Rigidbody ghostBallRigidbody;
    //public float ghostBallDelay;

    private List<GameObject> bumpersToRemove;

    /*void StopBall()
    {
        //Opcion 1: Desactivarla
        //ball.SetActive(false);
        //ballRigidbody.gameObject.SetActive(false);

        //Opcion 2: Quitar gravedad
        //ballRigidbody.useGravity=false;

        //Opcion 3: Hacerla Kinematic
        ballRigidbody.isKinematic=true;
        ballTransform.SetParent(launcher); //cada vez que paremos la bola el nuevo padre sera el tubo lanzador para que se mueva con el 

    }*/

    //private bool initialized;
    private bool initialized;
    //private void Init()

    protected override void Init()
    {
        if(initialized) return;
        initialized= true;

        bumpersToRemove=new List<GameObject>();
        ballsCurrentAmount=ballsStartAmount;
        ResetBall();

    }


    private void Start()
    {   
        Init();
      
        //LA RUTINA SE INICIA AQUI
        //StartCoroutine(routine: ShowGhostBall());

        //INICIAMOS EL SINGLETON:
        //SingletonExample.instance.TestMe();

    }

    //PARA SUSCRIBIRNOS A UN EVENTO HACEN FALTA ESTAS 3 FUNCIONES
    private void OnEnable() // Suscripcion
    {
        Bumper.OnBumperActivated+= OnBumperActivated;
    }
    void OnBumperActivated(Bumper bumper) //Manejo (lo que queremos que haga aqui si eso pasa)
    {
        bumpersToRemove.Add(bumper.gameObject);
    }
    private void OnDisable() //Desuscripcion
    {
        Bumper.OnBumperActivated-=OnBumperActivated;
    }





    //RUTINA PARA QUE LANCE UNA BOLA FANTASMA SOLA (no perfeccionada)
    /*IEnumerator ShowGhostBall()
    {
        while(true)
        {
            ghostBallRigidbody.position= ballLaunchPoint.position;
            ghostBallRigidbody.velocity=Vector3.zero;
            ghostBallRigidbody.AddForce(ballLaunchPoint.forward*launchForce,ForceMode.Impulse);  
            yield return new WaitForSeconds(ghostBallDelay); 
        }
    }*/

    
    /*void ResetLauncher()
    {
        launcher.rotation=Quaternion.identity; //para enderezar el lanzador al reiniciar el juego, la linea de abajo es la misma
        //launcher.rotation=Quaternion.Euler(x:0,y:0,z:0);
    }*/
    public void RecoverBall()
    {
        Debug.Log(message: "Free Ball!");
        //ResetLauncher();
        DeleteBumpers();
        ballsCurrentAmount++;
        TryStartNewRound();
    }

    public void LooseBall()
    {
        //ResetLauncher();
        DeleteBumpers();
        Debug.Log("hola");
        TryStartNewRound();
    }

    void TryStartNewRound()
    {
        ballsCurrentAmount--;

        //Condicion de derrota:
        if (ballsCurrentAmount<0)
        {
            Debug.Log(message:"GAME OVER!");
            Destroy(ballTransform.gameObject);
            return; //evita que se ejecute lo de abajo, acaba la funcion aqui
        }
        ResetBall();       

        if(OnNewRound!=null)
            OnNewRound(); 
    }

    public void ResetBall()
    {
        ballRigidbody.velocity=Vector3.zero;
        //other.transform.position=new Vector3(x:0,y:0,z:0);
        ballRigidbody.isKinematic=true;
        //ballTransform.SetParent(ballLaunchPoint);
        //ballTransform.localPosition= Vector3.zero;
        StartCoroutine(ResetLauncherAndBall());
           
    }

    IEnumerator ResetLauncherAndBall()
    {
        ballTransform.localScale=Vector3.zero;
        float animationTime=0.75f;
        float elapsedTime=0;
        Quaternion startRotation=launcher.rotation;
        while(elapsedTime<animationTime)
        {
            launcher.rotation=Quaternion.LerpUnclamped(startRotation, Quaternion.identity,elapsedTime/animationTime);
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        launcher.rotation=Quaternion.identity;

        if(ballTransform==null)
            yield break;
        ballTransform.localScale= Vector3.zero;
        ballTransform.position=ballLaunchPoint.position;

        animationTime=0.5f;
        elapsedTime=0;
        Vector3 ballTargetScale=Vector3.one*0.6f;
        while(elapsedTime<animationTime)
        {
            ballTransform.localScale=Vector3.Lerp(Vector3.zero,ballTargetScale,elapsedTime/animationTime);
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        ballTransform.localScale=ballTargetScale;
        ballTransform.SetParent(ballLaunchPoint);
    }



    void LaunchBall()
    {
        if (ballTransform.parent==ballLaunchPoint)
        {
            ballRigidbody.isKinematic=false;
            ballTransform.SetParent(null);
            ballRigidbody.AddForce(ballLaunchPoint.forward*launchForce,ForceMode.Impulse);  
        }

    }



    IEnumerator LaunchAnimation()
    {
        float animationTime=0.6f;
        float elapsedTime=0;
        bool launched=false;
        while(elapsedTime<animationTime)
        {
            float t=elapsedTime/animationTime;
            launcher.localScale=new Vector3(
                launcherXAnimation.Evaluate(t),
                launcherYAnimation.Evaluate(t),
                1
            );
            if(t>0.5f && !launched) //"cuando pase un 60% de la animacion"
            {
                launched=true;
                ballRigidbody.isKinematic=false;
                ballRigidbody.AddForce(ballLaunchPoint.forward*launchForce,ForceMode.Impulse);
            }
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        launcher.localScale=Vector3.one;

    }

    void DeleteBumpers()
    {
        int basePoints=0;
        int numberOfBumpers=bumpersToRemove.Count; //las bolas con las que ha chocado
        for (var i=0; i< bumpersToRemove.Count; i++)
        {
            GameObject bumperGO=bumpersToRemove[i];
            Bumper bumper=bumperGO.GetComponent<Bumper>();
            basePoints+=bumper.score;
            bumper.Destroy(i*0.1f);  
        }
        
        bumpersToRemove.Clear();
        Debug.Log(message: $"Base Score: {basePoints}");
        int roundScore=basePoints*numberOfBumpers;
        Debug.Log(message: "Round Score: " + roundScore);
        score+=roundScore;
    }

    private void Update()
    {
        //Condicion de victoria:
        Bumper[] foundBumper= FindObjectsOfType<Bumper>(); //buscamos si hay bolas, todas tienen en comun que tienen el scrip Bumper
        if (foundBumper.Length==0)
        {
            Debug.Log(message: "VICTORY!");
            return;
        }    

        if (ballRigidbody== null || ballTransform.parent!= ballLaunchPoint)
        {
            return;
        }
        if(Keyboard.current.aKey.isPressed)
        {
            launcher.Rotate(Vector3.back,angle: launcherRotationSpeed*Time.deltaTime);
        }
        if(Keyboard.current.dKey.isPressed)
        {
            launcher.Rotate(Vector3.forward,angle: launcherRotationSpeed*Time.deltaTime);
        }
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            LaunchBall();
        }
    }





}
