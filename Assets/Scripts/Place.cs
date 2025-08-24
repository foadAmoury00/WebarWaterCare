using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    [SerializeField] GameObject ObjectToPlace;
   public void TabToPlace()
   {
        ObjectToPlace.SetActive(true);
        ObjectToPlace.transform.SetParent(null);
   }
}
