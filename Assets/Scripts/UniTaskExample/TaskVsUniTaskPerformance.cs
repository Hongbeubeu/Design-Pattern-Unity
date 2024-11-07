using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks; // Cần đảm bảo UniTask được cài đặt
using UnityEngine;

public class TaskVsUniTaskPerformance : MonoBehaviour
{
    private async void Start()
    {
        // Đo hiệu năng của Task
        var stopwatch = new Stopwatch();

        // Chạy Task và đo thời gian
        for (var i = 0; i < 10; i++)
        {
            stopwatch.Reset();
            stopwatch.Start();
            await RunTaskAsync();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Task execution time: {stopwatch.ElapsedMilliseconds} ms");
            
            stopwatch.Reset();
            stopwatch.Start();
            await RunUniTaskAsync();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"UniTask execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    // Task method
    private static async Task RunTaskAsync()
    {
        // Mô phỏng công việc bất đồng bộ
        await Task.Delay(1000);
    }

    // UniTask method
    private static async UniTask RunUniTaskAsync()
    {
        // Mô phỏng công việc bất đồng bộ
        await UniTask.Delay(1000);
    }
}