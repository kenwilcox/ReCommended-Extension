﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantCapturedContext : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\Await";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantCapturedContextHighlighting;

        [Test]
        public void TestRedundantCapturedContext() => DoNamedTest2();
    }
}