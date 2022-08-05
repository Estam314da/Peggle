using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour // Asi la convertimos en una clase parametrica "donde T es de tipo MonoBehaviour"
{
    protected static T _instance; //con protected damos acceso
    public static T instance
    {
        get
        {
            if (_instance==null)
            {
                _instance=FindObjectOfType<T>();
                if(_instance==null)
                {
                    GameObject go= new GameObject();
                    go.name=typeof(T).ToString();
                    _instance=go.AddComponent<T>();

                }
                (_instance as Singleton<T>).Init(); //polimorfismo: usamos la instancia como un singleton del tipo T
            }
            return _instance;            
        }
    }
    virtual protected void Init() {}

}


/* Las cosas pueden tener 3 tipos de visibilidad
public: todos pueden acceder
protected: Esta clase y las que hereden de ella
private: Solo la clase en la que estoy, los hijos no

3 tipos segun la definicion
abstracta (11:43 del 11/05/22)
virtual: la diferencia con abstract es que no necesitamos que un hijo implemente el Init, pero si queremos que tenga la opcion de hacerlo (o que por defecto el init haga algo) Queremos dar la opcion a quien herede de eso a que lo implemente y añada un comportamiento personalizado

*/