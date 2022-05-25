using UnityEngine;
using HarmonyLib;
using PTmod;

namespace MapEditor
{
    [HarmonyPatch(typeof(PlayerMovement), "Movement")]
    class MovementPatch
    {
        public static bool Prefix()
        {
            if(FreeCam.enableFreeCam)
            {
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(PlayerMovement), "MyInput")]
    class InputPatch
    {
        public static bool Prefix()
        {
            if(FreeCam.enableFreeCam)
            {
                return false;
            }
            return true;
        }
    }

    [PTmodLoad]
    class FreeCam : MonoBehaviour
    {
        public static float CamSpeed = 20f;
        public static bool enableFreeCam = false;

        public static void Enable()
        {
            enableFreeCam = true;
        }

        public static void Disable()
        {
            enableFreeCam = false;
            Core.player.playerCam.transform.position = Core.player.transform.position;
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q)) 
            {
                if (enableFreeCam) Disable(); 
                else Enable();
            }
            if(!enableFreeCam) return;


            if (Input.GetKey(KeyCode.W))
            {
                Core.player.playerCam.transform.position += Core.player.orientation.transform.forward * Time.deltaTime * CamSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                Core.player.playerCam.transform.position -=  Core.player.orientation.transform.right * Time.deltaTime * CamSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                Core.player.playerCam.transform.position -= Core.player.orientation.transform.forward * Time.deltaTime * CamSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                Core.player.playerCam.transform.position += Core.player.orientation.transform.right * Time.deltaTime * CamSpeed;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Core.player.playerCam.transform.position += Core.player.orientation.transform.up * Time.deltaTime * CamSpeed;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Core.player.playerCam.transform.position -= Core.player.orientation.transform.up * Time.deltaTime * CamSpeed;
            }
        }
    }
}