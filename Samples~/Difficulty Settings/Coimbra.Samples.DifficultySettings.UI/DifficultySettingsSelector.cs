using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coimbra.Samples.DifficultySettings.UI
{
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings Selector")]
    public class DifficultySettingsSelector : Actor
    {
        [SerializeField]
        private SerializableDictionary<Button, DifficultySettings> _difficultySettingsButtons = new();

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
