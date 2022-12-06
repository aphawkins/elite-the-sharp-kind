namespace Elite.Structs
{
    internal struct ship_solid
    {
        internal int num_faces;
        internal ship_face[] face_data;

        internal ship_solid(int num_faces, ship_face[] face_data)
        {
            this.num_faces = num_faces;
            this.face_data = face_data;
        }
    };
}