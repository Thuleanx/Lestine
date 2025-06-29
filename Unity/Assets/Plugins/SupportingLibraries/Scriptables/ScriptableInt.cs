using UnityEngine;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/Int", order = 1)]
    public class ScriptableInt : Scriptable<int> {}
}
