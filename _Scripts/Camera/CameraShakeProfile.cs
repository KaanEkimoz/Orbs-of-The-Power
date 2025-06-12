using UnityEngine;

namespace com.game.camera
{
    [CreateAssetMenu(menuName = "Game/Camera/Camera Shake Profile", fileName = "New Camera Shake Profile", order = int.MinValue)]
    public class CameraShakeProfile : ScriptableObject
    {
        [SerializeField, Min(0f)] private float m_duration;
        [SerializeField] private float m_amplitude = 1f;
        [SerializeField] private float m_frequency = 1f;

        public float Duration => m_duration;
        public float Amplitude => m_amplitude;
        public float Frequency => m_frequency;
    }
}
