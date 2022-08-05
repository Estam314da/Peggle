using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrueba : MonoBehaviour
{
    protected GameObject player;    
    protected Transform enemyModel;
    public GameObject coinPrefab;
    public float speed=1.5f;
    protected int direction;
    protected int lastDirection;


    protected float enemyWidth; //radio del ancho del modelo de enemigo, para ajustar la longitud del raycast
    protected float enemyHeight; //radio de la altura del modelo de enemigo, para ajustar la longitud del raycast
    protected float enemyHead;
    protected Vector3 originalPosition;
    protected bool enemyDamaged;
    protected bool hasDied=false;


    public delegate void EnemyDyingByJump(); 
    public static event EnemyDyingByJump OnEnemyDyingByJump;

    public delegate void EnemyDyingByShut(); 
    public static event EnemyDyingByShut OnEnemyDyingByShut;



    public virtual void StartEnemy()
    {
        player=GameObject.Find("Onion");
        enemyModel = transform.GetChild(0);
        originalPosition=transform.position;
        direction=1;
        lastDirection=1;
        enemyDamaged=false;
    }

    public void FixedUpdateEnemy()
    {
        
        UpdateBodyRotation();
        direction=MoveAndRecolocateEnemy();

    }

    public int MoveAndRecolocateEnemy()
    {
        float currentDistanceWithPlayer= Vector3.Magnitude(transform.position-player.transform.position);
        float distanceFromOriginToPlayer= Vector3.Magnitude(originalPosition-player.transform.position);        
        if(currentDistanceWithPlayer<=22 || distanceFromOriginToPlayer<=22) 
        {
            transform.Translate(Vector3.right*direction*speed*Time.deltaTime); //si player esta cerca del enemigo, este se mueve
        }
        else 
        {            
            transform.position=originalPosition; //si player se ha alejado, el enemigo vuelve a su posicion
            if (player.transform.position.x>transform.position.x) //cuando vuelva a aparecer player, el enemigo se movera desde su psicion origen con direccion hacia player 
                direction=1;   
            else
                direction=-1;
        }
        return direction;
    }

    public void UpdateBodyRotation()
    {
        if(lastDirection!=direction)
        {
            StartCoroutine(TurnSlowly(direction));
        }
        lastDirection=direction;
        
    }
    IEnumerator TurnSlowly(int dir)
    {
        float elapsedTime=0;
        float TimeToTurn=0.8f;
        Vector3 desiredForward= new Vector3(0,0,-dir);
        while(elapsedTime<TimeToTurn)
        {
            enemyModel.forward=Vector3.Slerp(enemyModel.forward, desiredForward,elapsedTime/1.25f*TimeToTurn);
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        
        Vector3 collisionPoint = other.collider.ClosestPointOnBounds( transform.position );


        if(other.transform.tag!="Player")
        {
            //if(other.transform.tag!="Ramp") direction=direction*-1;
            if(other.transform.tag=="Bullet")
            {
                StartCoroutine(GiveCoin());
                if (OnEnemyDyingByShut!=null)
                    OnEnemyDyingByShut();
            }
            if(Mathf.RoundToInt(transform.position.y*10)!=Mathf.RoundToInt((collisionPoint.y+enemyHeight)*10) && other.transform.tag!="Ramp")
            {
                direction=direction*-1;
            }

            
        }
        else //si te has chocado con player
        {
            if(collisionPoint.y!=enemyHeight) // y es por arriba
            {
                enemyDamaged=true; // entonce es que player ha saltado encima, y el enemigo ha recibido daño
            }
            //Debug.DrawLine( transform.position, collisionPoint, Color.red );
        }
        

    }

    IEnumerator BulletDead()
    {
        transform.gameObject.GetComponent<BoxCollider>().enabled=false;
        yield return 0;
    }
    IEnumerator GiveCoin()
    {
        gameObject.GetComponent<Collider>().enabled=false;
        GameObject coin=Instantiate(coinPrefab, transform.position,Quaternion.identity);
        Vector3 coinOriginalPosition=coin.transform.position;
        Vector3 coinDesiredPosition=coin.transform.position+Vector3.up*2;

        float elapsedTime=0;
        float animationTime=1;
        bool coinGoingDown=false;
        while(elapsedTime<animationTime)
        {
            //if(coin==null) yield break; // yield break se sale de la corrutina directamente asi que la destruccion que esta fuera del while nunca se ejecuta
            if(coin==null) continue; //continue se salta lo que queda de esta iteracion del bucle y pasa a la siguiente, asi la corrutina si acaba (mov y destruccion del enemigo)
            if(Mathf.RoundToInt(coin.transform.position.y*100)<Mathf.RoundToInt(coinDesiredPosition.y*100) && coinGoingDown==false)
                coin.transform.position=Vector3.LerpUnclamped(coin.transform.position, coinDesiredPosition,elapsedTime/animationTime);
            else if(Mathf.RoundToInt(coin.transform.position.y*100f)>Mathf.RoundToInt(coinOriginalPosition.y*100f))
            {
                coinGoingDown=true;
                coin.transform.position=Vector3.LerpUnclamped(coin.transform.position, coinOriginalPosition,elapsedTime/animationTime);
            }
            //transform.position=Vector3.Lerp(transform.position,enemyOriginalPosition+new Vector3(2,1,0),elapsedTime/animationTime);
            transform.Rotate(Vector3.forward * Time.deltaTime*360);

            elapsedTime+=Time.deltaTime;
            yield return 0;
        }      
        Destroy(transform.gameObject);
        yield return 0;
    }

}
