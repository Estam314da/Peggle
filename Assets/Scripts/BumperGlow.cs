using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BumperGlow : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    private GameObject targetBumper;
    private ObjectPool<GameObject> objectPool;

    public void SetupGlow(Bumper bumper, ObjectPool<GameObject> pool)
    {
        targetBumper = bumper.gameObject;
        objectPool = pool;
        transform.position = bumper.transform.position;

        Color color = bumper.activatedColor;
        color.a = 0;
        spriteRenderer.color = color;
        StartCoroutine(Fade(0,1));
    }

    private void Update()
    {
        if(targetBumper!=null)
            transform.position=targetBumper.transform.position;
    }


    IEnumerator Fade(float from, float to)
    {
        float animationTime = 0.25f;
        float elapsedTime = 0;
        Color color = spriteRenderer.color;
        while (elapsedTime < animationTime)
        {
            color.a = Mathf.Lerp(from,to,elapsedTime/animationTime);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return 0;
        }
        color.a = to;
        spriteRenderer.color = color;
        if(to ==0)
            objectPool.Release(gameObject);
    }

    private void OnEnable()
    {
        Bumper.OnBumperDestroyed += OnBumperDestroyed;
    }

    private void OnDisable()
    {
        Bumper.OnBumperDestroyed -= OnBumperDestroyed;
    }

    void OnBumperDestroyed(Bumper bumper)
    {
        if (bumper.gameObject == targetBumper)
        {
            StartCoroutine(Fade(1,0));           
        }
    }
    
}
