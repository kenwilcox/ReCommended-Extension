﻿using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Search;

namespace ReCommendedExtension.Analyzers.AsyncVoid
{
    [ElementProblemAnalyzer(
        typeof(ICSharpDeclaration),
        HighlightingTypes = new[] { typeof(AsyncVoidFunctionExpressionHighlighting), typeof(AvoidAsyncVoidHighlighting) })]
    public sealed class AsyncVoidAnalyzer : ElementProblemAnalyzer<ICSharpDeclaration>
    {
        [Pure]
        static bool IsPublicSurfaceArea([NotNull] IMethod method)
        {
            switch (method.AccessibilityDomain.DomainType)
            {
                case AccessibilityDomain.AccessibilityDomainType.PUBLIC:
                case AccessibilityDomain.AccessibilityDomainType.PROTECTED:
                case AccessibilityDomain.AccessibilityDomainType.PROTECTED_OR_INTERNAL:
                    return true;

                default: return false;
            }
        }

        static void Analyze([NotNull] IMethodDeclaration methodDeclaration, [NotNull] IHighlightingConsumer consumer)
        {
            if (!methodDeclaration.IsAsync || !methodDeclaration.IsVoidMethodDeclaration())
            {
                return; // not an "async void" method
            }

            var method = methodDeclaration.DeclaredElement;
            if (method == null)
            {
                return; // cannot analyze
            }

            if (method.GetImmediateSuperMembers().Any())
            {
                consumer.AddHighlighting(
                    new AvoidAsyncVoidHighlighting("'void' method overridden or implemented as 'async void'.", methodDeclaration));
                return;
            }

            // find usages
            var psiServices = method.GetPsiServices();

            Debug.Assert(SearchDomainFactory.Instance != null);

            var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
            var references = psiServices.Finder.FindReferences(method, solutionSearchDomain, NullProgressIndicator.Create());

            if (IsPublicSurfaceArea(method))
            {
                if (references.Length > 0)
                {
                    consumer.AddHighlighting(
                        new AvoidAsyncVoidHighlighting("'async void' public surface area method with detected usages.", methodDeclaration));
                }
                else
                {
                    var implicitUseAnnotationProvider = psiServices.GetCodeAnnotationsCache().GetProvider<ImplicitUseAnnotationProvider>();

                    var useKindFlags = implicitUseAnnotationProvider.IsImplicitlyUsed(method);
                    if (useKindFlags == null)
                    {
                        // [UsedImplicitly] annotation not applied
                        consumer.AddHighlighting(
                            new AvoidAsyncVoidHighlighting("'async void' public surface area method without detected usages.", methodDeclaration));
                    }
                }
            }
            else
            {
                var count = references.Count(reference => !reference.AssertNotNull().IsEventTarget());
                if (count > 0)
                {
                    consumer.AddHighlighting(
                        new AvoidAsyncVoidHighlighting(
                            string.Format(
                                "'async void' method used {0} time{1} not as a direct event handler.",
                                count.ToString(),
                                count == 1 ? "" : "s"),
                            methodDeclaration));
                }
            }
        }

        static void Analyze([NotNull] ILocalFunctionDeclaration localFunctionDeclaration, [NotNull] IHighlightingConsumer consumer)
        {
            if (!localFunctionDeclaration.IsAsync || !localFunctionDeclaration.IsVoidMethodDeclaration())
            {
                return; // not an "async void" local function
            }

            var psiServices = localFunctionDeclaration.GetPsiServices();

            Debug.Assert(SearchDomainFactory.Instance != null);

            var solutionSearchDomain = SearchDomainFactory.Instance.CreateSearchDomain(psiServices.Solution, false);
            var references = psiServices.Finder.FindReferences(
                localFunctionDeclaration.DeclaredElement,
                solutionSearchDomain,
                NullProgressIndicator.Create());
            var count = references.Count(reference => !reference.AssertNotNull().IsEventTarget());
            if (count > 0)
            {
                consumer.AddHighlighting(
                    new AvoidAsyncVoidHighlighting(
                        string.Format(
                            "'async void' local function used {0} time{1} not as a direct event handler.",
                            count.ToString(),
                            count == 1 ? "" : "s"),
                        localFunctionDeclaration));
            }
        }

        static void Analyze([NotNull] ILambdaExpression lambdaExpression, [NotNull] IHighlightingConsumer consumer)
        {
            if (!lambdaExpression.IsAsync || !lambdaExpression.ReturnType.IsVoid())
            {
                return; // not an "async (...) => ..." expression that returns void
            }

            if (lambdaExpression.Parent is IAssignmentExpression assignmentExpression && assignmentExpression.IsEventSubscriptionOrUnSubscription())
            {
                return; // direct event target
            }

            Debug.Assert(lambdaExpression.AsyncKeyword != null);

            consumer.AddHighlighting(
                new AsyncVoidFunctionExpressionHighlighting(
                    "'async void' lambda expression not used as a direct event handler.",
                    lambdaExpression.AsyncKeyword,
                    () => lambdaExpression.SetAsync(false)));
        }

        static void Analyze([NotNull] IAnonymousMethodExpression anonymousMethodExpression, [NotNull] IHighlightingConsumer consumer)
        {
            if (!anonymousMethodExpression.IsAsync || !anonymousMethodExpression.ReturnType.IsVoid())
            {
                return; // not an "async delegate (...) { ... }" that returns void
            }

            if (anonymousMethodExpression.Parent is IAssignmentExpression assignmentExpression &&
                assignmentExpression.IsEventSubscriptionOrUnSubscription())
            {
                return; // direct event target
            }

            Debug.Assert(anonymousMethodExpression.AsyncKeyword != null);

            consumer.AddHighlighting(
                new AsyncVoidFunctionExpressionHighlighting(
                    "'async void' anonymous method expression not used as a direct event handler.",
                    anonymousMethodExpression.AsyncKeyword,
                    () => anonymousMethodExpression.SetAsync(false)));
        }

        protected override void Run(ICSharpDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            switch (element)
            {
                case IMethodDeclaration methodDeclaration:
                    Analyze(methodDeclaration, consumer);
                    break;

                case ILocalFunctionDeclaration localFunctionDeclaration:
                    Analyze(localFunctionDeclaration, consumer);
                    break;

                case ILambdaExpression lambdaExpression:
                    Analyze(lambdaExpression, consumer);
                    break;

                case IAnonymousMethodExpression anonymousMethodExpression:
                    Analyze(anonymousMethodExpression, consumer);
                    break;
            }
        }
    }
}