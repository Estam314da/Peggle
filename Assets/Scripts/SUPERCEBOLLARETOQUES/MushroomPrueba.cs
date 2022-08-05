using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPrueba : EnemyPrueba
{
    private float timeToDie=0.25f;
    private Vector3 initialScale;

    void Start()
    {
        StartEnemy();
    }

    public override void StartEnemy()
    {
        base.StartEnemy();
        enemyWidth=0.5f;
        enemyHeight=0.5f;
        enemyHead=0.95f;
        initialScale=transform.GetChild(0).localScale;        
    }

    void FixedUpdate()
    {
        FixedUpdateEnemy();
        if (enemyDamaged)
        {
            StartCoroutine(JumpDeath());
        }
    }

    IEnumerator JumpDeath()
    {
        float elapsedTime=0;
        Vector3 currentPosition=transform.position;
        while (elapsedTime<timeToDie)
        {
            float t=elapsedTime/timeToDie;
            transform.GetChild(0).localScale=Vector3.Lerp(initialScale, new Vector3(1,0.2f,1), t*t*50);
            transform.position= Vector3.Lerp(currentPosition, currentPosition-Vector3.up*0.375f,t*t*50);
            elapsedTime+=Time.deltaTime;
            yield return 0;           
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(transform.gameObject);
    }
}
