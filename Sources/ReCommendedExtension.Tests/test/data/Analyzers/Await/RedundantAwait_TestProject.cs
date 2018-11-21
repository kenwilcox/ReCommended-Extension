﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    public class AwaitForMethods
    {
        [TestMethod]
        async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [TestMethod]
        async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [TestMethod]
        async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [TestMethod]
        async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [TestMethod]
        async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [TestMethod]
        async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [TestMethod]
        async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [TestMethod]
        async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }
}