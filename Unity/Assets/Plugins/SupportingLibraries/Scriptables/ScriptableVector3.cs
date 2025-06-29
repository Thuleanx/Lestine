using UnityEngine;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/Vector3", order = 1)]
    public class ScriptableVector3 : Scriptable<Vector3> {}
}
