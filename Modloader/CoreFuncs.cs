using UnityEngine;
using HarmonyLib;
using PTmod;

namespace PTmod
{
    [HarmonyPatch(typeof(PlayerMovement), "Look")]
    class LookPatch
    {
        public static bool Prefix()
        {
            if(Core.lockCamera)
            {
                return false;
            }

            return true;
        }
    }

    public class Core : MonoBehaviour
    {
        public static PlayerMovement player;
        public static bool lockCamera;

        public static void LockCamera()
        {
            lockCamera = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        public static void UnlockCamera()
        {
            lockCamera = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void Update()
        {
            if(player == null) player = FindObjectOfType<PlayerMovement>();
        }
    }
}