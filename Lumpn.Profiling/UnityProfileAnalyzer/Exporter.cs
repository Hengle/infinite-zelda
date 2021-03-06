﻿using System.Collections.Generic;
using System.IO;

namespace Lumpn.Profiling.UnityProfileAnalyzer
{
    public sealed class Exporter
    {
        private readonly Dictionary<string, int> markers = new Dictionary<string, int>();
        private readonly ProfileData data = new ProfileData(0);

        public Exporter()
        {
            data.AddThreadName("1:Main Thread");
        }

        public void Convert(IEnumerable<Frame> frames, long frequency)
        {
            foreach (var frame in frames)
            {
                var msStartTime = frame.CalcStartTime(frequency);
                var msFrame = frame.CalcElapsedMilliseconds(frequency);
                var profileFrame = new ProfileFrame(msStartTime, msFrame);
                data.Add(profileFrame);

                var thread = new ProfileThread(0);
                profileFrame.Add(thread);

                foreach (var child in frame.Root.Children)
                {
                    AddMarkers(thread, child, 1, frequency);
                }
            }
        }

        private void AddMarkers(ProfileThread thread, Sample sample, int depth, long frequency)
        {
            var nameIndex = GetOrCreateMarker(sample.Name);
            var msMarkerTotal = sample.CalcElapsedMilliseconds(frequency);
            var marker = new ProfileMarker(nameIndex, msMarkerTotal, depth);
            thread.Add(marker);

            foreach (var child in sample.Children)
            {
                AddMarkers(thread, child, depth + 1, frequency);
            }
        }

        private int GetOrCreateMarker(string name)
        {
            if (!markers.TryGetValue(name, out int index))
            {
                index = data.AddMarkerName(name);
                markers.Add(name, index);
            }
            return index;
        }

        public void Export(string filename)
        {
            using (var stream = File.Create(filename))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    data.Write(writer);
                }
            }
        }
    }
}
