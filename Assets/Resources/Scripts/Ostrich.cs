using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: Ostrich
//
// Description: This class is the script for the ostrich body - it doesn't do much right now
//-------------------------------------------------------------------------------------------------------------
public class Ostrich : MonoBehaviour
{
	void Start()
   {
      this.name = "ostrich";
      this.transform.localScale += Vector3.one * 1000.0f;
	}

	void Update()
   {

	}

}
