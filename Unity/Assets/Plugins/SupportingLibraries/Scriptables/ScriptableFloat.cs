using UnityEngine;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/Float", order = 1)]
    public class ScriptableFloat : Scriptable<float> {}
}
