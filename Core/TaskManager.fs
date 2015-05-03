using System;

namespace Tasky.Core {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class TaskManager {
		public static TaskDatabase theDb = new TaskDatabase(TaskDatabase.DatabaseFilePath("TaskDatabase.db3"));
	}
}