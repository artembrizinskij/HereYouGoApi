using System;
using DAl.Sql.Services;
using Domain.Entities;

namespace Security.Providers
{
    public interface IContextProvider
    {
        Account Account { get; }
        void Inizialize(string accountId, ICommonDbService db);

        /// <summary>
        /// Получить Id контекстного пользователя
        /// </summary>
        Guid GetCurrentId();
    }
}