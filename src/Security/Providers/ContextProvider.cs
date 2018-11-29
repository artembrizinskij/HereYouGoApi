using System;
using DAl.Sql.Services;
using Domain.Entities;

namespace Security.Providers
{
    public class ContextProvider : IContextProvider
    {
        public Account Account { get; private set; }

        public void Inizialize(string accountId, ICommonDbService db)
        {
            if (Account == null && !string.IsNullOrEmpty(accountId))
            {
                Guid.TryParse(accountId, out Guid id);
                Account = db.Accounts.FindOneById(id);
            }
        }

        public Guid GetCurrentId()
        {
            if (Account != null)
                return Account.Id;
            return default(Guid);
        }
    }
}
