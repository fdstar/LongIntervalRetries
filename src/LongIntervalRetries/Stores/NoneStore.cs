using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LongIntervalRetries.Stores
{
    internal class NoneStore<TKey> : IStore<TKey>
    {
        public Task<IEnumerable<StoredInfo<TKey>>> GetAllUnfinishedRetries()
        {
            return Task.FromResult(default(IEnumerable<StoredInfo<TKey>>));
        }

        public Task<TKey> InsertAndGetId(StoredInfo<TKey> entity)
        {
            return Task.FromResult(default(TKey));
        }

        public Task Update(StoredInfo<TKey> entit)
        {
            return Task.FromResult(true);
        }
    }
}
