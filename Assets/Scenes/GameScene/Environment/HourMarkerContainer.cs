using System.Collections.Generic;
using UnityEngine;

namespace Countdown
{
    public class HourMarkerContainer : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private HourMarker[] hourMarkers;
        private Dictionary<int, HourMarker> hourMap;

        public int MarkerCount { get => hourMarkers.Length; }
        
        public int ActiveMarkers
        {
            get
            {
                var activeCount = 0;
                foreach (var marker in hourMarkers)
                {
                    if (marker.Phase == HourMarkerPhase.Active) { activeCount++; }
                }

                return activeCount;
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
