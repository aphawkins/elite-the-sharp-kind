namespace Elite.Structs
{
    public struct ship_data
    {
        public string name;
        public int num_points;
        public int num_lines;
        public int num_faces;
        public int max_loot;
        public int scoop_type;
        public double size;
        public int front_laser;
        public int bounty;
        public int vanish_point;
        public int energy;
        public int velocity;
        public int missiles;
        public int laser_strength;
        public ship_point[] points;
        public ship_line[] lines;
        public ship_face_normal[] normals;

        public ship_data(string name, int num_points, int num_lines, int num_faces, int max_loot,
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
