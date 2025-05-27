using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ITeleportable>(out var teleportable))
        {
            // if (other.TryGetComponent(out MonoBehaviour mover))
            // {
            //     //Agent等が持つForceStopMovementを呼び出す
            //     mover.Invoke("ForceStopMovement", 0f);
            // }
            
            teleportable.TeleportTo(destinationPoint.position);
            teleportable.ForceStopMovement();
        }
    }
    
    
    /// <summary>
    /// テレポート先をインスペクターに表示
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            Gizmos.DrawSphere(destinationPoint.position, 0.2f);
        }
    }
}