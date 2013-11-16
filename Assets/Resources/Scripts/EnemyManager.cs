using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: EnemyManager
//
// Description: This is the main object that handles all instances of the enemies
//-------------------------------------------------------------------------------------------------------------
public class EnemyManager : MonoBehaviour
{
	void Start()
   {
      this.name = "enemyManager";

      GlobalTypes.EnemyType enemy;

      // add the ground entity
      /*
      enemy.go = new GameObject();
      enemy.go.transform.localScale = 10.0f * Vector3.one;
      enemy.script = (EnemyGround)enemy.go.AddComponent(typeof(EnemyGround));
      enemy.script.Initialize(this.transform, ref enemy.go);
      this.enemyList.Add (enemy);
      */

      // add the circling enemy
      enemy.go = new GameObject();
      enemy.go.transform.localScale = 10.0f * Vector3.one;
      enemy.script = (EnemyCircling)enemy.go.AddComponent(typeof(EnemyCircling));
      enemy.script.Initialize(this.transform, ref enemy.go);
      enemy.go.transform.Translate(new Vector3(0.0f, 0.0f, 400.0f), Space.World);
      this.enemyList.Add (enemy);

      // add the circling enemy
      enemy.go = new GameObject();
      enemy.go.transform.localScale = 10.0f * Vector3.one;
      enemy.script = (EnemyCircling)enemy.go.AddComponent(typeof(EnemyCircling));
      enemy.script.Initialize(this.transform, ref enemy.go);
      enemy.go.transform.Translate(new Vector3(0.0f, 0.0f, -400.0f), Space.World);
      this.enemyList.Add (enemy);


      // add the vertical enemy
      enemy.go = new GameObject();
      enemy.go.transform.localScale = 10.0f * Vector3.one;
      enemy.script = (EnemyVertical)enemy.go.AddComponent(typeof(EnemyVertical));
      enemy.script.Initialize(this.transform, ref enemy.go);
      enemy.go.transform.Translate(new Vector3(-600.0f, 0.0f, 0.0f), Space.World);
      this.enemyList.Add (enemy);

      // add the vertical enemy
      enemy.go = new GameObject();
      enemy.go.transform.localScale = 10.0f * Vector3.one;
      enemy.script = (EnemyVertical)enemy.go.AddComponent(typeof(EnemyVertical));
      enemy.script.Initialize(this.transform, ref enemy.go);
      enemy.go.transform.Translate(new Vector3(600.0f, 0.0f, 0.0f), Space.World);
      this.enemyList.Add (enemy);
	}

   //------------------------------------------------------------------
   // Function (public): Update
   //------------------------------------------------------------------
	void Update()
   {
      // for every enemy in the list
      for (int i=0;i<this.enemyList.Count;++i)
      {
         // update enemies that are still alive
         GlobalTypes.EnemyType enemy = (GlobalTypes.EnemyType)this.enemyList[i];
         switch (enemy.script.GetMode())
         {
            case Enemy.EnemyModeEnum.ALIVE:
               enemy.script.SetOwnshipTransform(this.ownshipTransform, this.ownshipVelocity);
               //enemy.script.Update();
               break;
            case Enemy.EnemyModeEnum.JOUSTED:
               break;
            case Enemy.EnemyModeEnum.REMOVE_ME:
               // remove enemies that no longer exist
               Destroy(enemy.go);
               this.enemyList.RemoveAt(i);
               break;
            default:
               Debug.Log ("unhandled case in EnemyManager::Update()");
               break;
         }
      }
	}

   public void SetOwnshipTransform(Transform transform, float velocity)
   {
      this.ownshipTransform = transform;
      this.ownshipVelocity = velocity;
   }

   //------------------------------------------------------------------
   // Function (public): GetEnemyList - returns the list of active enemies
   //                                   does this really return a reference or a copy of all enemies?
   //------------------------------------------------------------------
   public System.Collections.ArrayList GetEnemyList()
   {
      return this.enemyList;
   }

   //------------------------------------------------------------------
   // Function (public): GetNumberOfEnemies - returns the number of active enemies
   //------------------------------------------------------------------
   public int GetNumberOfEnemies()
   {
      int numberOfEnemies = 0;

      for (int i=0;i<this.enemyList.Count;++i)
      {
         if (((GlobalTypes.EnemyType)this.enemyList[i]).script.GetMode() == Enemy.EnemyModeEnum.ALIVE)
         {
            ++numberOfEnemies;
         }
      }
      return numberOfEnemies;
   }

   //make list public for reference access (is this really true?)
   public System.Collections.ArrayList enemyList = new System.Collections.ArrayList();

   private Transform ownshipTransform;
   private float ownshipVelocity;
}
