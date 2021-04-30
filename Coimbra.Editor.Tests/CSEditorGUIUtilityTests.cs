﻿using NUnit.Framework;

namespace Coimbra.Editor.Tests
{
    [TestFixture]
    [TestOf(typeof(CSFrameworkEditorGUIUtility))]
    internal class CSEditorGUIUtilityTests
    {
        [TestCase("m_1_CSEditorGUIUtility2020_3LTS", "1 CS Editor GUI Utility 2020.3 LTS")]
        [TestCase("m_", "")]
        [TestCase("___", "")]
        [TestCase("_1__2", "1 2")]
        [TestCase("", "")]
        [TestCase("m_a", "A")]
        [TestCase("uiOne", "Ui One")]
        [TestCase("ui2", "Ui 2")]
        [TestCase("_guiCanvas", "Gui Canvas")]
        [TestCase("_gui_ui_Canvas", "Gui Ui Canvas")]
        public void ToDisplayNameWorks(string input, string expected)
        {
            Assert.AreEqual(expected, CSFrameworkEditorGUIUtility.ToDisplayName(input));
        }
    }
}
