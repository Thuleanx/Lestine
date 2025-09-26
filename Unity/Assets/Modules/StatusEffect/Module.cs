using System.Collections.Generic;
using PrettyPatterns;

namespace StatusEffects {
	public class Module : Singleton<Module> {
		public class Owner {
            List<int> staticEffects;
			List<int> tickableEffects;
		}

		public class ActiveEffects {
			StatusEffect[] effects;
			float[] timeExpire;
            float[] intensity;
			int[] owners;
		}

		ActiveEffects tickableEffects;
	}
}
