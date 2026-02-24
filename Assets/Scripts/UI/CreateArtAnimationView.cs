using System;
using UnityEngine;

/// <summary>
/// Create Art 動畫視圖：繼承 AnimatorBase，在 Create Art 時開啟，隔一段時間後關閉。
/// </summary>
public class CreateArtAnimationView : AnimatorBase
{
    [SerializeField] private float displayDuration = 2f;

    /// <summary>
    /// 播放動畫：Open → 等待 displayDuration 秒 → Close。
    /// 使用 Awaitable 讓調用方可以選擇 await 此方法。
    /// </summary>
    public async Awaitable PlayAnimationAsync()
    {
        ClearAllAction();
        
        Open();
        
        await Awaitable.WaitForSecondsAsync(displayDuration);

        
        
        Close();

        while(!IsClosed)
        {
            await Awaitable.NextFrameAsync();
        }
    }
}