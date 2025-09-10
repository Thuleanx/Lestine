using UnityEngine;

namespace Saba {
	public class SabaLootContainer : MonoBehaviour {
		[SerializeField]
		float cost;

		enum State { Unopened, Opened }
		;
		State currentState;
		Interaction interaction;

		void Awake() {
			interaction = GetComponent<Interaction>();
			if (!interaction) interaction = gameObject.AddComponent<Interaction>();
		}

		void OnEnable() {
			interaction.enabled = false;
			currentState = State.Unopened;
		}

		void Open() {
			currentState = State.Opened;
			interaction.enabled = false;
		}

		public class Interaction : SabaInteractable {
			SabaLootContainer lootContainer;

			void Awake() => lootContainer = GetComponent<SabaLootContainer>();

			public override void Interact(SabaEntity entity) {
				if (!SabaGameState.instance) return;

				bool isAffordable = lootContainer.cost < SabaGameState.instance.Currency;
				if (!isAffordable) return;

				lootContainer.Open();
			}
		}
	}

}
