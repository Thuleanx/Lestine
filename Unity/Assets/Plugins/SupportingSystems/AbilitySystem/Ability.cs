using System;

namespace AbilitySystem {
    public interface IAbility<T> {
        public void Activate(T param, Action callback = null);
        public void EndAbility();
    }
}
