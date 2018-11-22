namespace DAl.Sql.Services
{
    public interface IBaseSqlService
    {
        void Commit();
        void CommitAsync();
    }
}