using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: OstrichLegs
//
// Description: This class handles a single instance of the ostrich leg
//-------------------------------------------------------------------------------------------------------------
public class OstrichLegs : MonoBehaviour
{
   //-----------------------------------------------------------------
   // Function (public): Start
   //------------------------------------------------------------------
	void Start()
   {
      // these are arbitrary values used to place the model's legs next to the body
      // the translation was gathered out of the unity editor window * the scale factor for the ostrich body model
      this.transform.localScale += Vector3.one;
      this.transform.Translate(new Vector3(0.00246796f, -0.0120252f, 0.00688431f) * 1000.0f);
	}

   //-----------------------------------------------------------------
   // Function (public): SetMaximumVolume
   //------------------------------------------------------------------
   public void SetMaximumVolume(float maximumVolume)
   {
      this.audio.volume = maximumVolume;
   }

   //-----------------------------------------------------------------
   // Function (public): Initialize
   //------------------------------------------------------------------
   public void Initialize(bool isLeftLeg)
   {

      this.footstepAudioClipArray.Add ((AudioClip)Instantiate(Resources.Load("footstep1")));
      this.footstepAudioClipArray.Add ((AudioClip)Instantiate(Resources.Load("footstep2")));
      this.footstepAudioClipArray.Add ((AudioClip)Instantiate(Resources.Load("footstep3")));
      this.footstepAudioClipArray.Add ((AudioClip)Instantiate(Resources.Load("footstep4")));

      this.audio.volume = 1.0f;
      this.audio.minDistance = 8.0f;
      this.audio.maxDistance = 18.0f;
      this.audio.dopplerLevel = 0.3f;
      this.audio.playOnAwake = false;
      this.audio.panLevel = 1.0f;

      if (isLeftLeg)
      {
         this.name = "left leg";
         this.isLeftLeg = true;
         this.footstepTimer = 0.0f;

      }
      else
      {
         this.name = "right leg";
         this.transform.localScale += Vector3.one;
         this.transform.Translate(new Vector3(0.0f, 0.0f, 0.03f));
         this.footstepTimer = TIME_BETWEEN_FOOTSTEP_SOUNDS / 2.0f;
         this.isLeftLeg = false;
      }
      this.animation["run"].speed = 0.0f;
   }

   //-----------------------------------------------------------------
   // Function (public): Update
   //------------------------------------------------------------------
	public void Update()
   {
	}

   //-----------------------------------------------------------------
   // Function (public): Animate
   //------------------------------------------------------------------
   public void Animate(bool onGround, bool isKicking, float speed)
   {
      if (onGround)
      {
         if (!this.onGround)
         {
            this.onGround = true;

            if (this.isLeftLeg)
            {
               this.animation["run"].time = this.animation["run"].length / 2.0f;
            }
         }
         if (this.footstepTimer > TIME_BETWEEN_FOOTSTEP_SOUNDS)
         {
            this.footstepTimer = 0.0f;
            int soundIndex = (int)(Random.value * this.footstepAudioClipArray.Count);
            soundIndex = 1;
            this.audio.clip = (AudioClip)this.footstepAudioClipArray[soundIndex];
            this.audio.Play();
         }
         else
         {
            this.footstepTimer += (Time.deltaTime * speed);
         }

         this.animation.CrossFade("run", 0.2f);
         this.animation["run"].speed = speed * SPEED_FACTOR;

      }
      else
      {
         this.onGround = false;

         if (isKicking)
         {
            this.animation.CrossFade("kick", 0.5f);

            if (this.kickSpeedTimer >= CHANGE_KICK_SPEED_TIMER)
            {
               this.animation["kick"].speed = Random.Range(0.5f, 2.5f);
               this.kickSpeedTimer = 0.0f;
            }
            else
            {
               this.kickSpeedTimer += Time.deltaTime;
            }
         }
         else
         {
            this.kickSpeedTimer = 0.0f;
            this.animation.CrossFade("legStow", 0.5f);
         }
      }
   }

   // private variables
   private bool onGround = false;
   private bool isLeftLeg = false;
   private float footstepTimer = 0.0f;
   private bool isKicking = false;
   private float kickSpeedTimer = 0.0f;
   private System.Collections.ArrayList footstepAudioClipArray = new System.Collections.ArrayList();

   // constants
   private const float SPEED_FACTOR = 4.0f;

   private const float CHANGE_KICK_SPEED_TIMER = 0.5f;


   private const float TIME_BETWEEN_FOOTSTEP_SOUNDS = 0.7f;
}