using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: Ownship
//
// Description: This class represents the player
//-------------------------------------------------------------------------------------------------------------
public class Ownship : MonoBehaviour
{
   public enum OwnshipModeEnum
   {
      ALIVE = 0,
      DEAD  = 1
   }

   //-------------------------------------------------------------------------------------------
   // Function (public): Start - initial function
   //-------------------------------------------------------------------------------------------
	void Start()
   {
      this.name = GlobalTypes.OWNSHIP_NAME;
      this.cameraDriver.go = new GameObject();
      this.cameraDriver.script = (CameraDriver)this.cameraDriver.go.AddComponent(typeof(CameraDriver));
      this.cameraDriver.script.SetParent(this.transform, this.mode);

      this.ostrichLegsRight.go = (GameObject)Instantiate(Resources.Load("ostrich legs"));
      this.ostrichLegsRight.go.AddComponent<AudioSource>();
      this.ostrichLegsRight.script = (OstrichLegs)this.ostrichLegsRight.go.AddComponent(typeof(OstrichLegs));
      this.ostrichLegsRight.script.Initialize(false);
      this.ostrichLegsRight.script.SetMaximumVolume(0.15f);      
      this.ostrichLegsRight.go.transform.parent = this.transform;

      this.ostrichLegsLeft.go = (GameObject)Instantiate(Resources.Load("ostrich legs"));
      this.ostrichLegsLeft.go.AddComponent<AudioSource>();
      this.ostrichLegsLeft.script = (OstrichLegs)this.ostrichLegsLeft.go.AddComponent(typeof(OstrichLegs));
      this.ostrichLegsLeft.script.Initialize(true);
      this.ostrichLegsLeft.script.SetMaximumVolume(0.15f);

      this.ostrichLegsLeft.go.transform.parent = this.transform;

      this.ostrichWings.go = (GameObject)Instantiate(Resources.Load("ostrich wing 2"));
      this.ostrichWings.go.AddComponent<AudioSource>();
      this.ostrichWings.script = (OstrichWings)this.ostrichWings.go.AddComponent(typeof(OstrichWings));
      this.ostrichWings.script.Initialize(true); // tell this object it is attached to the ownship
      this.ostrichWings.go.transform.parent = this.transform;
      this.ostrichWings.go.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f), Space.Self);
      this.ostrichWings.go.transform.Translate(new Vector3(0.0f, -0.25f, -0.2f));
      this.ostrichWings.go.transform.localScale = Vector3.one * 25.0f;

      Renderer renderer = this.ostrichWings.go.transform.Find("Cylinder_001").GetComponent<Renderer>();
      for (int i=0; i<renderer.materials.Length; ++i)
      {
         renderer.materials[i].color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
      }

      renderer = this.ostrichWings.go.transform.Find("Cylinder_002").GetComponent<Renderer>();
      for (int i=0; i<renderer.materials.Length; ++i)
      {
         renderer.materials[i].color = new Color(0.2f, 0.2f, 0.2f, OSTRICH_TRANSPARENCY);
      }

      renderer = this.ostrichWings.go.transform.Find("Cylinder_003").GetComponent<Renderer>();
      for (int i=0; i<renderer.materials.Length; ++i)
      {
         renderer.materials[i].color = new Color(0.2f, 0.2f, 0.2f, OSTRICH_TRANSPARENCY);
      }

      renderer = this.ostrichWings.go.transform.Find("Cylinder").GetComponent<Renderer>();
      for (int i=0; i<renderer.materials.Length; ++i)
      {
         renderer.materials[i].color = new Color(0.2f, 0.2f, 0.2f, OSTRICH_TRANSPARENCY);
      }

      renderer = this.ostrichWings.go.transform.Find("ostrich_neck").GetComponent<Renderer>();
      for (int i=0; i<renderer.materials.Length; ++i)
      {
         renderer.materials[i].color = new Color(0.0f, 0.0f, 0.0f, OSTRICH_TRANSPARENCY/4.0f);
      }


      this.lance.go = (GameObject)Instantiate(Resources.Load("lance"));
      this.lance.script = (Lance)this.lance.go.AddComponent(typeof(Lance));
      this.lance.go.transform.Rotate (new Vector3(0.0f, 180.0f, 0.0f), Space.Self);
      this.lance.go.transform.Translate (new Vector3(-0.5f, -0.5f, 0.75f));
      this.lance.go.transform.parent = this.transform;

      this.flightDynamics = new FlightDynamics(0.045f,//SPEED_UP_INCREMENT
                                               0.03f, //SPEED_DOWN_INCREMENT
                                               1.15f, //YAW_INCREMENT
                                               2.0f,  //MAX_SPEED
                                               30.0f, //FLAP_INITIAL_VELOCITY
                                               0.0f,//0.75f, //ROLL_INCREMENT
                                               10.0f, //MAX_ROLL
                                               true);
   }

   //-------------------------------------------------------------------------------------------
   // Function (public): Initialize - passes in this script's game object and sets up the collider
   //-------------------------------------------------------------------------------------------
   public void Initialize(ref GameObject go)
   {
      // add a sphere collider to the ownship
      go.AddComponent<Rigidbody>();
      go.AddComponent<BoxCollider>();
      go.collider.isTrigger = true;
      go.collider.name = GlobalTypes.OWNSHIP_NAME;
      ((BoxCollider)go.collider).size   = COLLIDER_SIZE * Vector3.one;
      ((BoxCollider)go.collider).center =  Vector3.zero;
      go.rigidbody.useGravity = false;
   }

   //-------------------------------------------------------------------------------------------
   // Function (public): OnTriggerEnter - called when a collider hits the one attached to this object
   //-------------------------------------------------------------------------------------------
   void OnTriggerEnter(Collider collider)
   {
      if (collider.enabled)
      {
         if (this.transform.position.y > collider.rigidbody.position.y)
         {
           this.flightDynamics.CueExternalForce(new Vector3(0.0f, 60.0f, 0.0f));
         }
         else
         {
            this.flightDynamics.CueExternalForce(new Vector3(0.0f, -100.0f, 0.0f));
            this.time = 0.0f;
            this.mode = OwnshipModeEnum.DEAD;
         }

         switch (collider.name)
         {
            case GlobalTypes.CIRCLING_ENEMY_NAME:
               break;
            case GlobalTypes.VERTICAL_ENEMY_NAME:
               break;
            default:
               break;
         }
      }
   }

   //-------------------------------------------------------------------------------------------
   // Function (public): Update - called every frame
   //-------------------------------------------------------------------------------------------
	void Update()
   {
      switch (this.mode)
      {
         case OwnshipModeEnum.ALIVE:
            // only speed up if on the ground (and can run) or in the air and a flap has occurred
            if ((this.flightDynamics.OnGround() || Input.GetKey("space")) && Input.GetKey("up")) this.flightDynamics.SpeedUp();
            if (Input.GetKey("down"))      this.flightDynamics.SlowDown();
            if (Input.GetKeyDown("space"))
            {
               this.ostrichWings.script.FlapWings();
               this.flightDynamics.Flap(this.transform);
            }
            if (Input.GetKey("left"))      this.flightDynamics.TurnLeft(this.transform);
            if (Input.GetKey("right"))     this.flightDynamics.TurnRight(this.transform);

            this.flightDynamics.Move (this.transform);
            break;
         case OwnshipModeEnum.DEAD:
            this.time += Time.deltaTime;
            this.flightDynamics.DeathSpiral(this.transform);
            if (this.time >= TOTAL_DEAD_TIME)
            {
               this.mode = OwnshipModeEnum.ALIVE;
            }
            break;
         default:
            Debug.Log ("unhandled case in Ownship::Update");
            break;
      }

      this.ostrichLegsLeft .script.Animate(this.flightDynamics.OnGround(),
                                           false, // isKicking always set false for ownship
                                           this.flightDynamics.GetSpeed());
      this.ostrichLegsRight.script.Animate(this.flightDynamics.OnGround(),
                                           false, // isKicking always set false for ownship
                                           this.flightDynamics.GetSpeed());

      // tell the camera about the current ownship transform
      this.cameraDriver.script.SetParent(this.transform, this.mode);
   }

   public void SetEnemyList(System.Collections.ArrayList enemyList)
   {
      this.cameraDriver.script.SetEnemyList (enemyList);
   }

   public OwnshipModeEnum GetMode()
   {
      return this.mode;
   }

   public Transform GetTransform()
   {
      return this.transform;
   }

   public float GetVelocity()
   {
      return this.flightDynamics.GetSpeed();
   }

   // private variables
   private GlobalTypes.CameraDriverType cameraDriver;
   private GlobalTypes.OstrichType      ostrich;
   private GlobalTypes.OstrichWingsType ostrichWings;
   private GlobalTypes.OstrichLegsType  ostrichLegsLeft;
   private GlobalTypes.OstrichLegsType  ostrichLegsRight;
   private GlobalTypes.LanceType        lance;

   private OwnshipModeEnum mode = OwnshipModeEnum.ALIVE;

   private FlightDynamics flightDynamics;
   private float time = 0.0f;

   // constants
   private const float COLLIDER_SIZE = 20.0f;
   private const float TOTAL_DEAD_TIME = 3.0f;
   private const float OSTRICH_TRANSPARENCY = 0.2f;
}



