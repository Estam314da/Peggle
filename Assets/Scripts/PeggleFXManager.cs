
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class PeggleFXManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bumperHitFX;
    public GameObject bumperGlow;
    public GameObject uiBumperScore;
    
    [Header("References")]
    public Transform IngameView;

    //para limitar los efectos de particulas que ponemos en la escena lo hacemos con la piscina (en vez de crearlos y destruirlos, creamos 5 y luego esos los movemos segun necesitemos)
    private int bumperHitFXAmout = 5; 
    private int currentBumperHitFX = 0;
    private List<GameObject> bumperHitFXPool = new List<GameObject>();

    private ObjectPool<GameObject> bumperGlowPool; //hay que decirle de que tipo de cosas es el pool
    private ObjectPool<UIScoreText> uiScorePool;

    void Start()
    {
        for (int i = 0; i < bumperHitFXAmout; i++)
        {
            GameObject fx = Instantiate(bumperHitFX, Vector3.one * 1000, Quaternion.identity);
            bumperHitFXPool.Add(fx);
        }

        //el object pool llama a funciones, otra forma de escribirlo todo dentro esta desbajo
        bumperGlowPool = new ObjectPool<GameObject>(
            CreateGlow,
            OnGetGlow,
            OnReleaseGlow,
            actionOnDestroy: null, collectionCheck: true, defaultCapacity: 5, maxSize: 10
        );
        /*bumperGlowPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                return Instantiate(bumperGlow, Vector3.one*1000, Quaternion.identity);
            },
            actionOnGet: go =>
            {
                go.SetActive(true);
            },
            actionOnRelease: go =>
            {
                go.SetActive(false);
            }, actionOnDestroy: null, collectionCheck: true, defaultCapacity:5, maxSize: 10
        );*/

        uiScorePool=new ObjectPool<UIScoreText>(
            CreateUIScore,
            OnGetUIScore,
            OnReleaseUIScore,
            null, true,5,10
        );

    }

    GameObject CreateGlow()
    {   
        return Instantiate(bumperGlow, Vector3.one * 1000, Quaternion.identity);
    }

    void OnGetGlow(GameObject go) // go es por ser game object
    {
        go.SetActive(true);
    }

    void OnReleaseGlow(GameObject go)
    {
        go.SetActive(false);
    }

    UIScoreText CreateUIScore()
    {
        GameObject score= Instantiate(uiBumperScore,Vector3.zero, Quaternion.identity, IngameView);
        score.SetActive(false);
        return score.GetComponent<UIScoreText>();
    }

    void OnGetUIScore(UIScoreText go)
    {
        go.gameObject.SetActive(true);
    }

    void OnReleaseUIScore(UIScoreText go)
    {
        go.gameObject.SetActive(false);
    }




    GameObject GetNextBumperHitFX()
    {
        GameObject next = bumperHitFXPool[currentBumperHitFX];
        currentBumperHitFX = (currentBumperHitFX + 1) % bumperHitFXAmout;
        ParticleSystem ps = next.GetComponent<ParticleSystem>();
        ps.Play();
        return next;
    }
    
    private void OnEnable()
    {
        Bumper.OnBumperActivated += OnBumperActivated;
    }

    private void OnDisable()
    {
        Bumper.OnBumperActivated -= OnBumperActivated;
    }

    void OnBumperActivated(Bumper bumper)
    {
        GameObject fx = GetNextBumperHitFX(); //efecto de particulas
        fx.transform.position = bumper.transform.position;
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startColor = bumper.scoreBasedColor;

        GameObject glow = bumperGlowPool.Get();
        glow.GetComponent<BumperGlow>().SetupGlow(bumper, bumperGlowPool);

        ShowBumperScore(bumper); //añadimos puntiacion en la interfaz
    }

    void ShowBumperScore (Bumper bumper)
    {
        UIScoreText uiScore=uiScorePool.Get(); //pool busca un prefab instanciado que este libre
        uiScore.SetupScoreText(bumper,uiScorePool);

        //Vector3 screenPosition=Camera.main.WorldToScreenPoint(bumper.transform.position); // la posicion de los objetos en el mundo y en la ui es diferente, hay que convertirla
    }




}
