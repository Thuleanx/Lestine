using UnityEngine;
using System.Collections.Generic;

namespace Saba {
	[RequireComponent(typeof(SabaEntity))]
	public class SabaEffectDispatch : MonoBehaviour {
		public struct DispatchData<T> {
			// Buffs granted when this event is triggered
			public List<SabaBuffData> buffs;
			public List<SabaEffect> effects;

            public void Init() {
                buffs = new List<SabaBuffData>();
                effects = new List<SabaEffect>();
            }

			public void AddBuff(IEnumerable<SabaBuffData> buffs) => this.buffs.AddRange(buffs);
            public void AddEffect(IEnumerable<SabaEffect> effects) => this.effects.AddRange(effects);
		};

		public DispatchData<SabaGameplayEvents.Kill> onKillDispatcher;

		List<SabaGameplayEvents.Kill> killEvents = new List<SabaGameplayEvents.Kill>();

		SabaEntity entity;
		void Awake() { 
            entity = GetComponent<SabaEntity>(); 
            onKillDispatcher.Init();
        }

		public void LazyDispatch(IEnumerable<SabaGameplayEvents.Kill> @event) { killEvents.AddRange(@event); }

        void Update() {
            TickEvents();
        }

        void TickEvents() {
            foreach (SabaGameplayEvents.Kill killEvent in killEvents) {
                foreach (SabaBuffData buff in onKillDispatcher.buffs)
                    buff.ApplyTo(entity);
            }
            killEvents.Clear();
        }
	}
}
