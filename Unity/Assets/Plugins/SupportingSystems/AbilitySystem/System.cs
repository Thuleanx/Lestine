using UnityEngine;
using System;

namespace AbilitySystem {
    public enum Priority {
        LOW = 0,
        MED = 1,
        HIGH = 2,
        CRITICAL = 3
    }

    public class System : MonoBehaviour {
        bool currentlyRunningAbility = false;
        Priority priorityOfCurrentAbility;
        Action endCurrentAbility = null;
        public bool DEBUG = false;

        public bool IsBlocked => IsCurrentPriorityAtLeast(Priority.MED);
        public bool IsCurrentPriorityAtLeast(Priority prio) 
            => currentlyRunningAbility && priorityOfCurrentAbility >= prio;

        public void ActivateAbility<T>(Priority priority, IAbility<T> ability, T param) {
            bool isRunningHigherPrioTask = IsCurrentPriorityAtLeast(priority);
            if (isRunningHigherPrioTask)
                return;

            // This has to be a while loop (kinda dangerous) because endCurrentAbility
            // can spawn new ones
            while (currentlyRunningAbility && endCurrentAbility != null) {
                // this ensures that endCurrentAbility is not run twice if the current ability end
                // attempts to activate another ability
                currentlyRunningAbility = false;
                Action endCurrentAbilityOnce = endCurrentAbility;
                endCurrentAbility = null;
                endCurrentAbilityOnce();
            }

            if (DEBUG) Debug.Log("Activate ability: " + ability);

            currentlyRunningAbility = true;
            priorityOfCurrentAbility = priority;
            endCurrentAbility = ability.EndAbility;

            ability.Activate(param, OnAbilityEnd);
        }

        void OnAbilityEnd() {
            currentlyRunningAbility = false;
            priorityOfCurrentAbility = Priority.LOW;
            endCurrentAbility = null;
        }
    }
}
