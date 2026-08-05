// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Widgets;

namespace EliteSharpLib.Config;

/// <summary>
/// Writes the config file after a setting changes. Every change on a settings
/// screen is live and saved as it is made, so rather than each setting
/// remembering to save, the saving wraps the setting: whatever it is bound to
/// underneath, it is persisted the moment it is applied.
/// </summary>
/// <param name="setting">The setting to save after.</param>
/// <param name="save">Writes the config file.</param>
internal sealed class SavedSetting(ISetting setting, Action save) : ISetting
{
    public string Name => setting.Name;

    public IReadOnlyList<string> Values => setting.Values;

    public int SelectedIndex
    {
        get => setting.SelectedIndex;

        set
        {
            setting.SelectedIndex = value;
            save();
        }
    }
}
