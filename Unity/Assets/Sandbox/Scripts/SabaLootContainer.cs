using UnityEngine;

namespace Saba {
	public class SabaLootContainer : MonoBehaviour {
		[SerializeField]
		float cost;
        [SerializeField]
        SabaLoot lootPrefab;
        [SerializeField]
        SabaItemDefinition item;

		enum State { Unopened, Opened }
		State currentState;
		Interaction interaction;

		void Awake() {
			interaction = GetComponent<Interaction>();
			if (!interaction) interaction = gameObject.AddComponent<Interaction>();
		}

		void OnEnable() {
			currentState = State.Unopened;
			interaction.enabled = true;
		}

		void Open() {
			currentState = State.Opened;
			interaction.enabled = false;
            SabaLoot loot = Instantiate(lootPrefab, transform.position, transform.rotation);
            loot.itemDefinition = item;
            Destroy(gameObject);
		}

		public class Interaction : SabaInteractable {
			SabaLootContainer lootContainer;

			void Awake() => lootContainer = GetComponent<SabaLootContainer>();

			public override void Interact(SabaEntity entity) {
				if (!SabaGameState.instance) return;

				bool isAffordable = lootContainer.cost <= SabaGameState.instance.Currency;
				if (!isAffordable) return;

				lootContainer.Open();
                SabaGameState.instance.Currency -= lootContainer.cost;
			}

            public override string GetInteractionPrompt() => "Buy chest";
            public override Sprite GetInteractionSprite() => null;
		}
	}

}
