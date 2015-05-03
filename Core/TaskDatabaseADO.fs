using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

using Mono.Data.Sqlite;

namespace Tasky.Core {
	/// <summary>
	/// TaskDatabase uses ADO.NET to create the [Items] table and create,read,update,delete data
	/// </summary>
	public class TaskDatabase : IDisposable {
		static object locker = new object();
		private SqliteConnection connection = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and the table.
		/// </summary>
		public TaskDatabase(string dbPath) {
			connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();

			// create the table (and ignore the exception, if the table already exists)
		    var c = connection.CreateCommand();
			c.CommandText = "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, Name NTEXT, Notes NTEXT, Done INTEGER);";
			try {c.ExecuteNonQuery();} catch {}
		}

		public void Dispose() {
			connection.Close();
		}

  		public static string DatabaseFilePath(string sqliteFilename) {
			#if NETFX_CORE
			var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
			#else

			#if SILVERLIGHT
			// Windows Phone expects a local path, not absolute
			var path = sqliteFilename;
			#else

			#if __ANDROID__
			// Just use whatever directory SpecialFolder.Personal returns
			string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			#else
			// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
			// (they don't want non-user-generated data in Documents)
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
			#endif
			var path = Path.Combine(libraryPath, sqliteFilename);
			#endif

			#endif
			return path;	
		}
			
		/// <summary>Convert from DataReader to Task object</summary>
		private static Task FromReader(SqliteDataReader r) {
			var t = new Task();
			t.ID = Convert.ToInt32(r ["_id"]);
			t.Name = r ["Name"].ToString();
			t.Notes = r ["Notes"].ToString();
			t.Done = Convert.ToInt32(r ["Done"]) == 1 ? true : false;
			return t;
		}

		public List<Task> GetTasks() {
			var tl = new List<Task>();

			lock(locker) {
				using(var contents = connection.CreateCommand()) {
					contents.CommandText = "SELECT [_id], [Name], [Notes], [Done] from [Items]";
					var r = contents.ExecuteReader();
					while(r.Read()) {
						tl.Add(FromReader(r));
					}
				}
			}
			return tl;
		}

		public Task GetTask(int id) {
			var t = new Task();
			lock(locker) {
				using(var command = connection.CreateCommand()) {
					command.CommandText = "SELECT [_id], [Name], [Notes], [Done] from [Items] WHERE [_id] = ?";
					command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
					var r = command.ExecuteReader();
					while(r.Read()) {
						t = FromReader(r);
						break;
					}
				}
			}
			return t;
		}

		public int SaveTask(Task t) {
			int r = 0;
			lock(locker) {
				if(t.ID != 0) {
					using(var command = connection.CreateCommand()) {
						command.CommandText = "UPDATE [Items] SET [Name] = ?, [Notes] = ?, [Done] = ? WHERE [_id] = ?;";
						command.Parameters.Add(new SqliteParameter(DbType.String) { Value = t.Name });
						command.Parameters.Add(new SqliteParameter(DbType.String) { Value = t.Notes });
						command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = t.Done });
						command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = t.ID });
						r = command.ExecuteNonQuery();
					}
					return r;
				} else {
					using(var command = connection.CreateCommand()) {
						command.CommandText = "INSERT INTO [Items] ([Name], [Notes], [Done]) VALUES (? ,?, ?)";
						command.Parameters.Add(new SqliteParameter(DbType.String) { Value = t.Name });
						command.Parameters.Add(new SqliteParameter(DbType.String) { Value = t.Notes });
						command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = t.Done });
						r = command.ExecuteNonQuery();
					}
					return r;
				}
			}
		}

		public int DeleteTask(int id) {
			lock(locker) {
				int r;
				using(var command = connection.CreateCommand()) {
					command.CommandText = "DELETE FROM [Items] WHERE [_id] = ?;";
					command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id});
					r = command.ExecuteNonQuery();
				}
				return r;
			}
		}
	}
}