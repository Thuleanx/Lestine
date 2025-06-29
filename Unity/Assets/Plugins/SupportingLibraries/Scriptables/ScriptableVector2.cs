using UnityEngine;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/Vector2", order = 1)]
    public class ScriptableVector2 : Scriptable<Vector2> {}
}
