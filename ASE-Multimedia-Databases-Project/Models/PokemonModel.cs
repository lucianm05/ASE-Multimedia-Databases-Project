namespace ASE_Multimedia_Databases_Project.Models
{
    public class PokemonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Generation { get; set; }
        public int? EvolutionId { get; set; }
        public List<int>? Images { get; set; }
        public int? Sound { get; set; }
    }

    public class PokemonDTO
    {
        public string Name { get; set; }
        public int Generation { get; set; }
        public IFormFile[] Images { get; set; }
        public IFormFile[] Sound { get; set; }
    }

    public class FindSimilarPokemonsDTO
    {
        public IFormFile[] Images { get; set; }
    }
}
