using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: OstrichLegs
//
// Description: This class handles a single instance of the ostrich wing
//-------------------------------------------------------------------------------------------------------------
public class OstrichWings : MonoBehaviour
{
   //-----------------------------------------------------------------
   // Function (public): Start
   //------------------------------------------------------------------
	void Start()
   {
      // these are arbitrary values used to place the model's legs next to the body
      // the translation was gathered out of the unity editor window * the scale factor for the ostrich body model
      //this.transform.localScale += Vector3.one;
      //this.transform.Translate(new Vector3(0.00246796f, -0.0120252f, 0.00688431f) * 1000.0f);
	}

   //-----------------------------------------------------------------
   // Function (public): Initialize
   //------------------------------------------------------------------
   public void Initialize(bool isOwnship)
   {

      this.transform.Translate (0.0f, 0.02f, 0.0f, Space.World);
      this.transform.localScale += Vector3.one * 5.0f;
      this.name = "wings";
      this.transform.Rotate (0.0f, 90.0f, 0.0f, Space.Self);

      this.audio.clip = (AudioClip)Instantiate(Resources.Load("whoosh"));;
      if (isOwnship)
      {
         this.audio.volume = 0.1f;
      }
      else
      {
         this.audio.volume = 1.0f;
      }

      this.audio.minDistance = 8.0f;
      this.audio.maxDistance = 18.0f;
      this.audio.dopplerLevel = 0.3f;
      this.audio.playOnAwake = false;
      this.audio.panLevel = 1.0f;
   }

   //-----------------------------------------------------------------
   // Function (public): Update
   //------------------------------------------------------------------
	public void Update()
   {
	}

   public void FlapWings()
   {
      this.animation["flap"].speed = 1.0f;
      this.animation["flap"].time = 0.0f;
      this.animation.Play ("flap");
      this.audio.Play();
   }

   // private variables

   // constants

}