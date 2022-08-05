using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonExample : MonoBehaviour
{
    protected static SingletonExample _instance; //Variable priivada o protegida estatica
    static public SingletonExample instance //Punto de acceso global
    {
        get // el get no se puede buscar desde fuera, solo se puede leer
        {
            if (_instance==null)
            {
                _instance=FindObjectOfType<SingletonExample>();
                if(_instance==null)
                {
                    GameObject go= new GameObject();
                    go.name="SingletonExample";
                    _instance=go.AddComponent<SingletonExample>();
                    //Crearla
                    //Iniciarla
                }
                _instance.Init();
            }
            return _instance;
        }
    }

    private bool initialized;

    void Init()
    {
        if (initialized) return; //Ponemos estas 2 lineas para elegir el orden en el que se ejecuta
        initialized=true;
        Debug.Log("Init!");
    }

    public void TestMe()
    {
        Debug.Log("Tested!");
    }


    private void Start()
    {
        Debug.Log("Awake!");
    }

    
}
