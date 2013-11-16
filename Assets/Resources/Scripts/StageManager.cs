using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: StageManager
//
// Description: This class is the main object that instantiates everything else
//-------------------------------------------------------------------------------------------------------------
public class StageManager : MonoBehaviour
{

   //-----------------------------------------------------------------
   // Function (public): Start
   //------------------------------------------------------------------
	void Start()
   {
      this.ownship.go = new GameObject();
      this.ownship.script = (Ownship) this.ownship.go.AddComponent(typeof(Ownship));
      this.ownship.script.Initialize(ref this.ownship.go);

      this.enemyManager.go = new GameObject();
      this.enemyManager.script = (EnemyManager) this.enemyManager.go.AddComponent(typeof(EnemyManager));

      this.guiInstructions.go = new GameObject();
      this.guiInstructions.script = (GUIInstructions) this.guiInstructions.go.AddComponent(typeof(GUIInstructions));
   }

   //-----------------------------------------------------------------
   // Function (public): Update
   //------------------------------------------------------------------
	void Update()
   {
      if (this.guiInstructions.go && this.guiInstructions.script.RemoveMe())
      {
         Destroy (this.guiInstructions.go);
      }
      else
      {
         this.guiInstructions.script.SetNumberOfEnemies(this.enemyManager.script.GetNumberOfEnemies());
         this.guiInstructions.script.SetOwnshipMode(this.ownship.script.GetMode());
      }

      // get the enemy list from the enemy manager and pass it to the ownship
      this.ownship.script.SetEnemyList(this.enemyManager.script.GetEnemyList());
      this.enemyManager.script.SetOwnshipTransform(this.ownship.script.GetTransform(),
                                                   this.ownship.script.GetVelocity());
   }

   // private variables
   private GlobalTypes.OwnshipType ownship;
   private GlobalTypes.EnemyManagerType enemyManager;
   private GlobalTypes.GUIInstructionsType guiInstructions;

}

