using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: EnemyCircling
//
// Description: This is the circling enemy
//-------------------------------------------------------------------------------------------------------------
public class EnemyCircling : Enemy
{
	void Start()
   {
      this.name = GlobalTypes.CIRCLING_ENEMY_NAME;

      // configure this object's flight dynamics object
      this.flightDynamics = new FlightDynamics(0.045f,//SPEED_UP_INCREMENT
                                               0.03f, //SPEED_DOWN_INCREMENT
                                               0.95f, //YAW_INCREMENT
                                               0.65f, //MAX_SPEED
                                               30.0f, //FLAP_INITIAL_VELOCITY
                                               0.0f,  //ROLL_INCREMENT
                                               0.0f,  //MAX_ROLL
											   false);//Ownship

      // scale this model so it shows up correctly - should be fixed in blender instead?
      this.transform.localScale += new Vector3(1000.0f, 1000.0f, 1000.0f);
      this.mode = EnemyModeEnum.ALIVE;

      TIME_BETWEEN_FLAPS = 0.40f;
	}

   //------------------------------------------------------------------
   // Function (protected): Initialize
   //------------------------------------------------------------------
   public override void Initialize(Transform      parentTransform,
                                   ref GameObject go)
   {
      this.ostrichLegsRight.go = (GameObject)Instantiate(Resources.Load("ostrich legs"));
      this.ostrichLegsRight.go.AddComponent<AudioSource>();
      this.ostrichLegsRight.script = (OstrichLegs)this.ostrichLegsRight.go.AddComponent(typeof(OstrichLegs));
      this.ostrichLegsRight.script.Initialize(false);
      this.ostrichLegsRight.go.transform.parent = this.transform;

      this.ostrichLegsLeft.go = (GameObject)Instantiate(Resources.Load("ostrich legs"));
      this.ostrichLegsLeft.go.AddComponent<AudioSource>();
      this.ostrichLegsLeft.script = (OstrichLegs)this.ostrichLegsLeft.go.AddComponent(typeof(OstrichLegs));
      this.ostrichLegsLeft.script.Initialize(true);
      this.ostrichLegsLeft.go.transform.parent = this.transform;

      this.jouster.go = (GameObject)Instantiate(Resources.Load("jouster 4"));
      this.jouster.script = (Jouster)this.jouster.go.AddComponent(typeof(Jouster));
      this.jouster.go.transform.localScale = 0.4f * this.jouster.go.transform.localScale;
	   this.jouster.go.transform.Translate(new Vector3(0.028f, 0.008f, 0.0f));
      this.jouster.go.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
      this.jouster.go.transform.parent = this.transform;

      this.ostrichWings.go = (GameObject)Instantiate(Resources.Load("ostrich wing 2"));
      this.ostrichWings.go.AddComponent<AudioSource>();      
      this.ostrichWings.script = (OstrichWings)this.ostrichWings.go.AddComponent(typeof(OstrichWings));
      this.ostrichWings.script.Initialize(false);
      this.ostrichWings.go.transform.parent = this.transform;


      // set the parent transform
      this.transform.parent = parentTransform;

      // add a box collider to the enemy
      go.AddComponent<Rigidbody>();
      go.AddComponent<BoxCollider>();
      go.collider.isTrigger = true;
      go.collider.name = go.name;
      ((BoxCollider)go.collider).size   = COLLIDER_SIZE * Vector3.one;
      ((BoxCollider)go.collider).center =  new Vector3(0.001f, 0.004f, 0.0f);
      go.rigidbody.useGravity = false;
   }

   //------------------------------------------------------------------
   // Function (protected): Update
   //------------------------------------------------------------------
	public void Update()
   {
      switch (this.mode)
      {
         case EnemyModeEnum.ALIVE:
         case EnemyModeEnum.JOUSTED:

            // run this enemy's flight model
            this.time += Time.deltaTime;
            if (this.time > TIME_BETWEEN_FLAPS)
            {
               this.time = 0.0f;
               if (this.transform.position.y < ALTITUDE || this.flightDynamics.IsOutsideArena(this.transform))
               {
                  this.ostrichWings.script.FlapWings();
                  this.flightDynamics.Flap(this.transform);
               }
               this.flightDynamics.SpeedUp();
               this.flightDynamics.TurnLeft(this.transform);
            }
            this.Move();
            break;
         case EnemyModeEnum.REMOVE_ME:
            break;
         default:
            Debug.Log ("unhandled mode in EnemyCircling::Update()");
            break;
      }
	}

   private const float ALTITUDE = 85.0f;
   private const float COLLIDER_SIZE = 0.01f;
}