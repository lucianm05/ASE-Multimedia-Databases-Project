namespace ASE_Multimedia_Databases_Project.Models
{
    public class PokemonImageModel
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public byte[] Image { get; set; }
    }
}
