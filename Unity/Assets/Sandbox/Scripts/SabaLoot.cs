using UnityEngine;

namespace Saba {
	public class SabaLoot : SabaInteractable {
		public SabaItemDefinition itemDefinition;

		public override void Interact(SabaEntity entity) {
			foreach (SabaBuffData buffData in itemDefinition.buffsToApply) buffData.ApplyTo(entity);
			entity.GetComponent<SabaEffectDispatch>()?.onKillDispatcher.AddBuff(itemDefinition.buffsOnKill);
			Destroy(gameObject);
		}

		public override string GetInteractionPrompt() => itemDefinition.displayName;
		public override Sprite GetInteractionSprite() => itemDefinition.icon;
	}
}
