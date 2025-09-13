namespace Saba {
	public class SabaLoot : SabaInteractable {
        public SabaItemDefinition itemDefinition;

        public override void Interact(SabaEntity entity) {
            foreach (SabaBuffData buffData in itemDefinition.buffsToApply)
                buffData.ApplyTo(entity);
            Destroy(gameObject);
        }
    }
}
