using Oracle.ManagedDataAccess.Client;

namespace ASE_Multimedia_Databases_Project.Contexts
{
    public class PokemonDbContext: IPokemonDbContext
    {
        private IConfiguration config;
        private OracleConnection connection;
        private OracleCommand command;
        private string connectionString;

        public PokemonDbContext(IConfiguration cfg)
        {
            config = cfg;
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public OracleConnection GetConnection()
        {
            connection = new OracleConnection(connectionString);
            return connection;
        }

        public OracleCommand GetCommand()
        {
            command = connection.CreateCommand();
            return command;
        }
    }
}
