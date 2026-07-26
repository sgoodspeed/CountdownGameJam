using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Countdown
{
    public class HourMarkerContainer : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private HourMarker[] hourMarkers;
        private Dictionary<int, HourMarker> hourMap;

        public int MarkerCount { get => hourMarkers.Length; }
        
        public HourMarker HighestActiveMarker
        {
            get
            {
                HourMarker highest = null;
                foreach (var marker in hourMarkers)
                {
                    if (marker.Phase == HourMarkerPhase.Active && (highest?.Hour ?? 0) < marker.Hour)
                    {
                        highest = marker;
                    }
                }

                return highest;
            }
        }
        
        private void Awake()
        {
            hourMap = new Dictionary<int, HourMarker>();
            foreach (var marker in hourMarkers)
            {
                hourMap.Add(marker.Hour, marker);
            }
        }

        public bool AnyActiveMarkerAbove(int thisHour)
        {
            foreach (var marker in hourMarkers)
            {
                if (marker.Hour > thisHour && marker.Phase == HourMarkerPhase.Active)
                {
                    return true;
                }
            }
            
            return false;
        }

        public int FindNextLowestHour(int thisHour)
        {
            var lowest = thisHour;
            for(var i = lowest - 1; i >= 0; i--)
            {
                if (hourMap.TryGetValue(i, out var nextMarker)) 
                {
                    switch (nextMarker.Phase)
                    {
                        case HourMarkerPhase.Active: return lowest;
                        case HourMarkerPhase.Destroyed: 
                            lowest = nextMarker.Hour;
                            break;
                    }
                }
            }
            
            return lowest;
        }

        public List<Vector2> GetActiveMarkerPositions()
        {
            return hourMarkers
                .Where(m => m.Phase == HourMarkerPhase.Active)
                .Select(m => (Vector2)m.transform.position)
                .ToList();
        }

        public void OnBeforeSerialize()
        {
            if (hourMarkers == null || hourMarkers.Length == 0)
            {
                hourMarkers = GetComponentsInChildren<HourMarker>();
            }
        }
        public void OnAfterDeserialize() { }
    }
}
