
using UnityEngine;
using System.Collections;

//-------------------------------------------------------------------------------------------------------------
// Class: GUIInstructions
//
// Description: This class is used to present text on the screen
//-------------------------------------------------------------------------------------------------------------
public class GUIInstructions : MonoBehaviour
{

   enum InstructionsModeEnum
   {
      INSTRUCTIONS      = 0,
      NUMBER_OF_ENEMIES = 1,
      OWNSHIP_DEAD      = 2,
      REMOVE_ME         = 3
   }
   //------------------------------------------------------------------
   // Start: called at initialization
   //------------------------------------------------------------------
   void Start()
   {
      this.name = "GUIInstructions";

      this.guiStyle = new GUIStyle();
      this.guiStyle.font = (Font)Resources.Load("vinque");
      this.guiStyle.fontSize = BASE_FONT_SIZE;
      this.guiStyle.alignment = TextAnchor.MiddleCenter;
      this.guiStyle.normal.textColor = new Color(255.0f/255.0f, 255.0f/255.0f, 255.0f/255.0f, 0.7f);

   }

   //------------------------------------------------------------------
   // Update: called once a frame
   //------------------------------------------------------------------
   void Update()
   {
      this.time += Time.deltaTime;
      switch (this.mode)
      {
         case InstructionsModeEnum.INSTRUCTIONS:
            if (this.time > 10.0f)
            {
               this.mode = InstructionsModeEnum.NUMBER_OF_ENEMIES;
               this.time = 0.0f;
            }
            break;
         case InstructionsModeEnum.NUMBER_OF_ENEMIES:
            break;
         case InstructionsModeEnum.OWNSHIP_DEAD:
            break;
      }
   }

   public void SetNumberOfEnemies(int numberOfEnemies)
   {
      this.numberOfEnemies = numberOfEnemies;
   }


   //------------------------------------------------------------------
   // OnGUI: called from executive - renders the GUI labels
   //------------------------------------------------------------------
   void OnGUI()
   {
      switch (this.mode)
      {
         case InstructionsModeEnum.INSTRUCTIONS:
            GUI.Label(new Rect (Screen.width * 0.22f, Screen.height * 0.35f, 500, 500),
               "Spacebar to flap\nUp/down to move forward/backward\nLeft/Right for left/right\nPress F to find target\nBe above target to win",
               this.guiStyle);
            break;
         case InstructionsModeEnum.NUMBER_OF_ENEMIES:
            if (this.numberOfEnemies > 1)
            {
               GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.5f, 480, 500),
                  "Thou hast " + this.numberOfEnemies + " enemies to defeat",
                  this.guiStyle);
            }
            else if (this.numberOfEnemies == 1)
            {
               GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.5f, 480, 500),
                  "Thou hast only 1 enemy to defeat",
                  this.guiStyle);
            }
            else if (this.numberOfEnemies == 0)
            {
               GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.5f, 480, 500),
                  "Thou hast defeated all enemies!",
                  this.guiStyle);
            }
            break;
         case InstructionsModeEnum.OWNSHIP_DEAD:
            GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.5f, 480, 500),
               "Thou hast been jousted!",
               this.guiStyle);
            break;
         default:
            Debug.Log("unhandled case in GUIInstructions::OnGUI");
            break;;
      }
   }

   public void SetOwnshipMode(Ownship.OwnshipModeEnum ownshipMode)
   {
      switch (ownshipMode)
      {
         case Ownship.OwnshipModeEnum.ALIVE:
            if (this.mode != InstructionsModeEnum.INSTRUCTIONS)
            {
               this.mode = InstructionsModeEnum.NUMBER_OF_ENEMIES;
            }
            break;
         case Ownship.OwnshipModeEnum.DEAD:
            this.mode = InstructionsModeEnum.OWNSHIP_DEAD;
            break;
         default:
            Debug.Log ("unhandled case in GUIInstructions::SetOwnshipMode");
            break;

      }
   }

   public bool RemoveMe()
   {
      return this.mode == InstructionsModeEnum.REMOVE_ME;
   }


   //------------------------------------------------------------------
   // private declarations
   //------------------------------------------------------------------
   private const int BASE_FONT_SIZE = 30;

   private GUIStyle guiStyle;

   private float time = 0.0f;
   private int numberOfEnemies;
   private InstructionsModeEnum mode = InstructionsModeEnum.INSTRUCTIONS;
}



