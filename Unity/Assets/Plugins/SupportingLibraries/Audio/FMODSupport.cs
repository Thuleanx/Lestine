using FMODUnity;
using FMOD.Studio;
using UnityEngine;

namespace Audio {
    public static class FMODExtensions {

        public static bool CheckFMODResult(FMOD.RESULT result) {
            bool checkOK = result == FMOD.RESULT.OK;
            if (!checkOK) { Debug.LogError(result); }
            return checkOK;
        }
    }
}
