namespace Elite.Engine
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static class CommanderFactory
    {
        /// <summary>
        /// The default commander. Do not modify.
        /// </summary>
        /// <returns>Commander Jameson.</returns>
        internal static Commander Jameson()
		{
			return new Commander()
            {
                name = "JAMESON",
                mission = 0,
                shiplocation = new(0x14, 0xAD),
                galaxy = new(0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7),
                credits = 100,
                fuel = 7,
                galaxy_number = 0,
                front_laser = elite.PULSE_LASER,
                rear_laser = 0,
                left_laser = 0,
                right_laser = 0,
                cargo_capacity = 20,
                current_cargo = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                ecm = false,
                fuel_scoop = false,
                energy_bomb = false,
                energy_unit = EnergyUnit.None,
                docking_computer = false,
                galactic_hyperdrive = false,
                escape_pod = false,
                missiles = 3,
                legal_status = 0,
                station_stock = new int[] {0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,
			        0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
                    0x3A, 0x07, 0x09, 0x08, 0x00},
                market_rnd = 0,
                score = 0,
                saved = 0x80
            };
		}

        /// <summary>
        /// The maximum equipment level, for testing purposes.
        /// </summary>
        /// <returns>Commander Max.</returns>
        internal static Commander Max()
        {
            return new Commander()
            {
                name = "MAX",
                mission = 0,
                shiplocation = new(0x14, 0xAD),
                galaxy = new(0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7),
                credits = 10000,
                fuel = 7,
                galaxy_number = 0,
                front_laser = elite.MILITARY_LASER,
                rear_laser = elite.MILITARY_LASER,
                left_laser = elite.MILITARY_LASER,
                right_laser = elite.MILITARY_LASER,
                cargo_capacity = 35,
                current_cargo = new int[] { 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0 },
                ecm = true,
                fuel_scoop = true,
                energy_bomb = true,
                energy_unit = EnergyUnit.Naval,
                docking_computer = true,
                galactic_hyperdrive = true,
                escape_pod = true,
                missiles = 4,
                legal_status = 0,
                station_stock = new int[] {0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,
			        0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
                    0x3A, 0x07, 0x09, 0x08, 0x00},
                market_rnd = 0,
                score = 0x1900,
                saved = 0x80
            };
        }
    }
}
