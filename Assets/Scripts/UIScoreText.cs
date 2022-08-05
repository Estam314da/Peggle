using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Vector3=UnityEngine.Vector3;

public class UIScoreText : MonoBehaviour
{
    public AnimationCurve growCurve;
    public AnimationCurve shrinkCurve;
    public float floatVelocity=5;

    public TextMeshProUGUI label;
    private ObjectPool<UIScoreText> pool;
    public void SetupScoreText(Bumper bumper, ObjectPool<UIScoreText> pool)
    {
        this.pool=pool;
        label.text=$"+{bumper.score}";
        RectTransform rt =label.rectTransform;
        Vector3 screenPosition=Camera.main.WorldToScreenPoint(bumper.transform.position);
        rt.position= screenPosition;//anchoredPosition tiene encuenta los anclas que tiene el red transform
        StartCoroutine(ShowScoreText(rt));
    }
    IEnumerator ShowScoreText(RectTransform scoreText)
    {
        scoreText.position +=(Vector3)Random.insideUnitCircle*5;

        float animationTime= 0.5f;
        float elapsedTime=0;
        while(elapsedTime<animationTime)
        {
            float t= growCurve.Evaluate(elapsedTime/animationTime);
            scoreText.localScale=Vector3.LerpUnclamped(Vector3.zero,Vector3.one,t);
            scoreText.position+=Vector3.up*Time.deltaTime*floatVelocity;
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        scoreText.localScale=Vector3.one;

        animationTime=1;
        elapsedTime=0;
        while(elapsedTime<animationTime)
        {
            scoreText.position+=Vector3.up*Time.deltaTime*floatVelocity;
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        animationTime=0.5f;
        elapsedTime=0;
        while(elapsedTime<animationTime)
        {
            float t=shrinkCurve.Evaluate(elapsedTime/animationTime);
            scoreText.localScale=Vector3.LerpUnclamped(Vector3.one,Vector3.zero,t);
            scoreText.position+=Vector3.up*Time.deltaTime*floatVelocity;
            elapsedTime+=Time.deltaTime;
            yield return 0;
        }
        scoreText.localScale=Vector3.zero;

        pool.Release(this); // para liberar ese scoretext de pool
    }
}
