using UnityEngine;
using System.Collections;

namespace Datastructures {
    public class MonoBlackboard : MonoBehaviour, IBlackboard {
        Hashtable table = new Hashtable();
        Hashtable IBlackboard.Board { 
            get => table;
            set => table = value;
        }
    }
}
