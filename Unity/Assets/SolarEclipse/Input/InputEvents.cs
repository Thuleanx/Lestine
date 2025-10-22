using ADammy;

namespace eclipse.input {
    public struct AttackAction : IEvent {
        public bool active;
    }

    public struct ExecutionAction : IEvent {}
    public struct InteractionAction : IEvent {}
}
