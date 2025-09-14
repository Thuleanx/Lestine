using UnityEngine;
using UnityEngine.Assertions;

using MathUtils;

namespace Saba {
	[CreateAssetMenu(menuName = "Saba/DataTables/Items")]
	public class SabaItemDataTable : ScriptableObject {
        [System.Serializable]
		public struct Data {
			public SabaItemDefinition item;
			public int weight;
		}

		public Data[] items;

		int totalWeight;
		int[] cumulativeWeights;

        void Awake() => Precompute();
		void OnValidate() => Precompute();

        void Precompute() {
			cumulativeWeights = new int[items.Length];
			for (int i = 0; i < items.Length; i++)
				cumulativeWeights[i] = (i > 0 ? cumulativeWeights[i - 1] : 0) + items[i].weight;
			totalWeight = items.Length > 0 ? cumulativeWeights[items.Length - 1] : 0;
        }

		public SabaItemDefinition Pool() {
			if (totalWeight <= 0) return null;

			int selectedWeight = Mathx.RandomRange(0, totalWeight);

			int lt = 0, rt = items.Length;
			while (lt < rt) {
				int mid = (lt + rt) / 2;

				if (cumulativeWeights[mid] < selectedWeight) lt = mid + 1;
				else rt = mid - 1;
			}

			Assert.IsTrue(lt < items.Length);

			return items[lt].item;
		}
	}
}
