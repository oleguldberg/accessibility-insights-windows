﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UITests
{
    [TestClass]
    public class LiveMode : AIWinSession
    {
        readonly string TestAppPath = Path.GetFullPath("../../../../tools/WildlifeManager/WildlifeManager.exe");
        Process _wildlifeManager;

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void LiveModeTests()
        {
            TestLiveMode();
            TestTestResults();
        }

        private void TestTestResults()
        {
            driver.LiveMode.RunTests();
            var testsComplete = WaitFor(() => !driver.Title.Contains("(Scanning)"), new TimeSpan(0,0,0,0,500), 10);

            Assert.IsTrue(testsComplete);
            VerifyTestModeTitle();
            ScanAccessibility();
        }

        private void TestLiveMode()
        {
            var appOpened = WaitFor(() => _wildlifeManager.MainWindowTitle == "Wildlife Manager 2.0", new TimeSpan(0, 0, 1), 10, _wildlifeManager.Refresh);
            var appSelected = WaitFor(() => driver.LiveMode.SelectedElementText == "window 'Wildlife Manager 2.0'", new TimeSpan(0, 0, 1), 5);
            driver.LiveMode.TogglePause();

            Assert.IsTrue(appOpened);
            Assert.IsTrue(appSelected);
            VerifyLiveModeTitle();
            ScanAccessibility();
        }

        private void VerifyLiveModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Inspect - Live", driver.Title);
        }

        private void VerifyTestModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Test - Test results", driver.Title);
        }

        private void ScanAccessibility()
        {
            var issueCount = driver.ScanAIWin(TestContext, "EventPage");
            Assert.AreEqual(0, issueCount);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();

            _wildlifeManager = Process.Start(TestAppPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _wildlifeManager?.CloseMainWindow();
            _wildlifeManager?.Kill();
            TearDown();
        }
    }
}