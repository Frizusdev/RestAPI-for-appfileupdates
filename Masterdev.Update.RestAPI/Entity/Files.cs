using System;
using System.ComponentModel.DataAnnotations;

namespace Masterdev.Update.RestAPI.Entity
{
	public class Files
	{
		[Key]
		public int id { get; set; }
		public int createdby { get; set; }
		public int app_id { get; set; }
		public string file_name { get; set; }
		public string file_path { get; set; }
		public string version { get; set; }
		public DateTime date { get; set; }
	}
}

