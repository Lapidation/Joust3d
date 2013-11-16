using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: FlightDynamics
//
// Description: This is class can be instantiated for an ownship or enemies to perform the flight dynamics.
//              It is configured through its constructor to allow for different flight characteristics.
//-------------------------------------------------------------------------------------------------------------
public class FlightDynamics
{
   private enum FlightModeEnum
   {
      CARRYING_JOUSTER = 0,
      FREE_BIRD        = 1,
      REMOVE_ME        = 2
   }

   public FlightDynamics(float speedUpIncrement,
                         float speedDownIncrement,
                         float yawIncrement,
                         float maxSpeed,
                         float flapInitialVelocity,
                         float rollIncrement,
                         float maxRoll,
                         bool  isOwnship)
   {
      SPEED_UP_INCREMENT = speedUpIncrement;
      SPEED_DOWN_INCREMENT = speedDownIncrement;
      YAW_INCREMENT = yawIncrement;
      MAX_SPEED = maxSpeed;
      FLAP_INITIAL_VELOCITY = flapInitialVelocity;
      ROLL_INCREMENT = rollIncrement;
      MAX_ROLL = maxRoll;
      //ROLL_INCREMENT = 0.0f;
      //MAX_ROLL = 0.0f;
      this.isOwnship = isOwnship; // I wish this wasn't required but couldn't find a way to make this all correct

      this.go = new GameObject();
      this.go.AddComponent<AudioSource>();
      this.go.name = "flight game object";

      this.wallHitClip  = (AudioClip)Object.Instantiate(Resources.Load("wall hit"));
      this.windGustClip = (AudioClip)Object.Instantiate(Resources.Load("wind gust"));

      this.go.audio.volume = 1.0f;
      this.go.audio.minDistance = 8.0f;
      this.go.audio.maxDistance = 30.0f;
      this.go.audio.dopplerLevel = 0.3f;
      this.go.audio.playOnAwake = false;
      this.go.audio.panLevel = 1.0f;
   }

   //------------------------------------------------------------------
   // Function (public): TurnLeft
   //------------------------------------------------------------------
   public void TurnLeft(Transform transform)
   {
      if (!this.outsideArena && this.mode == FlightModeEnum.CARRYING_JOUSTER)
      {
         transform.Rotate(new Vector3(0.0f, -YAW_INCREMENT, 0.0f), Space.Self);
         if (!this.onGround)
         {
            transform.Rotate(Vector3.forward, ROLL_INCREMENT, Space.Self);
         }
      }
   }

   //------------------------------------------------------------------
   // Function (public): TurnRight
   //------------------------------------------------------------------
   public void TurnRight(Transform transform)
   {
      if (!this.outsideArena && this.mode == FlightModeEnum.CARRYING_JOUSTER)
      {
         transform.Rotate(new Vector3(0.0f, YAW_INCREMENT, 0.0f), Space.Self);
         if (!this.onGround)
         {
            transform.Rotate(Vector3.forward, -ROLL_INCREMENT, Space.Self);
         }
      }
   }

   //------------------------------------------------------------------
   // Function (public): SpeedUp
   //------------------------------------------------------------------
   public void SpeedUp()
   {
      if (!this.outsideArena)
      {
         this.speed += SPEED_UP_INCREMENT;
         if (this.speed > MAX_SPEED)
         {
            this.speed = MAX_SPEED;
         }
      }
   }

   //------------------------------------------------------------------
   // Function (public): SlowDown
   //------------------------------------------------------------------
   public void SlowDown()
   {
      if (!this.outsideArena)
      {
         this.speed -= SPEED_DOWN_INCREMENT;
         if (this.speed < 0.0f)
         {
            this.speed = 0.0f;
         }
      }
   }

