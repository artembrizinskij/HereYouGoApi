using System;
using System.Collections.Generic;
using System.Text;
using DAl.Sql.Repositories;
using Domain.Entities;

namespace DAl.Sql.Services
{
    public class CommonDbService : BaseSqlService<CommonContext>, ICommonDbService
    {
        public CommonDbService(CommonContext context) : base(context) { }

        private IRepository<Account> _accounts;

        public IRepository<Account> Accounts => _accounts ?? (_accounts = new Repository<Account>(Context));
    }
}
