using UnityEngine;

//-------------------------------------------------------------------------------------------------------------
// Class: GlobalTypes
//
// Description: Global types and utilities
//-------------------------------------------------------------------------------------------------------------

public class GlobalTypes
{
   public struct OwnshipType
   {
      public GameObject go;
      public Ownship script;
   }

   public struct CameraDriverType
   {
      public GameObject go;
      public CameraDriver script;
   }

   public struct OstrichType
   {
      public GameObject go;
      public Ostrich script;
   }

   public struct OstrichLegsType
   {
      public GameObject go;
      public OstrichLegs script;
   }

   public struct OstrichWingsType
   {
      public GameObject go;
      public OstrichWings script;
   }

   public struct JousterType
   {
      public GameObject go;
      public Jouster script;
   }
	
   public struct LanceType
   {
      public GameObject go;
      public Lance script;
   }	

   public struct EnemyManagerType
   {
      public GameObject go;
      public EnemyManager script;
   }

   public struct EnemyType
   {
      public GameObject go;
      public Enemy script;
   }

   public struct GUIInstructionsType
   {
      public GameObject go;
      public GUIInstructions script;
   }

   // declare public constant strings for collider names
   public const string OWNSHIP_NAME        = "ownship";
   public const string CIRCLING_ENEMY_NAME = "circling enemy";
   public const string VERTICAL_ENEMY_NAME = "vertical enemy";

   public const float GRAVITY = -9.8f * 6.0f;
   public const float ON_GROUND_THRESHOLD = 2.0f;
   public const float GROUND_BOUNCE_DAMPEN_FACTOR = 0.1f;   

   //-------------------------------------------------------------------------------------------
   // function (public): RotateVector - rotates a vector by the given orientation
   //-------------------------------------------------------------------------------------------
   public Vector3 RotateVector(Vector3 orientation, Vector3 v)
   {
      float[,] rotationMatrix = new float[3,3];
      // convert the euler angles to radians for trig operations
      // and compute the rotation matrix
      Vector3 rotation = orientation * Mathf.Deg2Rad;
      rotationMatrix[0,0] =  Mathf.Cos(rotation.y);
      rotationMatrix[0,1] =  0.0f;
      rotationMatrix[0,2] =  Mathf.Sin(rotation.y);
      rotationMatrix[1,0] =  Mathf.Sin(rotation.x)*Mathf.Sin(rotation.y);
      rotationMatrix[1,1] =  Mathf.Cos(rotation.x);
      rotationMatrix[1,2] = -Mathf.Sin(rotation.x)*Mathf.Cos(rotation.y);
      rotationMatrix[2,0] = -Mathf.Cos(rotation.x)*Mathf.Sin(rotation.y);
      rotationMatrix[2,1] =  Mathf.Sin(rotation.x);
      rotationMatrix[2,2] =  Mathf.Cos(rotation.x)*Mathf.Cos(rotation.y);

      Vector3 answer;
      answer.x = rotationMatrix[0,0] * v.x +
                 rotationMatrix[0,1] * v.y +
                 rotationMatrix[0,2] * v.z;
      answer.y = rotationMatrix[1,0] * v.x +
                 rotationMatrix[1,1] * v.y +
                 rotationMatrix[1,2] * v.z;
      answer.z = rotationMatrix[2,0] * v.x +
                 rotationMatrix[2,1] * v.y +
                 rotationMatrix[2,2] * v.z;

      return answer;
   }
}


