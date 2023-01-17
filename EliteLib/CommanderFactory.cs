namespace Elite
{
    using Elite.Structs;

    internal static class CommanderFactory
    {
        /// <summary>
        /// The default commander. Do not modify.
        /// </summary>
        /// <returns>Commander Jameson.</returns>
        internal static Commander Jameson()
		{
			return new Commander(
				"JAMESON",                                  /* Name 			*/
				0,                                          /* Mission Number 	*/
				0x14, 0xAD,                                 /* Ship X,Y			*/

				new(0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7),    /* Galaxy Seed		*/
				1000,                                       /* Credits * 10		*/
				70,                                         /* Fuel	* 10		*/
				0,                                          /* Galaxy - 1		*/
				elite.PULSE_LASER,                          /* Front Laser		*/
				0,                                          /* Rear Laser		*/
				0,                                          /* Left Laser		*/
				0,                                          /* Right Laser		*/
				20,                                         /* Cargo Capacity	*/
				new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },        /* Current Cargo	*/
				false,                                          /* ECM				*/
				false,                                          /* Fuel Scoop		*/
				false,                                          /* Energy Bomb		*/
				0,                                              /* Energy Unit		*/
				false,                                          /* Docking Computer */
				false,                                          /* Galactic H'Drive	*/
				false,                                          /* Escape Pod		*/
				3,                                          /* No. of Missiles	*/
				0,                                          /* Legal Status		*/
				new int[] {0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,		/* Station Stock	*/
			        0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
			        0x3A, 0x07, 0x09, 0x08, 0x00},
				0,                                          /* Fluctuation		*/
				0,                                          /* Score			*/
				0x80                                        /* Saved			*/
			);
		}

        /// <summary>
        /// The maximum equipment level, for testing purposes.
        /// </summary>
        /// <returns>Commander Max.</returns>
        internal static Commander Max()
        {
            return new Commander(
                "MAX",                                      /* Name 			*/
                0,                                          /* Mission Number 	*/
                0x14, 0xAD,                                 /* Ship X,Y			*/

                new(0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7),    /* Galaxy Seed		*/
                10000,                                      /* Credits * 10		*/
                70,                                         /* Fuel	* 10		*/
                0,                                          /* Galaxy - 1		*/
                elite.MILITARY_LASER,                       /* Front Laser		*/
                elite.MILITARY_LASER,                       /* Rear Laser		*/
                elite.MILITARY_LASER,                       /* Left Laser		*/
                elite.MILITARY_LASER,                       /* Right Laser		*/
                35,                                         // Large Cargo Bay
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },        /* Current Cargo	*/
                true,                                          /* ECM				*/
                true,                                          /* Fuel Scoop		*/
                true,                                          /* Energy Bomb		*/
                2,                                             /* Energy Unit		*/
                true,                                          /* Docking Computer */
                true,                                          /* Galactic H'Drive	*/
                true,                                          /* Escape Pod		*/
                4,                                          /* No. of Missiles	*/
                0,                                          /* Legal Status		*/
                new int[] {0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,		/* Station Stock	*/
			        0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
                    0x3A, 0x07, 0x09, 0x08, 0x00},
                0,                                          /* Fluctuation		*/
                0x1900,                                     /* Score			*/
                0x80                                        /* Saved			*/
            );
        }
    }
}
