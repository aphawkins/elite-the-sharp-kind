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

#ifndef ELITE_H
#define ELITE_H

#include "planet.h"
#include "trade.h"

#define PULSE_LASER		0x0F
#define BEAM_LASER		0x8F
#define MILITARY_LASER	0x97
#define MINING_LASER	0x32


#define MAX_UNIV_OBJECTS	20


struct commander
{
	char name[32];
	int mission;
	int ship_x;
	int ship_y;
	struct galaxy_seed galaxy;
	int credits;
	int fuel;
	int unused1;
	int	galaxy_number;
	int front_laser;
	int rear_laser;
	int left_laser;
	int right_laser;
	int unused2;
	int unused3;
	int cargo_capacity;
	int current_cargo[NO_OF_STOCK_ITEMS];
	int ecm;
	int fuel_scoop;
	int energy_bomb;
	int energy_unit;
	int docking_computer;
	int galactic_hyperdrive;
	int escape_pod;
	int unused4;
	int unused5;
	int unused6;
	int unused7;
	int missiles;
	int legal_status;
	int station_stock[NO_OF_STOCK_ITEMS];
	int market_rnd;
	int score;
	int saved;
};

struct player_ship
{
	int max_speed;
	int max_roll;
	int max_climb;
	int max_fuel;
	int altitude;
	int cabtemp;
};

extern char scanner_filename[256];
extern int hoopy_casinos;
extern int instant_dock;
extern int speed_cap;
extern int scanner_cx;
extern int scanner_cy;
extern int compass_centre_x;
extern int compass_centre_y;

extern int planet_render_style;

extern int game_over;
extern int docked;
extern int finish;
extern int flight_speed;
extern int flight_roll;
extern int flight_climb;
extern int front_shield;
extern int aft_shield;
extern int energy;
extern int laser_temp;
extern int mcount;
extern int detonate_bomb;
extern int witchspace;
extern int auto_pilot;


void restore_saved_commander (void);


#endif
