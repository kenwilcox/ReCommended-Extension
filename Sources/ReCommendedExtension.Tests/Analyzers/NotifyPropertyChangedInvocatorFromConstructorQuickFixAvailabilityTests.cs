using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [TestPackagesWithAnnotations]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructorQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is NotifyPropertyChangedInvocatorFromConstructorHighlighting;

        [Test]
        public void TestNotifyPropertyChangedInvocatorFromConstructorAvailability() => DoNamedTest2();
    }
}