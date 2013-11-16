using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: Jouster
//
// Description: This class is the script for the enemy jouster
//-------------------------------------------------------------------------------------------------------------
public class Jouster : MonoBehaviour
{

   public enum JousterModeEnum
   {
      JOUSTING  = 0,
      FALLING   = 1,
      DEAD      = 2,
      REMOVE_ME = 3
   }

   //------------------------------------------------------------------
   // Function (public): Start
   //------------------------------------------------------------------
	void Start()
   {
      this.name = "jouster";
	}

   //------------------------------------------------------------------
   // Function (public): Update
   //------------------------------------------------------------------
	public void Update()
   {
      switch (this.mode)
      {
         case JousterModeEnum.JOUSTING:
            break;
         case JousterModeEnum.FALLING:
            this.animation["falling"].speed = 1.0f;
            this.animation["falling"].time = 0.0f;
            this.animation["falling"].wrapMode = WrapMode.Once;
            this.animation.Play ("falling");

            this.transform.Translate(new Vector3(this.fallingVelocity.x, 0.0f, this.fallingVelocity.z), Space.World);
            this.transform.Translate(new Vector3(0.0f, this.fallingVelocity.y * Time.deltaTime + 0.5f*GlobalTypes.GRAVITY*Time.deltaTime*Time.deltaTime, 0.0f), Space.World);
            this.fallingVelocity.y = this.fallingVelocity.y + (GlobalTypes.GRAVITY * Time.deltaTime);

            if (this.transform.position.y < -8.0f && this.fallingVelocity.y < 0.0f)
            {
               this.transform.Translate(new Vector3(0.0f, -(this.transform.position.y - (-8.0f)), 0.0f), Space.World);
               this.fallingVelocity.y = 0.0f;
               this.mode = JousterModeEnum.DEAD;
            }
            break;
         case JousterModeEnum.DEAD:
            this.fallingVelocity *= 0.92f;
            this.transform.Translate(this.fallingVelocity, Space.World);

            break;
         case JousterModeEnum.REMOVE_ME:
            break;
         default:
            Debug.Log ("unhandled case in Jouster::Update()");
            break;
      }

	}

   //------------------------------------------------------------------
   // Function (public): RotateJousterToOwnship - rotates the jouster to the ownship
   //------------------------------------------------------------------
   public void RotateJousterToOwnship(float angle)
   {
      // this is the mapping between angles and animation frames from blender
      float convertedAngle = 0.5f*angle + 90.0f;

      if (Mathf.Abs (convertedAngle - this.previousConvertedAngle) > MAXIMUM_ANGLE_CHANGE_PER_FRAME)
      {
         if (convertedAngle > this.previousConvertedAngle)
         {
            convertedAngle = this.previousConvertedAngle + MAXIMUM_ANGLE_CHANGE_PER_FRAME;
         }
         else
         {
            convertedAngle = this.previousConvertedAngle - MAXIMUM_ANGLE_CHANGE_PER_FRAME;
         }
      }

      this.animation["jousterToOwnship"].time = (convertedAngle/180.0f) * this.animation["jousterToOwnship"].length;
      this.animation["jousterToOwnship"].speed = 0.0f;

      this.previousConvertedAngle = convertedAngle;
   }

   //------------------------------------------------------------------
   // Function (public): GotJousted - this is called one time to transition the jouster to its jousted state
   //------------------------------------------------------------------
   public void GotJousted(Vector3 ownshipVelocity)
   {
      this.fallingVelocity = ownshipVelocity;
      this.transform.parent = null;
      this.transform.Rotate (new Vector3(90.0f, 0.0f, 0.0f), Space.Self);
      this.mode = JousterModeEnum.FALLING;
   }

   private float previousConvertedAngle = 90.0f;
   private JousterModeEnum mode = JousterModeEnum.JOUSTING;
   private Vector3 fallingVelocity = Vector3.zero;

   // constants
   private const float MAXIMUM_ANGLE_CHANGE_PER_FRAME = 1.85f;

}

