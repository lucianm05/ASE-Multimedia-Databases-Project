using Oracle.ManagedDataAccess.Client;

namespace ASE_Multimedia_Databases_Project.Contexts
{
    public interface IPokemonDbContext
    {
        OracleCommand GetCommand();
        OracleConnection GetConnection();
    }
}
