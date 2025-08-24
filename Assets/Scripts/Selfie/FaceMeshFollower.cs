using UnityEngine;
using TMPro;
namespace WebARFoundation {
    public class FaceMeshFollower : MonoBehaviour {
        [SerializeField] TextMeshProUGUI spherePosition;   // Drag your FaceMesh object here in Inspector

        [SerializeField] FaceMesh faceMesh;   // Drag your FaceMesh object here in Inspector
        [SerializeField] Vector3 positionOffset = Vector3.zero;
        [SerializeField] Vector3 rotationOffset = Vector3.zero;
        [SerializeField] Vector3 scaleMultiplier = Vector3.one;

        void LateUpdate() {
            if (faceMesh == null) return;

            // Copy transform
            transform.position = faceMesh.transform.position + faceMesh.transform.rotation * positionOffset;
            //transform.rotation = faceMesh.transform.rotation * Quaternion.Euler(rotationOffset);
            //transform.localScale = Vector3.Scale(faceMesh.transform.localScale, scaleMultiplier);
        }
    }
}
