using GenericApp.Common.Responses;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericApp.Common.Data
{
    public class UsersDatabase
    {
        private readonly SQLiteAsyncConnection database;

        public UsersDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<UsuarioAppResponse>().Wait();
        }

        public async Task<List<UsuarioAppResponse>> GetItemsAsync()
        {
            return await database.Table<UsuarioAppResponse>().ToListAsync();
        }

        public async Task<UsuarioAppResponse> GetItemAsync(int id)
        {
            return await database.Table<UsuarioAppResponse>()
                .Where(i => i.ID == id)
                .FirstOrDefaultAsync();
            var a = 1;
        }

        public Task<int> SaveItemAsync(UsuarioAppResponse item)
        {
            return database.InsertAsync(item);
        }

        public Task<int> UpdateItemAsync(UsuarioAppResponse item)
        {
            return database.UpdateAsync(item);
        }



        public Task<int> DeleteItemAsync(UsuarioAppResponse item)
        {
            return database.DeleteAsync(item);
        }

    }
}