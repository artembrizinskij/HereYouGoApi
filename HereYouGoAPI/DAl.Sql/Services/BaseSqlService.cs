using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAl.Sql.Services
{
    public class BaseSqlService<TContext> : IBaseSqlService where TContext : DbContext
    {
        protected readonly TContext Context;

        protected BaseSqlService(TContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public async void CommitAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
