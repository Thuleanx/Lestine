using UnityEngine;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class AllSabaInteractables : Singleton<AllSabaInteractables> {
		public List<SabaInteractable> AsList = new List<SabaInteractable>();
	}

	public abstract class SabaInteractable : MonoBehaviour {
		void OnEnable() { AllSabaInteractables.instance.AsList.Add(this); }
		void OnDisable() {
			if (AllSabaInteractables.isInstanceAlive)
				AllSabaInteractables.instance.AsList.Remove(this);
		}
        public abstract void Interact(SabaEntity entity);

        public abstract string GetInteractionPrompt();
        public abstract Sprite GetInteractionSprite();
	}
}
