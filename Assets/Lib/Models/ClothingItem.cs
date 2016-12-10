namespace Assets.Lib.Models
{
    public class ClothingItem
    {
        public string Texture { get; private set; }
        public string Mesh { get; private set; }
        public string RootMeshName { get; private set; }
        public string Material { get; private set; }
        public string Id { get; private set; }
        public int BaseYRotation { get; private set; }
        public float BaseScale { get; private set; }

        public ClothingItem(
            string id,
            string texture,
            string mesh,
            string rootMeshName,
            string material,
            int baseYRotation,
            float baseScale
            )
        {
            Id = id;
            Texture = texture;
            Mesh = mesh;
            RootMeshName = rootMeshName;
            Material = material;
            BaseYRotation = baseYRotation;
            BaseScale = baseScale;
        }

    }
}