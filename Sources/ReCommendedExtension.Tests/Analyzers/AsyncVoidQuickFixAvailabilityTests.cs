﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.AsyncVoid;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AsyncVoidQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\AsyncVoidQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is AsyncVoidFunctionExpressionHighlighting || highlighting is AvoidAsyncVoidHighlighting;

        [Test]
        public void TestAnonymousMethodAvailability() => DoNamedTest2();

        [Test]
        public void TestLambdaExpressionAvailability() => DoNamedTest2();

        [Test]
        public void TestAsyncVoidMethodAvailability() => DoNamedTest2();

        [Test]
        public void TestAsyncVoidLocalFunctionAvailability() => DoNamedTest2();
    }
}