namespace Elite.Engine.Types
{
    internal class ship_data
    {
        internal string name;
        internal int max_loot;
        internal StockType scoopedType;
        internal float size;
        internal int front_laser;
        internal float bounty;
        internal int vanish_point;
        internal int energy;
        internal float velocity;
        internal int missiles;
        internal int laser_strength;
        internal ship_point[] points;
        internal ship_line[] lines;
        internal ship_face_normal[] normals;
        internal ship_face[] face_data;

        internal ship_data()
        {
        }

        internal ship_data(string name, int max_loot,
            StockType scoopedType, float size, int front_laser, float bounty, int vanish_point, int energy,
            float velocity, int missiles, int laser_strength, ship_point[] points, ship_line[] lines, ship_face_normal[] normals, ship_face[] face_data)
        {
            this.name = name;
            this.max_loot = max_loot;
            this.scoopedType = scoopedType;
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
            this.face_data = face_data;
        }
    };
}
