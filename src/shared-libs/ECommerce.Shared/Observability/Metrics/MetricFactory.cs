using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Observability.Metrics
{
    public class MetricFactory
    {
        private readonly Meter _meter;
        private readonly Dictionary<string, Counter<int>> _cachedCounters = new();
        private readonly Dictionary<string, Histogram<int>> _histogramCounters = new();

        public MetricFactory(string meterName)
        {
            _meter = new Meter(meterName);
        }
        public Counter<int> Counter(string name, string? unit = null)
        {
            if(_cachedCounters.TryGetValue(name, out Counter<int>? value))
                return value;

            var counter = _meter.CreateCounter<int>(name, unit);
            _cachedCounters.Add(name, counter);

            return counter;
        }
        public Histogram<int> Histogram(string name, string? units = null)
        {
            if (_histogramCounters.TryGetValue(name, out var value))
                return value;

            var histogram = _meter.CreateHistogram<int>(name, units);
            _histogramCounters.Add(name, histogram);
            return histogram;
        }
    }
}
