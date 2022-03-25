using UnityEngine;

namespace Coimbra.Tests.UnityTests
{
    internal sealed class LogBehaviour : MonoBehaviour
    {
        public const string AwakeLog = nameof(Awake);

        public const string EnableLog = nameof(EnableLog);

        private void Awake()
        {
            Debug.Log(AwakeLog);
        }

        private void OnEnable()
        {
            Debug.Log(EnableLog);
        }
    }
}
