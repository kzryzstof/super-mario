using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    public class TimeBehavior : MonoBehaviour
    {
        private TextMeshProUGUI _time;
        private readonly Stopwatch _stopwatch;
        private const int AvailableTime = 360;
        
        public TimeBehavior()
        {
            _stopwatch = Stopwatch.StartNew();
        }
    
        public void Start()
        {
            _time = GetComponent<TextMeshProUGUI>();
        }

        public void Update()
        {
            _time.text = $"{AvailableTime - (int)_stopwatch.Elapsed.TotalSeconds:00#}";
        }
    }
}
