using System;
using System.Collections.Generic;

using UnityEngine;

namespace PrettyPatterns {
	public interface RemovableSpanList {
		public int GetCurrentNum();
		public void SetCurrentNum(int num);
		public void Set(int i, int j);
		public void ResetSingle(int i);

		public int Allocate(int number) {
			int current = GetCurrentNum();
			SetCurrentNum(current + number);
			return current;
		}

		public static void Remove(RemovableSpanList table, ReadOnlySpan<int> indices, Action<int, int> onMove) {
			// we need to update stat table references of certain entities when
			// we kill some and remap the indices
			Dictionary<int, int> remapping = new Dictionary<int, int>();

			foreach (int index in indices) {
				int lastIndex = table.GetCurrentNum() - 1;

				int indexToRemove = index;

				bool previouslyMoved = remapping.ContainsKey(index);
				if (previouslyMoved) {
					indexToRemove = remapping[index];
					remapping.Remove(index);
				}

				if (lastIndex != indexToRemove) {
					remapping[lastIndex] = indexToRemove;
					if (onMove != null) onMove(indexToRemove, lastIndex);
					table.Set(indexToRemove, lastIndex);
                    table.ResetSingle(lastIndex);
				} else {
                    table.ResetSingle(lastIndex);
                }

				table.SetCurrentNum(lastIndex);
			}
        }
	}
}
