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

#ifndef SWAT_H
#define SWAT_H

#include "space.h"

#define MISSILE_UNARMED	-2
#define MISSILE_ARMED	-1

extern int ecm_active;
extern int missile_target;
extern int in_battle;

void reset_weapons ();
void tactics (int un);
int in_target (int type, double x, double y, double z);
void check_target (int un, struct univ_object *flip);
void check_missiles (int un);
void draw_laser_lines ();
int fire_laser ();
void cool_laser ();
void arm_missile ();
void unarm_missile ();
void fire_missile ();
void activate_ecm (int ours);
void time_ecm ();
void random_encounter ();
void explode_object (int un);
void abandon_ship ();
void create_thargoid ();
void dock_it (struct univ_object *ship);



#endif
