using Coimbra.Jobs;
using System.Collections;
using UnityEngine;

namespace Coimbra.Samples.ConcatStrings
{
    public sealed class ConcatStringsArraysJobSample : MonoBehaviour
    {
        private sealed class ConcatStringsArraysJob : IManagedJobParallelFor
        {
            public string[] Left { get; set; }

            public string[] Right { get; set; }

            public string[] Result { get; set; }

            public void Execute(int index)
            {
                Result[index] = $"{Left[index]}{Right[index]}";
            }
        }

        [SerializeField]
        private string[] _left =
        {
            "First",
            "Second",
            "Third",
        };

        [SerializeField]
        private string[] _right =
        {
            "Example 1",
            "Example 2",
            "Example 3",
        };

        [SerializeField]
        private int _innerLoopBatchCount = 1;

        private IEnumerator Start()
        {
            int length = Mathf.Min(_left.Length, _right.Length);

            ConcatStringsArraysJob task = new()
            {
                Left = _left,
                Right = _right,
                Result = new string[length],
            };

            ManagedJobHandle handle = task.Schedule(length, _innerLoopBatchCount);

            yield return null;

            handle.Complete();

            foreach (string result in task.Result)
            {
                print(result);
            }
        }
    }
}
