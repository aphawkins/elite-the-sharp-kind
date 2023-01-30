namespace Elite.Engine
{
	using System.Numerics;
	using Elite.Common.Enums;
	using Elite.Engine.Enums;

	/// <summary>
	/// Rolling Cobra MkIII.
	/// </summary>
	internal class Intro1
	{
		private readonly IGfx _gfx;
        private readonly space _space;
        private readonly Vector3[] intro_ship_matrix = new Vector3[3];

		internal Intro1(IGfx gfx, space space)
		{
			_gfx = gfx;
			_space = space;

            swat.clear_universe();
			VectorMaths.set_init_matrix(ref intro_ship_matrix);
			swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, 4500), intro_ship_matrix, -127, -127);
		}

		internal void Update()
		{
			space.universe[0].location.Z -= 100;

			if (space.universe[0].location.Z < 384)
			{
				space.universe[0].location.Z = 384;
			}

            elite.draw.ClearDisplay();

			elite.flight_roll = 1;
			_space.update_universe();

            _gfx.DrawImage(Image.EliteText, new(-1, 10));

            _gfx.DrawTextCentre(310, "Original Game (C) I.Bell & D.Braben.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(330, "Re-engineered by C.J.Pinder.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(360, "Load New Commander (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
		}
	}
}