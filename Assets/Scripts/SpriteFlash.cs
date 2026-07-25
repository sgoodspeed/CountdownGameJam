using System;
using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    [Serializable]
    public class SpriteFlashSettings
    {
        [ColorUsage(true, true)]
        public Color color = Color.white;
        [Range(0f, 1f)] public float intensity = 1f;
        public float duration = 0.3f;
        public int flashes = 2;
    }

    public static class SpriteFlash
    {
        public static Tween Play(SpriteRenderer[] renderers, SpriteFlashSettings settings)
        {
            if (renderers == null || renderers.Length == 0 || settings.flashes <= 0)
                return null;

            var originals = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                originals[i] = renderers[i].color;

            float blend = 0f;
            float halfFlash = settings.duration / (settings.flashes * 2);

            return DOTween.To(
                () => blend,
                x =>
                {
                    blend = x;
                    for (int i = 0; i < renderers.Length; i++)
                        renderers[i].color = Color.Lerp(originals[i], settings.color, x);
                },
                settings.intensity,
                halfFlash
            ).SetLoops(settings.flashes * 2, LoopType.Yoyo);
        }
    }
}
