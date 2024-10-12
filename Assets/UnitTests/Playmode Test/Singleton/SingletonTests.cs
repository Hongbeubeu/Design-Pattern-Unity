#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class SingletonTests
{
    // A simple MonoBehaviour class for testing Singleton
    private class TestSingleton : Singleton<TestSingleton>
    {
    }

    [SetUp]
    public void SetUp()
    {
        if (TestSingleton.Instance != null)
        {
            Object.DestroyImmediate(TestSingleton.Instance);
        }

        TestSingleton.ResetInstanceForTest();
    }

    [Test]
    public void Instance_IsCreated_WhenNotExists()
    {
        // Arrange & Act
        var instance = TestSingleton.Instance;

        // Assert
        Assert.NotNull(instance, "Singleton instance should be created");
        Assert.IsInstanceOf<TestSingleton>(instance);
    }

    [Test]
    public void Instance_IsNotDuplicated_WhenAlreadyExists()
    {
        // Arrange
        var firstInstance = TestSingleton.Instance;

        // Act
        var secondInstance = TestSingleton.Instance;

        // Assert
        Assert.AreSame(firstInstance, secondInstance, "Singleton instance should not be duplicated");
    }

    [Test]
    public void Instance_GameObjectName_IsCorrect()
    {
        // Act
        var instance = TestSingleton.Instance;

        // Assert
        Assert.AreEqual($"(singleton) {typeof(TestSingleton)}", instance.gameObject.name,
            "Singleton GameObject name should be set correctly");
    }

    [Test]
    public void Instance_IsNull_WhenApplicationIsQuitting()
    {
        // Arrange
        var instance = TestSingleton.Instance;

        // Simulate application quitting
        Object.DestroyImmediate(instance.gameObject);

        // Act
        var newInstance = TestSingleton.Instance;

        // Assert
        Assert.IsNull(newInstance, "Singleton instance should be null when application is quitting");
    }

    private const string _testScene1 = "Test Scene 1";
    private const string _testScene2 = "Test Scene 2";

    [UnityTest]
    public IEnumerator SingletonInstance_RemainsSame_AfterSceneChange()
    {
        // Arrange
        var instance = TestSingleton.Instance;
        Assert.IsNotNull(instance);

        // Ensure the instance is not destroyed when scene changes
        Assert.IsTrue(instance != null && instance.gameObject.scene.name == "DontDestroyOnLoad");

        // Act
        yield return SceneManager.LoadSceneAsync(_testScene1, LoadSceneMode.Single);

        var instanceAfterSceneChange = TestSingleton.Instance;

        // Assert
        Assert.AreEqual(instance, instanceAfterSceneChange, "Singleton instance should not be destroyed when scene changes");

        // Act
        yield return SceneManager.LoadSceneAsync(_testScene2, LoadSceneMode.Single);
        var finalInstance = TestSingleton.Instance;

        // Assert
        Assert.AreEqual(instance, finalInstance, "Singleton instance should not be destroyed when scene changes");
    }
}
#endif