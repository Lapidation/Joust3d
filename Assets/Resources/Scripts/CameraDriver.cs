using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: CameraDriver
//
// Description: This class handles the direction of the camera, which is attached to the ownship, which
//              acts as the player
//-------------------------------------------------------------------------------------------------------------

public class CameraDriver : MonoBehaviour
{
   //------------------------------------------------------------------
   // CameraModeEnum (enumeration): mode of the camera
   //------------------------------------------------------------------
   public enum CameraModeEnum
   {
      OPERATIONAL  = 0,
      DEATH_SPIRAL = 1
   }

   //------------------------------------------------------------------
   // Function (public): Start
   //------------------------------------------------------------------
	void Start()
   {
      this.name = "cameraDriver";
	}

   //------------------------------------------------------------------
   // Function (public): SetParent - chains this transform to the parent and sets its mode
   //------------------------------------------------------------------
   public void SetParent(Transform parentTransform, Ownship.OwnshipModeEnum ownshipMode)
   {
      this.cameraGo = GameObject.FindWithTag("MainCamera");
      this.cameraGo.transform.parent = parentTransform;

      // this allows a look-at point which is always straight ahead
      this.straightAhead = parentTransform.TransformPoint(new Vector3(0.0f, 0.0f, 1.0f));

      // determine the camera mode based on the current ownship mode
      if (ownshipMode == Ownship.OwnshipModeEnum.DEAD)
      {
         this.mode = CameraModeEnum.DEATH_SPIRAL;
      }
      else
      {
         this.mode = CameraModeEnum.OPERATIONAL;
      }
   }

   //------------------------------------------------------------------
   // Function (public): Update - main function for processing
   //------------------------------------------------------------------
	void Update()
   {
      // on every press of 'f' key, allow an auto-acquisition
      if (Input.GetKey (KeyCode.F))
      {
         this.automaticAcquisitionTimer = 0.0f;
      }

      // update the auto-acquisition timer
      if (this.automaticAcquisitionTimer < TOTAL_TIME_FOR_AUTO_ACQUISTION)
      {
         this.automaticAcquisitionTimer += Time.deltaTime;
      }

      float   optimalDistanceToTarget = float.MaxValue;
      float   optimalAngleToTarget = 0.0f;

      Vector3 lookAtPosition = this.straightAhead;
      Vector3 currentEnemyPosition = Vector3.zero;

      for (int i=0;i<this.enemyList.Count;++i)
      {
         //currentEnemyPosition = ((GlobalTypes.EnemyType)this.enemyList[i]).go.transform.position;
         currentEnemyPosition = ((GlobalTypes.EnemyType)this.enemyList[i]).script.GetJousterPosition();

         // get angle to this target
         Quaternion rotationToTarget   = Quaternion.LookRotation(currentEnemyPosition - this.cameraGo.transform.position);
         Quaternion rotationToStraight = Quaternion.LookRotation(this.straightAhead   - this.cameraGo.transform.position);
         float angle = Quaternion.Angle(rotationToTarget, rotationToStraight);

         // get the distance to this target
         float distanceToTargetSquared = ((this.cameraGo.transform.position.x - currentEnemyPosition.x)*
                                          (this.cameraGo.transform.position.x - currentEnemyPosition.x)) +
                                         ((this.cameraGo.transform.position.y - currentEnemyPosition.y)*
                                          (this.cameraGo.transform.position.y - currentEnemyPosition.y)) +
                                         ((this.cameraGo.transform.position.z - currentEnemyPosition.z)*
                                          (this.cameraGo.transform.position.z - currentEnemyPosition.z));

         // calculate the angle between the vector to the target and the vector to straight ahead
         // don't allow look angles too far behind the ownship
         float angleBetweenTargets = Vector3.Angle(currentEnemyPosition - this.cameraGo.transform.position,
                                                   this.straightAhead - this.cameraGo.transform.position);

         if (angleBetweenTargets < MAXIMUM_ANGLE_BETWEEN_TARGETS &&
             (Mathf.Abs (angle) < ANGLE_THRESHOLD_TO_LOOK_AT_TARGET ||
              distanceToTargetSquared < MIN_DISTANCE_SQUARED_FOR_ACQUISITION ||
              this.automaticAcquisitionTimer < TOTAL_TIME_FOR_AUTO_ACQUISTION ||
              this.mode == CameraModeEnum.DEATH_SPIRAL))
         {
            if (distanceToTargetSquared < optimalDistanceToTarget)
            {
               lookAtPosition = currentEnemyPosition;
               optimalDistanceToTarget = distanceToTargetSquared;
            }
         }
      }
      SmoothLookAt(lookAtPosition);
   }

   //------------------------------------------------------------------
   // Function (public): SetEnemyList - passes the enemy list in for use in acquiring the targets with the camera
   //------------------------------------------------------------------
   public void SetEnemyList(System.Collections.ArrayList enemyList)
   {
      this.enemyList = enemyList;
   }

   //------------------------------------------------------------------
   // Function (private): SmoothLookAt - interpolates between frames to look at a position smoothly
   //------------------------------------------------------------------
   private void SmoothLookAt(Vector3 lookAtPosition)
   {
      Quaternion rotation = Quaternion.LookRotation(lookAtPosition - this.cameraGo.transform.position);
      this.cameraGo.transform.rotation =
         Quaternion.Slerp(this.cameraGo.transform.rotation, rotation, Time.deltaTime * SMOOTH_LOOK_DAMPENING_FACTOR);
   }

   // private variables
   private GameObject cameraGo;
   private float time;
   private Vector3 straightAhead = new Vector3(0.0f, 0.0f, 1.0f);
   private float automaticAcquisitionTimer = TOTAL_TIME_FOR_AUTO_ACQUISTION;
   private CameraModeEnum mode = CameraModeEnum.OPERATIONAL;
   private System.Collections.ArrayList enemyList;

   // constants
   private const float SMOOTH_LOOK_DAMPENING_FACTOR = 2.0f;
   private const float ANGLE_THRESHOLD_TO_LOOK_AT_TARGET = 35.0f;
   private const float MIN_DISTANCE_SQUARED_FOR_ACQUISITION = 3000.0f;
   private const float TOTAL_TIME_FOR_AUTO_ACQUISTION = 3.0f;
   private const float MAXIMUM_ANGLE_BETWEEN_TARGETS = 120.0f;
}


