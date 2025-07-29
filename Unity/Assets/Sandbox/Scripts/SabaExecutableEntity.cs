using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
	public class SabaExecutableRuntimeGroup :
		Singleton<SabaExecutableRuntimeGroup> {
		public List<SabaEntity> activeEntities =
			new List<SabaEntity>();
	}
}
