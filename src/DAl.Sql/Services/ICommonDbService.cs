using DAl.Sql.Repositories;
using Domain.Entities;

namespace DAl.Sql.Services
{
    public interface ICommonDbService : IBaseSqlService
    {
        IRepository<Account> Accounts { get; }
    }
}