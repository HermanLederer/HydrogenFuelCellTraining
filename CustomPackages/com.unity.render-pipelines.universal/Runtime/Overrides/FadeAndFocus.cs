using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/FadeAndFocus")]
    public sealed class FadeAndFocus : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter fade = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter focus = new ClampedFloatParameter(0f, 0f, 1f);
        public Vector3Parameter focusDirection = new Vector3Parameter(Vector3.forward);

        public bool IsActive() => fade.value > 0f || focus.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
