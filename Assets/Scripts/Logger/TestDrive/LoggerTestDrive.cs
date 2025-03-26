using UnityEngine;

namespace hcore.Logger.TestDrive
{
    public class LoggerTestDrive : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Logs.Log<LoggerTestDrive>($"{KeyCode.A} was pressed.");
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Logs.LogWarning<LoggerTestDrive>($"{KeyCode.B} was pressed.");
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Logs.LogError<LoggerTestDrive>($"{KeyCode.C} was pressed.");
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                Logs.LogInfo<LoggerTestDrive>($"{KeyCode.D} was pressed.");
            }
        }
    }
}