using UnityEngine;
using System.Collections.Generic;

namespace Scriptables {
    public class ScriptableGlobals : MonoBehaviour {

        [System.Serializable]
        public struct ScriptableDefault<T> {
            [SerializeField] Scriptable<T> scriptable;
            [SerializeField] T defaultValue;

            public void Assign() => scriptable.Value = defaultValue;
        }

        [SerializeField] List<ScriptableDefault<float>> scriptableFloats;
        [SerializeField] List<ScriptableDefault<int>> scriptableInts;
        [SerializeField] List<ScriptableDefault<Vector2>> scriptableVector2;
        [SerializeField] List<ScriptableDefault<Vector3>> scriptableVector3;

        void Awake() {
            scriptableInts.ForEach( (x) => x.Assign() );
            scriptableFloats.ForEach( (x) => x.Assign());
            scriptableVector2.ForEach( (x) => x.Assign());
            scriptableVector3.ForEach( (x) => x.Assign());
        }
    }
}
