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

#endif
