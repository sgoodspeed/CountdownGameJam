using UnityEngine;

namespace Countdown
{
    public static class HourMarkerHealthIndicatorFactory
    {
        public struct Config
        {
            public Transform Parent;
            public int Count;
            public float Radius;
            public GameObject Prefab;
            public int SortingOrder;
        }

        public static SpriteRenderer[] Create(Config config)
        {
            var indicators = new SpriteRenderer[config.Count];

            for (int i = 0; i < config.Count; i++)
            {
                float angle = 90f - (i * 360f / config.Count);
                float rad = angle * Mathf.Deg2Rad;
                var localPos = new Vector3(
                    Mathf.Cos(rad) * config.Radius,
                    Mathf.Sin(rad) * config.Radius,
                    0f
                );

                SpriteRenderer sr;
                sr = CreateFromPrefab(config, i, localPos);

                indicators[i] = sr;
                sr.gameObject.SetActive(false);
            }

            return indicators;
        }

        private static SpriteRenderer CreateFromPrefab(Config config, int index, Vector3 localPos)
        {
            var go = Object.Instantiate(config.Prefab, config.Parent);
            go.name = $"HealthIndicator_{index}";
            go.transform.localPosition = localPos;

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = go.AddComponent<SpriteRenderer>();

            return sr;
        }
    }
}
