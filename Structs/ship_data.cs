namespace Elite.Structs
{
    internal struct ship_data
    {
        internal string name;
        internal int num_points;
        internal int num_lines;
        internal int num_faces;
        internal int max_loot;
        internal int scoop_type;
        internal double size;
        internal int front_laser;
        internal int bounty;
        internal int vanish_point;
        internal int energy;
        internal int velocity;
        internal int missiles;
        internal int laser_strength;
        internal ship_point[] points;
        internal ship_line[] lines;
        internal ship_face_normal[] normals;

        internal ship_data(string name, int num_points, int num_lines, int num_faces, int max_loot,
            int scoop_type, double size, int front_laser, int bounty, int vanish_point, int energy,
            int velocity, int missiles, int laser_strength, ship_point[] points, ship_line[] lines, ship_face_normal[] normals)
        {
            this.name = name;
            this.num_points = num_points;
            this.num_lines = num_lines;
            this.num_faces = num_faces;
            this.max_loot = max_loot;
            this.scoop_type = scoop_type;
            this.size = size;
            this.front_laser = front_laser;
            this.bounty = bounty;
            this.vanish_point = vanish_point;
            this.energy = energy;
            this.velocity = velocity;
            this.missiles = missiles;
            this.laser_strength = laser_strength;
            this.points = points;
            this.lines = lines;
            this.normals = normals;
        }
    };
}
