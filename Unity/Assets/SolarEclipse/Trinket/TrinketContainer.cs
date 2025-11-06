using UnityEngine;

using eclipse.items;
using PrettyPatterns;

namespace eclipse.trinket {
    [RequireComponent(typeof(Entity))]
	public class TrinketContainer : MonoBehaviour {
        const int MAX_TRINKETS = 100;

        public class Data : RemovableSpanList {
            public int num = 0;
            public Trinket[] trinkets;
            public int[] count;

            public static Data Create() {
                return new Data {
                    num = 0,
                    trinkets = new Trinket[MAX_TRINKETS],
                    count = new int[MAX_TRINKETS]
                };
            }

            public int GetCurrentNum() => num;
            public void ResetSingle(int i) {
                trinkets[i] = default;
                count[i] = 0;
            }

            public void Set(int i, int j) {
                trinkets[i] = trinkets[j];
                count[i] = count[j];
            }

            public void SetCurrentNum(int num) {
                this.num = num;
            }
        };

        Data data;
        Entity entity;

        void Awake() {
            data = Data.Create();
            entity = GetComponent<Entity>();
        }

        public void Acquire(Trinket trinket) {
            Debug.Log("On acquire");
            trinket.OnAdd(entity);
            EntityStatics.RecomputeStats(entity);

            for (int i = 0; i < data.num; i++) {
                bool isSameItem = data.trinkets[i] == trinket;
                if (isSameItem) {
                    data.count[i]++;
                    return;
                }
            }

            data.trinkets[data.num] = trinket;
            data.count[data.num++] = 1;
        }
	}
}
