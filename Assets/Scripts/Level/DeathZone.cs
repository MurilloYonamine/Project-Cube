using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    /// <summary>
    /// Zona de morte que faz o player voltar ao último rewind quando cair.
    /// Adicione este script a um GameObject com Collider (IsTrigger = true).
    /// </summary>
    public class DeathZone : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            PLAYER.PlayerController player = other.GetComponent<PLAYER.PlayerController>();
            
            if (player != null) {
                PlayerDebugManager.Instance?.AddLine("Player caiu! Fazendo rewind...", "DeathZone");
                
                // Aciona o rewind automaticamente
                if (player.PlayerRewind != null) {
                    player.StartCoroutine(AutoRewind(player));
                }
            }
        }
        
        private System.Collections.IEnumerator AutoRewind(PLAYER.PlayerController player) {
            // Pequeno delay para garantir que o player passou pelo trigger
            yield return new WaitForFixedUpdate();
            
            // Força o rewind chamando o método privado através de reflection
            var rewindMethod = typeof(PLAYER.COMPONENTS.REWIND.PlayerRewind)
                .GetMethod("TriggerRewind", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (rewindMethod != null) {
                rewindMethod.Invoke(player.PlayerRewind, null);
            }
        }
    }
}
