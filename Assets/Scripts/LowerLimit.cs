using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerLimit : MonoBehaviour
{  
   //public PeggleManager peggleManager;

   private void OnTriggerEnter(Collider other)
   {
      if(other.name=="Ball")
      {
         //peggleManager.LooseBall ();
         PeggleManager.instance.LooseBall();
         
      }

   }





















    /*
    private void OnTriggerEnter(Collider other)
    {
       Debug.Log(message: "Ha entrado " +other.name);
    }
    private void OnTriggerStay(Collider other)
    {
       Debug.Log(message: "Esta dentro " +other.name);
    }
    private void OnTriggerExit(Collider other)
    {
       Debug.Log(message: "Ha salido " +other.name);
    }

*/


}
