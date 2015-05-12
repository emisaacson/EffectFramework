
using System;

namespace EffectFramework.Core.Models.Db
{
    public interface IDbContext : IDisposable
    {
        IDisposable BeginTransaction();
        void Commit();
        void Rollback();
    }
}