   //------------------------------------------------------------------
   // Function (public): Flap
   //------------------------------------------------------------------
   public void Flap(Transform transform)
   {
      if (!this.outsideArena)
      {
         this.ySpeed += FLAP_INITIAL_VELOCITY;

         float theta = 0.0f;
         if (this.isOwnship)
         {
            theta = (-transform.eulerAngles.y + 90.0f) * Mathf.Deg2Rad;
         }
         else
         {
            theta = (-transform.eulerAngles.y + 180.0f) * Mathf.Deg2Rad;
         }

         Vector3 lookAngle = new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin (theta));
         this.moveVector = Vector3.Slerp(this.moveVector, lookAngle, LOOK_ANGLE_TO_MOVE_FACTOR_FLAP);
      }
   }

   //------------------------------------------------------------------
   // Function (public): CueExternalForce - used to bounce an entity -
   //                                       only the y direction is supported right now
   //------------------------------------------------------------------
   public void CueExternalForce(Vector3 force)
   {
      this.ySpeed += force.y;
   }

   //------------------------------------------------------------------
   // Function (public): OnGround
   //------------------------------------------------------------------
   public bool OnGround()
   {
      return this.onGround;
   }

   //------------------------------------------------------------------
   // Function (public): GetSpeed
   //------------------------------------------------------------------
   public float GetSpeed()
   {
      return this.speed;
   }

   //------------------------------------------------------------------
   // Function (public): Move - call this function to actually move the entity
   //------------------------------------------------------------------
   public void Move(Transform transform)
   {
      // set the on ground flag
      this.onGround = (transform.position.y < GlobalTypes.ON_GROUND_THRESHOLD);

      // stabilize the roll when no roll command is input
      if (transform.eulerAngles.z < 180.0f)
      {
         transform.Rotate(Vector3.forward, -transform.eulerAngles.z * ROLL_WASHOUT_FACTOR, Space.Self);
      }
      else
      {
         transform.Rotate(Vector3.forward, -(transform.eulerAngles.z - 360.0f) * ROLL_WASHOUT_FACTOR, Space.Self);
      }

      // this makes sure the ownship is dampens out to straight and level
      transform.Rotate(Vector3.right, -transform.eulerAngles.x * PITCH_WASHOUT_FACTOR, Space.Self);

      // get the delta altitude to move this frame based on kinematics equations
      this.ySpeed = this.ySpeed + (GlobalTypes.GRAVITY * Time.deltaTime);

      // if we hit the ceiling, reverse the speed and dampen it out
      if (transform.position.y > CEILING && this.ySpeed > 0.0f)
      {
         this.ySpeed = -(this.ySpeed * CEILING_DAMPEN_FACTOR);
      }

      // we have hit the ground - don't change any altitude
      if (transform.position.y < 0.0f)
      {
         transform.Translate(new Vector3(0.0f, -transform.position.y, 0.0f));
         this.ySpeed = -(this.ySpeed * GlobalTypes.GROUND_BOUNCE_DAMPEN_FACTOR);
      }

      // rotate the angles to change in yaw into the coordinate frame used in unity
      // I have no idea why I have to do this differently for the ownship - it should not have to be this way
      // but with this change, the enemies face the correct way with their flight dynamics as well as the camera
      float theta = 0.0f;
      if (this.isOwnship)
      {
         theta = (-transform.eulerAngles.y + 90.0f) * Mathf.Deg2Rad;
      }
      else
      {
         theta = (-transform.eulerAngles.y + 180.0f) * Mathf.Deg2Rad;
      }

      // dampen out all negative speeds
      if (this.speed < 0.0f)
      {
         this.speed *= NEGATIVE_SPEED_DAMPEN_FACTOR;
      }

      // handle the move vector when on the ground
      if (this.onGround)
      {
         Vector3 lookAngle = new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin (theta));

         if (this.timeOnGround < 1.5f)
         {
            this.moveVector = Vector3.Slerp(this.moveVector, lookAngle, LOOK_ANGLE_TO_MOVE_FACTOR_ON_GROUND);
            this.timeOnGround += Time.deltaTime;
         }
         else
         {
            this.moveVector = Vector3.Slerp(this.moveVector, lookAngle, 1.0f);
         }
      }
      else
      {
         this.timeOnGround = 0.0f;
      }

      // perform the yaw and the vertical movements
      transform.Translate((new Vector3(this.moveVector.x * this.speed, this.ySpeed * Time.deltaTime, this.moveVector.z * this.speed)), Space.World);

      if (this.isOwnship)
      {
         CheckArenaBoundariesOwnship(transform);
      }
      else
      {
         CheckArenaBoundariesEnemy(transform);
      }
   }

   //------------------------------------------------------------------
   // Function (private): CheckArenaBoundariesEnemy - bounces the player off the wall if needed
   //------------------------------------------------------------------
   private void CheckArenaBoundariesEnemy(Transform transform)
   {

      if (IsOutsideArena(transform))
      {
         // do wall collisions if currently jousting, or if this is a free bird and it has hit the wall
         if ( this.mode == FlightModeEnum.CARRYING_JOUSTER ||
             (this.mode == FlightModeEnum.FREE_BIRD && transform.position.y < ARENA_WALL_HEIGHT))
         {
            float playerHeading = (-transform.eulerAngles.y + 180.0f) * Mathf.Deg2Rad;
            Vector2 playerVector = new Vector2(Mathf.Cos (playerHeading), Mathf.Sin (playerHeading));
            Vector2 wallVector   = new Vector2(-transform.position.z, transform.position.x);

            float angleOfIncidence = Vector2.Angle(playerVector, wallVector);
            if (angleOfIncidence > 90.0f)
            {
               transform.Rotate (new Vector3(0.0f,  YAW_INCREMENT * 1.8f, 0.0f), Space.Self);
            }
            else
            {
               transform.Rotate (new Vector3(0.0f, -YAW_INCREMENT * 1.8f, 0.0f), Space.Self);
            }
            SpeedUp ();
         }
      }
   }

   //------------------------------------------------------------------
   // Function (private): CheckArenaBoundariesOwnship - bounces the player off the wall if needed
   //------------------------------------------------------------------
   private void CheckArenaBoundariesOwnship(Transform transform)
   {
      if (IsOutsideArena(transform) && !this.outsideArena)
      {
         if (transform.position.y > ARENA_WALL_HEIGHT)
         {
            // get angle of incidence between current vector and wall angle (derivative of the circle)
            Vector2 wallVector   = new Vector2(-transform.position.z, transform.position.x);
            float angleOfIncidence = Vector2.Angle (new Vector2(this.moveVector.x, this.moveVector.z), wallVector);

            float angleToRotate = ((2.0f * angleOfIncidence) * Mathf.Deg2Rad);

            Vector3 newDirection = new Vector3(this.moveVector.x * Mathf.Cos (angleToRotate) + this.moveVector.z * Mathf.Sin (angleToRotate),
                                               0.0f,
                                               this.moveVector.x * Mathf.Sin (angleToRotate) - this.moveVector.z * Mathf.Cos (angleToRotate));
				
            this.moveVector = newDirection;
            this.speed *= SKY_BOUNCE_DAMPEN_FACTOR;
				Debug.Log("above wall");
            // play wind gust audio
            this.go.audio.clip = this.windGustClip;
            this.go.transform.position = transform.position;
            this.go.audio.Play();

         }
         else
         {
			if(this.speed > 0)
			{
               this.speed = WALL_BOUNCE_FACTOR;
			   Debug.Log("speed is greater than zero");
			}
			else if(this.speed < 0)
			{
			   this.speed = WALL_BOUNCE_FACTOR;
			   Debug.Log("speed is less than zero");
			}
			else
			{
			   //Speed is zero
			   this.speed = WALL_BOUNCE_FACTOR;
			   Debug.Log("speed is zero");
			}
				
            // play wall hit audio
            this.go.audio.clip = this.wallHitClip;
            this.go.transform.position = transform.position;
            this.go.audio.Play();
         }

         this.outsideArena = true;
      }
      else
      {
         this.outsideArena = false;
      }
   }

   //------------------------------------------------------------------
   // Function (private): IsOutsideArena - returns true if this position is outside the arena
   //------------------------------------------------------------------
   public bool IsOutsideArena(Transform transform)
   {
      if (this.isOwnship)
      {
         return (transform.position.x*transform.position.x +
                 transform.position.z*transform.position.z) > ARENA_WALL_RADIUS*ARENA_WALL_RADIUS;
      }
      else
      {
         return (transform.position.x*transform.position.x +
                 transform.position.z*transform.position.z) > ARENA_WALL_RADIUS_ENEMIES*ARENA_WALL_RADIUS_ENEMIES;
      }
   }

   //------------------------------------------------------------------
   // Function (public): DeathSpiral - the ownship calls this when it hath been jousted
   //------------------------------------------------------------------
   public void DeathSpiral(Transform transform)
   {
      // get the delta altitude to move this frame based on kinematics equations
      if (this.ySpeed > 0.0f)
      {
         this.ySpeed *= -1.0f;
      }
      this.ySpeed = this.ySpeed + (GlobalTypes.GRAVITY * Time.deltaTime);
      // we have hit the ground - don't change any altitude
      if (transform.position.y < 0.0f)
      {
         transform.Translate(new Vector3(0.0f, -transform.position.y, 0.0f));
      }

      // rotate the angles to change in yaw into the coordinate frame used in unity
      float theta = (-transform.eulerAngles.y + 90.0f) * Mathf.Deg2Rad;

      if (this.onGround)
      {
         this.moveVector = new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta));
      }

      // perform the yaw and the vertical movements
      transform.Translate((new Vector3(this.moveVector.x * this.speed, this.ySpeed * Time.deltaTime, this.moveVector.z * this.speed)), Space.World);
      this.speed *= 0.95f;
   }

   //------------------------------------------------------------------
   // Function (public): FreeBird - this function frees the bird from its jouster's grips and lets it fly away
   //------------------------------------------------------------------
   public void FreeBird()
   {
      this.MAX_SPEED = FREE_BIRD_MAXIMUM_SPEED;
      this.mode = FlightModeEnum.FREE_BIRD;
   }

   // private variables
   private float speed = 0.0f;
   private float ySpeed = 0.0f;
   private float SPEED_UP_INCREMENT = 0.045f;
   private float SPEED_DOWN_INCREMENT = 0.3f;
   private float YAW_INCREMENT = .95f;
   private float MAX_SPEED = 2.0f;
   private float FLAP_INITIAL_VELOCITY = 40.0f;
   private float ROLL_INCREMENT = 1.0f;
   private float MAX_ROLL = 10.0f;
   private bool onGround = false;

   private bool isOwnship = false;
   private bool outsideArena = false;
   private float timeOnGround = 0.0f;

   private Vector3 moveVector = Vector3.zero;

   private FlightModeEnum mode = FlightModeEnum.CARRYING_JOUSTER;

   // audio support
   private GameObject go;
   AudioClip wallHitClip;
   AudioClip windGustClip;

   // constants
   private const float ROLL_WASHOUT_FACTOR = .05f;
   private const float PITCH_WASHOUT_FACTOR = .02f;
   private const float CEILING = 1000.0f;
   private const float CEILING_DAMPEN_FACTOR = 0.5f;

   private const float ARENA_WALL_RADIUS =  775.0f;
   private const float ARENA_WALL_RADIUS_ENEMIES = 715.0f;
   private const float ARENA_WALL_HEIGHT = 20.0f;
   private const float NEGATIVE_SPEED_DAMPEN_FACTOR = 0.95f;
   private const float LOOK_ANGLE_TO_MOVE_FACTOR_FLAP = 0.48f;
   private const float LOOK_ANGLE_TO_MOVE_FACTOR_ON_GROUND = 0.05f;
   private const float WALL_BOUNCE_FACTOR = -0.95f;
   private const float SKY_BOUNCE_DAMPEN_FACTOR = 0.85f;

   private const float FREE_BIRD_MAXIMUM_SPEED = 7.0f;
}