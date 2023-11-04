using ASE_Multimedia_Databases_Project.Contexts;
using ASE_Multimedia_Databases_Project.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ASE_Multimedia_Databases_Project.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private IPokemonDbContext dbContext;

        public PokemonRepository(IPokemonDbContext context)
        {
            dbContext = context;
        }

        public List<PokemonModel> FindMany()
        {
            List<PokemonModel> pokemons = new List<PokemonModel>();

            using (OracleConnection connection = dbContext.GetConnection())
            {
                using (OracleCommand command = connection.CreateCommand())
                {
                    try
                    {
                        connection.Open();

                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = @"select p.*, pi.id as image_id, ps.id as sound_id
                                            from p_pokemons p 
                                            join p_pokemons_images pi 
                                            on p.id = pi.pokemon_id 
                                            join p_pokemons_sounds ps
                                            on p.id = ps.pokemon_id
                                            order by p.id asc, image_id asc";

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                PokemonModel pokemon = new PokemonModel();

                                int id = Convert.ToInt32(reader["id"]);
                                int imageId = Convert.ToInt32(reader["image_id"]);

                                pokemon.Id = id;
                                pokemon.Name = Convert.ToString(reader["name"]);
                                pokemon.Generation = Convert.ToInt32(reader["generation"]);
                                pokemon.Images = new List<int>();
                                pokemon.Sound = Convert.ToInt32(reader["sound_id"]);

                                int listIndex = pokemons.FindIndex(p => p.Id == id);

                                if (listIndex < 0)
                                {
                                    pokemon.Images.Add(imageId);
                                    pokemons.Add(pokemon);
                                }
                                else
                                {
                                    pokemons[listIndex]?.Images?.Add(imageId);
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return pokemons;
        }

        public PokemonModel FindOne(int id)
        {
            PokemonModel pokemon = null;

            using (OracleConnection connection = dbContext.GetConnection())
            {
                using (OracleCommand command = connection.CreateCommand())
                {
                    try
                    {
                        connection.Open();

                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = "select * from p_pokemons where id = :id";

                        command.Parameters.Add("id", id);

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                pokemon = new PokemonModel();

                                while (reader.Read())
                                {
                                    pokemon.Id = Convert.ToInt32(reader["id"]);
                                    pokemon.Name = Convert.ToString(reader["name"]);
                                    pokemon.Generation = Convert.ToInt32(reader["generation"]);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return pokemon;
        }

        public async Task<List<PokemonModel>> FindSimilarPokemons(FindSimilarPokemonsDTO dto)
        {
            List<PokemonModel> pokemons = new List<PokemonModel>();

            using (OracleConnection connection = dbContext.GetConnection())
            {
                try
                {
                    connection.Open();

                    OracleString returnedIds = null;

                    using (OracleCommand command = dbContext.GetCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "get_similar_pokemons_ids";

                        command.Parameters.Add("Return_Value", OracleDbType.Varchar2, 255);
                        command.Parameters["Return_Value"].Direction = System.Data.ParameterDirection.ReturnValue;

                        command.Parameters.Add("p_image", OracleDbType.Blob);

                        var image = dto.Images.ElementAt(0);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await image.CopyToAsync(ms);
                            byte[] fileBytes = ms.ToArray();

                            command.Parameters["p_image"].Value = fileBytes;
                        }

                        await command.ExecuteNonQueryAsync();

                        returnedIds = (OracleString)command.Parameters["Return_Value"].Value;
                    }

                    if (returnedIds == null) return pokemons;

                    using (OracleCommand command = dbContext.GetCommand())
                    {
                        string[] ids = returnedIds.ToString().Split(',');
                        List<string> queryIds = new List<string>();

                        foreach(string id in ids)
                        {
                            string queryId = "'" + id + "'";
                            queryIds.Add(queryId);
                        }

                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = @"select p.*, pi.id as image_id, ps.id as sound_id
                                            from p_pokemons p 
                                            join p_pokemons_images pi 
                                            on p.id = pi.pokemon_id 
                                            join p_pokemons_sounds ps
                                            on p.id = ps.pokemon_id
                                            where p.id in (" + String.Join(", ", queryIds) + ")" 
                                            + "order by p.id asc, image_id asc";

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PokemonModel pokemon = new PokemonModel();

                                int id = Convert.ToInt32(reader["id"]);
                                int imageId = Convert.ToInt32(reader["image_id"]);

                                pokemon.Id = id;
                                pokemon.Name = Convert.ToString(reader["name"]);
                                pokemon.Generation = Convert.ToInt32(reader["generation"]);
                                pokemon.Images = new List<int>();
                                pokemon.Sound = Convert.ToInt32(reader["sound_id"]);

                                int listIndex = pokemons.FindIndex(p => p.Id == id);

                                if (listIndex < 0)
                                {
                                    pokemon.Images.Add(imageId);
                                    pokemons.Add(pokemon);
                                }
                                else
                                {
                                    pokemons[listIndex]?.Images?.Add(imageId);
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }

            }

            return pokemons;
        }

        public async Task<int> Create(PokemonDTO pokemonDto)
        {
            int id = 0;
            OracleTransaction transaction = null;

            using (OracleConnection connection = dbContext.GetConnection())
            {
                try
                {
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    using (OracleCommand command = dbContext.GetCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "insert_pokemon";

                        command.Parameters.Add("Return_Value", OracleDbType.Int32, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_name", OracleDbType.Varchar2, 25)
                            .Value = Convert.ToString(pokemonDto.Name);
                        command.Parameters.Add("p_generation", OracleDbType.Int32)
                            .Value = Convert.ToInt32(pokemonDto.Generation);

                        await command.ExecuteNonQueryAsync();

                        id = Convert.ToInt32((decimal)(OracleDecimal)(command.Parameters["Return_Value"].Value));
                    }

                    using (OracleCommand command = dbContext.GetCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "insert_pokemon_image";

                        command.Parameters.Add("Return_Value", OracleDbType.Int32, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_image", OracleDbType.Blob);
                        command.Parameters.Add("p_pokemon_id", OracleDbType.Int32);

                        foreach (var image in pokemonDto.Images)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await image.CopyToAsync(ms);
                                byte[] fileBytes = ms.ToArray();

                                command.Parameters["p_image"].Value = fileBytes;
                                command.Parameters["p_pokemon_id"].Value = id;

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "insert_pokemon_sound";

                        command.Parameters.Add("Return_Value", OracleDbType.Int32, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_sound", OracleDbType.Blob);
                        command.Parameters.Add("p_pokemon_id", OracleDbType.Int32);

                        var sound = pokemonDto.Sound.ElementAt(0);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            await sound.CopyToAsync(ms);
                            byte[] fileBytes = ms.ToArray();

                            command.Parameters["p_sound"].Value = fileBytes;
                            command.Parameters["p_pokemon_id"].Value = id;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    if (transaction != null)
                    {
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception e)
                {
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();

                    }
                    throw e;
                }
                finally
                {
                    if (transaction != null)
                    {
                        await transaction.DisposeAsync();
                    }
                    connection.Close();
                }
            }

            return id;
        }

        public async Task<byte[]> FindImage(int imageId)
        {
            byte[] imageBytes;

            using (OracleConnection connection = dbContext.GetConnection())
            {
                using (OracleCommand command = dbContext.GetCommand())
                {
                    try
                    {
                        await connection.OpenAsync();

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "get_pokemon_image";

                        command.Parameters.Add("Return_Value", OracleDbType.Blob, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_image_id", OracleDbType.Int32).Value = imageId;

                        await command.ExecuteNonQueryAsync();

                        OracleBlob dbBlob = (OracleBlob)command.Parameters["Return_Value"].Value;

                        imageBytes = new byte[dbBlob.Length];

                        await dbBlob.ReadAsync(imageBytes, 0, (int)dbBlob.Length);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return imageBytes;
        }

        public async Task<byte[]> FindSound(int soundId)
        {
            byte[] soundBytes;

            using (OracleConnection connection = dbContext.GetConnection())
            {
                using (OracleCommand command = dbContext.GetCommand())
                {
                    try
                    {
                        await connection.OpenAsync();

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "get_pokemon_sound";

                        command.Parameters.Add("Return_Value", OracleDbType.Blob, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_sound_id", OracleDbType.Int32).Value = soundId;

                        await command.ExecuteNonQueryAsync();

                        OracleBlob dbBlob = (OracleBlob)command.Parameters["Return_Value"].Value;

                        soundBytes = new byte[dbBlob.Length];

                        await dbBlob.ReadAsync(soundBytes, 0, (int)dbBlob.Length);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return soundBytes;
        }
    }
}
