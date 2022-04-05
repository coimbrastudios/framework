using UnityEngine;

namespace Coimbra.Tests.UnityTests
{
    internal sealed class LogBehaviour : MonoBehaviour
    {
        public const string AwakeLog = nameof(Awake);

        public const string EnableLog = nameof(OnEnable);

        public const string StartLog = nameof(Start);

        private void Awake()
        {
            Debug.Log(AwakeLog);
        }

        private void OnEnable()
        {
            Debug.Log(EnableLog);
        }

        private void Start()
        {
            Debug.Log(StartLog);
        }
    }
}
