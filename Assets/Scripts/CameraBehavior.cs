using UnityEngine;
using UnityEngine.Serialization;

namespace NoSuchCompany.Games.SuperMario
{
    public class CameraBehavior : MonoBehaviour
    {
        private Vector3 _velocity;

        [FormerlySerializedAs("PlayerGameObject")]
        public GameObject playerGameObject;

        [FormerlySerializedAs("TimeOffset")]
        public float timeOffset;

        [FormerlySerializedAs("PositionOffset")]
        public Vector3 positionOffset;

        public void Update()
        {
            Vector3 result = Vector3.SmoothDamp
            (
                transform.position, 
                playerGameObject.transform.position + positionOffset, 
                ref _velocity, 
                timeOffset
            );

            transform.position = new Vector3(result.x, transform.position.y, transform.position.z);
        }
    }
}
