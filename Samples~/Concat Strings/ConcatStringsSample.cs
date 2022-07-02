using Coimbra.Jobs;
using System.Collections;
using UnityEngine;

namespace Coimbra.Samples.ConcatStrings
{
    public sealed class ConcatStringsSample : MonoBehaviour
    {
        private sealed class ConcatStringsJob : IManagedJob
        {
            public string Left { get; set; }

            public string Right { get; set; }

            public string Result { get; set; }

            public void Execute()
            {
                Result = $"{Left}{Right}";
            }
        }

        [SerializeField]
        private string _left = "Task";

        [SerializeField]
        private string _right = "Sample";

        private IEnumerator Start()
        {
            ConcatStringsJob task = new()
            {
                Left = _left,
                Right = _right,
            };

            ManagedJobHandle handle = task.Schedule();

            yield return null;

            handle.Complete();

            print(task.Result);
        }
    }
}
