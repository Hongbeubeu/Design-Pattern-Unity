using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(1, 1, "1 should be equal to 1");
        Assert.That(() => throw new Exception(), Throws.Exception);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        if (!Application.isPlaying)
            yield return null;
        else
        {
            var current = Time.time;
            yield return new WaitForSeconds(1);
            Assert.AreEqual(current + 1, Time.time, 0.1f);
        }
    }
}