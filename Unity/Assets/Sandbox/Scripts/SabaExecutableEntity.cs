using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using PrettyPatterns;

namespace Saba {
    public class Deque<T> {
        int count;
        T[] data;
        int lt;
        int rt;

        public Deque(int count) {
            this.count = count;
            data = new T[count];
            lt = rt = 0;
        }

        public bool IsEmpty => lt==rt;

        public void Push(T element) {
            data[rt++] = element;
            if (rt == count) rt = 0;
            if (rt == lt) {
                Debug.LogError(
                    "Collision hit register exceeded capacity, silently dropping oldest entry"
                );
                lt++;
            }
        }

        public T Peek() {
            Assert.AreNotEqual(lt, rt, "Cannot peek into empty collection");
            return data[lt];
        }

        public void Pop() {
            Assert.AreNotEqual(lt, rt, "Cannot pop from an empty collection");
            lt++;
        }

        public IEnumerable<T> Enumerate() {
            for (int i = lt; i != rt; ) {
                yield return data[i];
                i++;
                if (i == count) i = 0;
            }
        }
    }

	public class SabaExecutableRuntimeGroup :
		Singleton<SabaExecutableRuntimeGroup> {

        const int MaxExecutableEnemies = 1000;

        [SerializeField] float executionTime;

        public struct Entry {
            public SabaEntity entity;
            public float deathTime;
        };

        [System.NonSerialized]
        public Deque<Entry> activeEntities = new Deque<Entry>(MaxExecutableEnemies);

        [SerializeField]
        float executableTime = 3;

        public void Register(SabaEntity entity) {
            activeEntities.Push(new Entry() {
                entity = entity,
                deathTime = Time.time
            });
        }

        public void Update() {
            const int MAX_DESPAWN_PER_UPDATE = 30;

            // So we don't despawn too many enemies at once
            for (int _ = 0; _ < MAX_DESPAWN_PER_UPDATE; _++) {
                if (activeEntities.IsEmpty) break;

                Entry entry = activeEntities.Peek();
                bool isEntryExpired = entry.deathTime + executableTime < Time.time;
                if (!isEntryExpired) break;
                activeEntities.Pop();

                if (entry.entity && entry.entity.isActiveAndEnabled)
                    Destroy(entry.entity.gameObject);
            }
        }
	}
}
