/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine
{
    internal static partial class Settings
	{
        internal class Setting
		{
			internal string name;
			internal string[] value;

			internal Setting(string name, string[] value)
			{
				this.name = name;
				this.value = value;
			}
		};
	}
}