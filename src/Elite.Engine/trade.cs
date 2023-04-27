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
	using Elite.Engine.Enums;
	using Elite.Engine.Ships;
	using Elite.Engine.Types;

	internal class trade
	{
        private readonly GameState _gameState;
        private readonly swat _swat;
		private readonly PlayerShip _ship;

		internal trade(GameState gameState, swat swat, PlayerShip ship)
		{
            _gameState = gameState;
            _swat = swat;
			_ship = ship;
        }

		internal int total_cargo()
		{
			int cargo_held = 0;

			for (int i = 0; i < 17; i++)
			{
				if ((_gameState.cmdr.current_cargo[i] > 0) &&
					(_gameState.stock_market[i].units == GameState.TONNES))
				{
					cargo_held += _gameState.cmdr.current_cargo[i];
				}
			}

			return cargo_held;
		}

		internal void scoop_item(int un)
		{
			SHIP type;
			int trade;

			if (space.universe[un].flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			type = space.universe[un].type;

			if (type == SHIP.SHIP_MISSILE)
            {
                return;
            }

            if ((!_ship.hasFuelScoop) || (space.universe[un].location.Y >= 0) ||
				(total_cargo() == _ship.cargoCapacity))
			{
				_swat.explode_object(un);
                _ship.DamageShip(128 + (space.universe[un].energy / 2), space.universe[un].location.Z > 0);
				return;
			}

			if (type == SHIP.SHIP_CARGO)
			{
				trade = RNG.Random(7);
                _gameState.cmdr.current_cargo[trade]++;
                elite.info_message(_gameState.stock_market[trade].name);
				swat.remove_ship(un);
				return;
			}

			if (elite.ship_list[(int)type].scoop_type != 0)
			{
				trade = elite.ship_list[(int)type].scoop_type + 1;
                _gameState.cmdr.current_cargo[trade]++;
                elite.info_message(_gameState.stock_market[trade].name);
				swat.remove_ship(un);
				return;
			}

			_swat.explode_object(un);
            _ship.DamageShip(space.universe[un].energy / 2, space.universe[un].location.Z > 0);
		}
	}
}