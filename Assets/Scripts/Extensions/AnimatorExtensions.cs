using UnityEngine;

/// <summary>
/// Extension methods for Unity Animator.
/// </summary>
public static class AnimatorExtensions
{
    /// <summary>
    /// Resets all trigger parameters on the animator so no trigger remains set.
    /// Call before SetTrigger to avoid stale triggers affecting the next transition.
    /// </summary>
    public static void ResetAllTriggers(this Animator animator)
    {
        if (animator == null || !animator.isInitialized) return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(parameter.name);
            }
        }
    }
}
