using UnityEngine;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    public abstract class PlayerComponent {
        protected PlayerController _playerController;
        public virtual void InitializeComponent(PlayerController playerController) {
            _playerController = playerController;
        }

        public virtual void AwakeComponent() { }
        public virtual void StartComponent() { }

        public virtual void UpdateComponent() { }
        public virtual void FixedUpdateComponent() { }
        public virtual void LateUpdateComponent() { }

        public virtual void OnEnableComponent() { }
        public virtual void OnDisableComponent() { }

        public virtual void OnDestroyComponent() { }

        public virtual void OnDrawGizmosComponent() { }

    }
}