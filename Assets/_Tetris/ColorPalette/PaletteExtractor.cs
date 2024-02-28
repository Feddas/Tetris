using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu]
public class PaletteExtractor : ScriptableObject
{
    [SerializeField] private ScriptableObject palette;

    [SerializeField] private List<ColorPreset> colorPresets = new List<ColorPreset>();

    public List<ColorPreset> ColorPresets
    {
        get { return colorPresets; }
        set { colorPresets = value; }
    }

    [ContextMenu("Read palettes")]
    private void Import()
    {
        if (palette == null) return;

        var type = palette.GetType();
        if (type.Name != "ColorPresetLibrary")
        {
            Debug.LogWarning($"'{type.Name}' is not a color Palette");
            return;
        }

        var presetsField = type.GetField("m_Presets", BindingFlags.Instance | BindingFlags.NonPublic);
        if (presetsField == null)
        {
            Debug.LogWarning("ColorPresetLibrary implementation changed, fix the script");
            return;
        }

        IList presets = presetsField.GetValue(palette) as IList;
        if (presets == null || presets.Count == 0) return;

        var presetType       = presets[0].GetType();
        var presetColorField = presetType.GetProperty("color", BindingFlags.Instance | BindingFlags.Public);
        var presetNameField  = presetType.GetProperty("name", BindingFlags.Instance | BindingFlags.Public);

        if (presetColorField == null || presetNameField == null)
        {
            Debug.LogWarning("ColorPreset implementation changed, fix the script");
            return;
        }

        ColorPresets.Clear();
        foreach (var preset in presets)
        {
            var color = (Color)presetColorField.GetValue(preset);
            var name  = (string)presetNameField.GetValue(preset);
            // Debug.Log($"ColorPreset {name} added");
            ColorPresets.Add(new ColorPreset(color, name));
        }
    }

    private void OnValidate()
    {
        Import();
    }

    [Serializable]
    public class ColorPreset
    {
        [field: Tooltip("Treat this field as read-only. Edit the Palette ScriptableObject referenced above.")]
        [field: SerializeField] public Color Color { get; set; }
        [field: SerializeField] public string Name { get; set; }

        public ColorPreset(Color color, string name)
        {
            Color = color;
            Name  = name;
        }
    }
}