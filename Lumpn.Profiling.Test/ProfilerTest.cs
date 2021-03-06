﻿using System.Threading;
using NUnit.Framework;

namespace Lumpn.Profiling.Test
{
    [TestFixture]
    public sealed class ProfilerTest
    {
        [Test]
        public void ExportToUnity()
        {
            for (int i = 0; i < 20; i++)
            {
                Profiler.BeginFrame();
                {
                    Profiler.BeginSample("Foo");
                    Thread.Sleep(5);
                    {
                        Profiler.BeginSample("Bar");
                        Thread.Sleep(5);
                        {
                            Profiler.BeginSample("Baz");
                            Thread.Sleep(10);
                            Profiler.EndSample();
                        }
                        Profiler.EndSample();
                    }
                    Profiler.EndSample();
                }
                Profiler.EndFrame();
            }

            Profiler.ExportToUnityProfileAnalyzer("samples.pdata");
        }
    }
}
