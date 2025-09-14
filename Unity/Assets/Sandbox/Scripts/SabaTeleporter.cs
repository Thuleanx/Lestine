using UnityEngine;

namespace Saba {
	[RequireComponent(typeof(SabaDirector))]
	public class SabaTeleporter : MonoBehaviour {
		public enum State { Active, Charging, PendingTeleport }
		;

		public State CurrentState { get; private set; } = State.Active;

		[SerializeField, Min(0.01f)] float chargingDuration = 30.0f;
		[SerializeField] float chargingRange = 1.0f;

		SabaDirector director;
		ActivationInteractor interactor;
        PortalInteractor portal;
		float chargingProgress = 0;

		void Awake() {
			if (!GetComponent<ActivationInteractor>()) gameObject.AddComponent<ActivationInteractor>();
			if (!GetComponent<PortalInteractor>()) gameObject.AddComponent<PortalInteractor>();
			interactor = GetComponent<ActivationInteractor>();
            portal = GetComponent<PortalInteractor>();
			director = GetComponent<SabaDirector>();
		}

		void Start() {
			CurrentState = State.Active;
			interactor.enabled = true;
            portal.enabled = false;
			chargingProgress = 0;
		}

		void OnDestroy() { 
            interactor.enabled = false;
            portal.enabled = false;
        }

		void StartCharging() {
			CurrentState = State.Charging;
			director.enabled = true;
            interactor.enabled = false;
            enabled = true;
		}

		void Update() {
			if (CurrentState != State.Charging) {
                enabled = false;
                return;
            }
			SabaPlayer player = SabaPlayer.instance;
			if (!player) return;

			bool isPlayerClose =
				((Vector2)(player.transform.position - transform.position)).sqrMagnitude <= chargingRange;
			if (isPlayerClose) chargingProgress += Time.deltaTime / chargingDuration;

            bool isChargingDone = chargingProgress >= 1;
            if (chargingProgress >= 1) {
                director.enabled = false;
                enabled = false;
                CurrentState = State.PendingTeleport;
                portal.enabled = true;
            }
		}

		public class ActivationInteractor : SabaInteractable {
			SabaTeleporter teleporter;

			void Awake() { teleporter = GetComponent<SabaTeleporter>(); }

			public override void Interact(SabaEntity entity) => teleporter.StartCharging();

            public override string GetInteractionPrompt() => "Activate teleporter";
            public override Sprite GetInteractionSprite() => null;
		}

		public class PortalInteractor : SabaInteractable {
			SabaTeleporter teleporter;

			void Awake() { teleporter = GetComponent<SabaTeleporter>(); }

			public override void Interact(SabaEntity entity) {
                Debug.Log("Should be teleporting to the end");
            }

            public override string GetInteractionPrompt() => "Next level";
            public override Sprite GetInteractionSprite() => null;
		}
	}

}
