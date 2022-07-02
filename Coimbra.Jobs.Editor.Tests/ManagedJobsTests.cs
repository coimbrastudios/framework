using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Coimbra.Jobs.Editor.Tests
{
    internal sealed class ManagedJobsTests
    {
        private sealed class TestManagedJob : IManagedJob
        {
            public string Left { get; set; }

            public string Right { get; set; }

            public string Result { get; set; }

            public void Execute()
            {
                Result = Left + Right;
            }
        }

        [Test]
        public void ManagedJob_Run()
        {
            const string left = "left";
            const string right = "right";

            TestManagedJob task = new()
            {
                Left = left,
                Right = right,
            };

            task.Run();

            Assert.AreEqual(task.Result, left + right);
        }

        [Test]
        public void ManagedJob_Schedule()
        {
            const string left = "left";
            const string right = "right";

            TestManagedJob task = new()
            {
                Left = left,
                Right = right,
            };

            task.Schedule().Complete();

            Assert.AreEqual(task.Result, left + right);
        }

        [UnityTest]
        public IEnumerator ManagedJob_Run_Wait_Result()
        {
            const string left = "left";
            const string right = "right";

            TestManagedJob task = new()
            {
                Left = left,
                Right = right,
            };

            task.Run();

            yield return null;

            Assert.AreEqual(task.Result, left + right);
        }

        [UnityTest]
        public IEnumerator ManagedJob_Schedule_Wait_Complete()
        {
            const string left = "left";
            const string right = "right";

            TestManagedJob task = new()
            {
                Left = left,
                Right = right,
            };

            ManagedJobHandle handle = task.Schedule();

            yield return null;

            handle.Complete();
            Assert.AreEqual(task.Result, left + right);
        }

        [UnityTest]
        public IEnumerator ManagedJob_Schedule_Wait_Complete_Wait_Result()
        {
            const string left = "left";
            const string right = "right";

            TestManagedJob task = new()
            {
                Left = left,
                Right = right,
            };

            ManagedJobHandle handle = task.Schedule();

            yield return null;

            handle.Complete();

            yield return null;

            Assert.AreEqual(task.Result, left + right);
        }
    }
}
