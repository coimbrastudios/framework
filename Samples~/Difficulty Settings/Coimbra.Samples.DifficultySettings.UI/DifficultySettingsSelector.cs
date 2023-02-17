using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coimbra.Samples.DifficultySettings.UI
{
    /// <summary>
    /// Pretty simple UI implementation that uses a <see cref="SerializableDictionary{TKey,TValue}"/> to define with <see cref="Button"/> should change to each <see cref="DifficultySettings"/>.
    /// </summary>
    /// <seealso cref="DifficultyListSettings"/>
    /// <seealso cref="DifficultySettings"/>
    /// <seealso cref="DifficultySettingsCube"/>
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings Selector")]
    public class DifficultySettingsSelector : Actor
    {
        [SerializeField]
        private SerializableDictionary<Button, DifficultySettings> _difficultySettingsButtons = new();

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();

            foreach (KeyValuePair<Button, DifficultySettings> pair in _difficultySettingsButtons)
            {
                pair.Key.onClick.AddListener(delegate
                {
                    ScriptableSettings.SetOrOverwrite(pair.Value);
                });
            }
        }
    }
}
