using AsyncTaskManagerWpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsyncTaskManagerWpf.Services
{
    public class TodoRepository
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public TodoRepository()
        {
            _folderPath = Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                "AsyncTaskManagerWpf" );

            _filePath = Path.Combine( _folderPath, "todos.json" );
        }

        public async Task SaveAsync( List<TodoItem> todos )
        {
            if ( !Directory.Exists( _folderPath ) ) {
                Directory.CreateDirectory( _folderPath );
            }

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;

            using ( FileStream stream = new FileStream(
                _filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                true ) ) {
                await JsonSerializer.SerializeAsync( stream, todos, options );
            }
        }

        public async Task<List<TodoItem>> LoadAsync()
        {
            if ( !File.Exists( _filePath ) ) {
                return new List<TodoItem>();
            }

            using ( FileStream stream = new FileStream(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true ) ) {
                List<TodoItem> todos = await JsonSerializer.DeserializeAsync<List<TodoItem>>( stream );

                if ( todos == null ) {
                    return new List<TodoItem>();
                }

                return todos;
            }
        }
    }
}