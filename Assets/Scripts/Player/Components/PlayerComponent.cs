using UnityEngine;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    public abstract class PlayerComponent {
        protected PlayerController _playerController;
        [HideInInspector] public bool enabled = true;
        public virtual void InitializeComponent(PlayerController playerController) {
            _playerController = playerController;
        }

        public virtual void AwakeComponent() { }
        public virtual void StartComponent() { }

        public virtual void UpdateComponent() {
            if (!enabled) return;
        }
        public virtual void FixedUpdateComponent() {
            if (!enabled) return;
        }
        public virtual void LateUpdateComponent() {
            if (!enabled) return;
        }

        public virtual void OnEnableComponent() { }
        public virtual void OnDisableComponent() { }

        public virtual void OnDestroyComponent() { }

        public virtual void OnDrawGizmosComponent() { }

    }
}