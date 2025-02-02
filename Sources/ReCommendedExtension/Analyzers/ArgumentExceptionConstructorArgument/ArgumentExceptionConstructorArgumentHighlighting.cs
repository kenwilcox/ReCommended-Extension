﻿using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ArgumentExceptionConstructorArgumentHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Parameter name used for the exception message" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ArgumentExceptionConstructorArgumentHighlighting : Highlighting
    {
        internal const string SeverityId = "ArgumentExceptionConstructorArgument";

        [NotNull]
        readonly ICSharpArgument argument;

        public ArgumentExceptionConstructorArgumentHighlighting([NotNull] string message, [NotNull] ICSharpArgument argument) : base(message)
            => this.argument = argument;

        public override DocumentRange CalculateRange() => argument.GetDocumentRange();
    }
}