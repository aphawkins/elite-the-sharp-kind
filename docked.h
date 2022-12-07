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

#ifndef DOCKED_H
#define DOCKED_H

void display_short_range_chart ();
void display_galactic_chart ();
void display_data_on_planet ();
void show_distance_to_planet ();
void move_cursor_to_origin ();
void find_planet_by_name (char *find_name);
void display_market_prices ();
void display_commander_status ();
int calc_distance_to_planet (galaxy_seed from_planet, galaxy_seed to_planet);
void highlight_stock (int i);
void select_previous_stock ();
void select_next_stock ();
void buy_stock ();
void sell_stock ();
void display_inventory ();
void equip_ship ();
void select_next_equip ();
void select_previous_equip ();
void buy_equip ();


extern int cross_x;
extern int cross_y;

#endif

