namespace Elite.Structs
{
    internal class ship_solid
    {
        internal ship_face[] face_data;

        internal ship_solid(ship_face[] face_data)
        {
            this.face_data = face_data;
        }
    };
}