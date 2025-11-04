using UnityEngine;
using System.Collections.Generic;

using PrettyPatterns;

namespace eclipse.interactable {
	public class InteractableSystem : Singleton<InteractableSystem> {
		List<Interactable> interactables = new List<Interactable>();

		public void AddInteractable(Interactable interactable) => interactables.Add(interactable);
		public void RemoveInteractable(Interactable interactable) => interactables.Remove(interactable);


        public Interactable PoolClosest(Vector2 location)
            => Statics.GetClosest2D<Interactable>(interactables, location);
	}
}
