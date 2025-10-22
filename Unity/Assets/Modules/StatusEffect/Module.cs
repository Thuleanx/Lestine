using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using PrettyPatterns;

namespace StatusEffects {
	public class Module<T> : Singleton<Module<T>> {
		const int maxEffects = 3000;
		const int maxOwners = 200;

		public class Owner {
			public T ownerRef;
			public List<int> staticEffects;
			public List<int> tickableEffects;
		}

		public class ActiveEffects {
			public int currentNum;
			public StatusEffect<T>[] effects;
			public float[] timeExpire;
			public float[] intensity;
			public int[] owners;
			public bool[] toRemove;

			public ActiveEffects(int capacity) {
				currentNum = 0;
				effects = new StatusEffect<T>[capacity];
				timeExpire = new float[capacity];
				intensity = new float[capacity];
				owners = new int[capacity];
				toRemove = new bool[capacity];
			}
		}

		bool areOwnersDirty;
		int ownersLeft;
		int[] requestableOwners;
		Owner[] owners;

		ActiveEffects staticEffects;
		ActiveEffects tickableEffects;

		public override void Awake() {
			base.Awake();
			owners = new Owner[maxOwners];
			for (int i = 0; i < owners.Length; i++) owners[i] = new Owner();

			int effectsLength = maxEffects;
			staticEffects = new ActiveEffects(effectsLength);
			tickableEffects = new ActiveEffects(effectsLength);

			ownersLeft = owners.Length;
			for (int i = 0; i < ownersLeft; i++) requestableOwners[i] = i;

			areOwnersDirty = false;
		}

		public int RequestOwner(T ownerRef) {
			Assert.IsTrue(ownersLeft > 0);
			ownersLeft--;
			owners[requestableOwners[ownersLeft]].ownerRef = ownerRef;
			return requestableOwners[ownersLeft];
		}

		public void ReturnOwner(int owner) {
			Owner ownerData = owners[owner];
			foreach (int staticEffect in ownerData.staticEffects) {
				staticEffects.effects[staticEffect].OnRemoved(ownerData.ownerRef, staticEffects.intensity[staticEffect]);
				staticEffects.toRemove[staticEffect] = true;
			}

			foreach (int tickableEffect in ownerData.tickableEffects) {
				tickableEffects.effects[tickableEffect].OnRemoved(ownerData.ownerRef, tickableEffects.intensity[tickableEffect]);
				tickableEffects.toRemove[tickableEffect] = true;
			}

			ownerData.ownerRef = default(T);
			ownerData.staticEffects.Clear();
			ownerData.tickableEffects.Clear();
			requestableOwners[ownersLeft++] = owner;
		}

		public int AddStaticEffect(int owner, StatusEffect<T> effect, float intensity, float duration) =>
			AddEffectToContainer(staticEffects, owner, effect, intensity, duration);

		public int AddTickableEffect(int owner, StatusEffect<T> effect, float intensity, float duration) =>
			AddEffectToContainer(tickableEffects, owner, effect, intensity, duration);

		int AddEffectToContainer(
			ActiveEffects effectContainer, int owner, StatusEffect<T> effect, float intensity, float duration
		) {
			int i = effectContainer.currentNum++;
			effectContainer.owners[i] = owner;
			effectContainer.effects[i] = effect;
			effectContainer.intensity[i] = intensity;
			effectContainer.timeExpire[i] = duration + Time.time;
			effectContainer.toRemove[i] = false;

            effect.OnGranted(owners[owner].ownerRef, intensity);

			if (effectContainer == staticEffects) owners[owner].staticEffects.Add(i);
			else owners[owner].tickableEffects.Add(i);

			return i;
		}

		public void Tick() {
			{
				bool isAnyExpired = MarkExpired(staticEffects);
				if (isAnyExpired) MakeCompact(staticEffects);
                areOwnersDirty |= isAnyExpired;
			}
			{
				bool isAnyExpired = MarkExpired(tickableEffects);
				if (isAnyExpired) MakeCompact(tickableEffects);
                areOwnersDirty |= isAnyExpired;
			}

            if (areOwnersDirty) {
                // Clear all owners, linearly
                for (int i = 0; i < owners.Length; i++) {
                    owners[i].tickableEffects.Clear();
                    owners[i].staticEffects.Clear();
                }

                for (int i = 0; i < staticEffects.currentNum; i++)
                    owners[staticEffects.owners[i]].staticEffects.Add(i);
                for (int i = 0; i < tickableEffects.currentNum; i++)
                    owners[tickableEffects.owners[i]].tickableEffects.Add(i);

                areOwnersDirty = false;
            }

			// we tick after removals, because tickable effects might need access to up to date info (to, say, apply
			// additional effects) and also we won't have to branch
			for (int i = 0; i < tickableEffects.currentNum; i++)
				tickableEffects.effects[i].Tick(owners[tickableEffects.owners[i]].ownerRef);
		}

		bool MarkExpired(ActiveEffects effectContainer) {
			bool isAnyExpired = false;
			for (int i = 0; i < effectContainer.currentNum; i++) {
				effectContainer.toRemove[i] |= effectContainer.timeExpire[i] < Time.time;
				isAnyExpired |= effectContainer.toRemove[i];
			}
			return isAnyExpired;
		}

		void MakeCompact(ActiveEffects effectContainer) {
			int j = 0;
			for (int i = 0; i < effectContainer.currentNum; i++) {
				bool shouldSlideLeft = !effectContainer.toRemove[i];

				if (shouldSlideLeft) {
					effectContainer.owners[j] = effectContainer.owners[i];
					effectContainer.timeExpire[j] = effectContainer.timeExpire[i];
					effectContainer.intensity[j] = effectContainer.intensity[i];
					effectContainer.effects[j] = effectContainer.effects[i];
					effectContainer.toRemove[j] = false;
					j++;
				}
			}
			effectContainer.currentNum = j;
		}
	}
}
