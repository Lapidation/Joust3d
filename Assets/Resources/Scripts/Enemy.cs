using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: Enemy (abstract base class)
//
// Description: This is the parent class of all enemies
//-------------------------------------------------------------------------------------------------------------
public abstract class Enemy : MonoBehaviour
{

   public enum EnemyModeEnum
   {
      ALIVE     = 0,
      JOUSTED   = 1,
      REMOVE_ME = 2
   }

	void Start()
   {
	}

	public void Update()
   {
	}

   public EnemyModeEnum GetMode()
   {
      return this.mode;
   }

   public bool DoKickingAnimation()
   {
      // first, calculate the horizontal distance between ownship and enemy (don't bother with square root)
      float xzSqrDistance = ((this.ownshipTransform.position.x - this.transform.position.x)*
                             (this.ownshipTransform.position.x - this.transform.position.x)) +
                            ((this.ownshipTransform.position.z - this.transform.position.z)*
                             (this.ownshipTransform.position.z - this.transform.position.z));

      return (xzSqrDistance < 3000.0f && (this.ownshipTransform.position.y < this.transform.position.y));
   }

   public float GetJousterAnimationFrame()
   {

      // first, calculate the horizontal distance between ownship and enemy (don't bother with square root)
      float xzSqrDistance = ((this.ownshipTransform.position.x - this.transform.position.x)*
                             (this.ownshipTransform.position.x - this.transform.position.x)) +
                            ((this.ownshipTransform.position.z - this.transform.position.z)*
                             (this.ownshipTransform.position.z - this.transform.position.z));

      if (xzSqrDistance < 12000.0f)
      {
         float enemyYaw = -this.transform.eulerAngles.y + 180.0f;
         Vector2 enemyVector = new Vector2(Mathf.Cos (enemyYaw * Mathf.Deg2Rad), Mathf.Sin (enemyYaw * Mathf.Deg2Rad));
         Vector2 vectorToOwnship = new Vector2(this.ownshipTransform.position.x - this.transform.position.x,
                                               this.ownshipTransform.position.z - this.transform.position.z);
         float angleToOwnship = Vector2.Angle(enemyVector, vectorToOwnship);


         // use the cross product to determine the sign
         // note that when converting between Vector2s and Vector3s, you cannot implicitly cast because it zeros the z term
         // instead set the y axis to zero and use the y coordinate for the z axis
         if (Vector3.Cross(new Vector3(enemyVector.x, 0.0f, enemyVector.y),
                           new Vector3(vectorToOwnship.x, 0.0f, vectorToOwnship.y)).y > 0.0f)
         {
            angleToOwnship *= -1.0f;
         }
         //Debug.Log ("enemyVector: " + enemyVector + " vectorToOwnship: " + vectorToOwnship + " angle: " + angleToOwnship);
         return angleToOwnship;
      }
      else
      {
         return 0.0f;
      }
   }

   void OnTriggerEnter(Collider collider)
   {
      if (collider.enabled && this.mode == EnemyModeEnum.ALIVE)
      {
         switch (collider.name)
         {
            case GlobalTypes.OWNSHIP_NAME:
               if (this.transform.position.y > collider.rigidbody.position.y)
               {
                  this.flightDynamics.CueExternalForce(new Vector3(0.0f, 100.0f, 0.0f));
               }
               else
               {
                  this.mode = EnemyModeEnum.JOUSTED;
                  float theta = (-this.ownshipTransform.eulerAngles.y + 90.0f) * Mathf.Deg2Rad;
                  this.jouster.script.GotJousted(new Vector3(Mathf.Cos(theta) * this.ownshipVelocity,
                                                             0.0f,
                                                             Mathf.Sin(theta) * this.ownshipVelocity));

                  this.flightDynamics.FreeBird();
               }
               break;
            default:
               break;
         }
      }
   }

   public void SetOwnshipTransform(Transform transform, float velocity)
   {
      this.ownshipTransform = transform;
      this.ownshipVelocity = velocity;
   }

   protected void Move()
   {
      switch (this.mode)
      {
         case EnemyModeEnum.ALIVE:
            // for every frame, move and animate the ostrich legs as needed
            this.flightDynamics.Move(this.transform);
            this.ostrichLegsLeft .script.Animate(this.flightDynamics.OnGround(),
                                                 this.DoKickingAnimation(),
                                                 this.flightDynamics.GetSpeed());
            this.ostrichLegsRight.script.Animate(this.flightDynamics.OnGround(),
                                                 this.DoKickingAnimation(),
                                                 this.flightDynamics.GetSpeed());
            this.jouster.script.RotateJousterToOwnship(GetJousterAnimationFrame());
            break;
        case EnemyModeEnum.JOUSTED:
            // for every frame, move and animate the ostrich legs as needed
            this.flightDynamics.SpeedUp();
            if (this.freeBirdFlapTimer >= TIME_BETWEEN_FREE_BIRD_FLAPS)
            {
               this.flightDynamics.Flap(this.transform);
               this.freeBirdFlapTimer = 0.0f;
            }
            else
            {
               this.freeBirdFlapTimer += Time.deltaTime;
            }
            this.flightDynamics.Move(this.transform);
            this.jouster.script.Update();
            this.ostrichLegsLeft .script.Animate(this.flightDynamics.OnGround(),
                                                 false, // don't kick when free
                                                 this.flightDynamics.GetSpeed());
            this.ostrichLegsRight.script.Animate(this.flightDynamics.OnGround(),
                                                 false, // don't kick when free
                                                 this.flightDynamics.GetSpeed());
            break;
        case EnemyModeEnum.REMOVE_ME:
            break;
        default:
            Debug.Log ("unhandled case in Enemy::Move()");
            break;

      }
   }


   public abstract void Initialize(Transform      parentTransform,
                                   ref GameObject go);

   //------------------------------------------------------------------
   // Function (protected): GetJousterPosition (used to view the jousters instead of the birds)
   //------------------------------------------------------------------
   public Vector3 GetJousterPosition()
   {
      return this.jouster.go.transform.position;
   }

   protected float time;
   protected float TIME_BETWEEN_FLAPS = 0.08f;
   protected EnemyModeEnum mode;
   protected Transform ownshipTransform;
   protected float ownshipVelocity;
   protected FlightDynamics flightDynamics;

   // these objects are attached to all enemies
   protected GlobalTypes.JousterType jouster;
   protected GlobalTypes.OstrichLegsType ostrichLegsRight;
   protected GlobalTypes.OstrichLegsType ostrichLegsLeft;
   protected GlobalTypes.OstrichWingsType ostrichWings;

   private float freeBirdFlapTimer = 0.0f;
   private const float TIME_BETWEEN_FREE_BIRD_FLAPS = 0.8f;

}
