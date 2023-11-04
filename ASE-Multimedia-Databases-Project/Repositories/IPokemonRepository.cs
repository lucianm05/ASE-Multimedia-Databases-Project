using ASE_Multimedia_Databases_Project.Models;

namespace ASE_Multimedia_Databases_Project.Repositories
{
    public interface IPokemonRepository
    {
        public List<PokemonModel> FindMany();
        public PokemonModel FindOne(int id);
        public Task<byte[]> FindImage(int imageId);
        public Task<byte[]> FindSound(int soundId);
        public Task<List<PokemonModel>> FindSimilarPokemons(FindSimilarPokemonsDTO findSimilarPokemonsDTO);
        public Task<int> Create(PokemonDTO pokemonDto);
        
    }
}
