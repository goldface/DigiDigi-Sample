/*
 * generated 2021-03-27 15:33:59.656988
 */
using System.Collections.Generic;
namespace AutoGenerated.DB
{
	[JJFramework.Runtime.Attribute.Table("CommonDefine")]
	public class CommonDefine
	{
		public class Define
		{
	        [Newtonsoft.Json.JsonProperty("idx")]
			public int idx { get; set; }
	        [Newtonsoft.Json.JsonProperty("name")]
			public string name { get; set; }
	        [Newtonsoft.Json.JsonProperty("description")]
			public string description { get; set; }
	        [Newtonsoft.Json.JsonProperty("unit")]
			public string unit { get; set; }
	        [Newtonsoft.Json.JsonProperty("type")]
			public int type { get; set; }
	        [Newtonsoft.Json.JsonProperty("typeInt")]
			public int typeInt { get; set; }
	        [Newtonsoft.Json.JsonProperty("typeFloat")]
			public float typeFloat { get; set; }
	        [Newtonsoft.Json.JsonProperty("typeString")]
			public string typeString { get; set; }
		}

        [Newtonsoft.Json.JsonProperty("data")]
		public List<Define> data;
		public List<Define> GetData() { return data; }
	}	
	[JJFramework.Runtime.Attribute.Table("Stage")]
	public class Stage
	{
		public class Define
		{
	        [Newtonsoft.Json.JsonProperty("phase")]
			public int phase { get; set; }
	        [Newtonsoft.Json.JsonProperty("playtime")]
			public int playtime { get; set; }
	        [Newtonsoft.Json.JsonProperty("spawn_time_min")]
			public int spawn_time_min { get; set; }
	        [Newtonsoft.Json.JsonProperty("spawn_time_max")]
			public int spawn_time_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("come_spd_min")]
			public int come_spd_min { get; set; }
	        [Newtonsoft.Json.JsonProperty("come_spd_max")]
			public int come_spd_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("wait_min")]
			public int wait_min { get; set; }
	        [Newtonsoft.Json.JsonProperty("wait_max")]
			public int wait_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("out_min")]
			public int out_min { get; set; }
	        [Newtonsoft.Json.JsonProperty("out_max")]
			public int out_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("spd_up_min")]
			public int spd_up_min { get; set; }
	        [Newtonsoft.Json.JsonProperty("spd_up_max")]
			public int spd_up_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("apper_max")]
			public int apper_max { get; set; }
	        [Newtonsoft.Json.JsonProperty("apper_prob")]
			public int apper_prob { get; set; }
		}

        [Newtonsoft.Json.JsonProperty("data")]
		public List<Define> data;
		public List<Define> GetData() { return data; }
	}	
	[JJFramework.Runtime.Attribute.Table("Char")]
	public class Char
	{
		public class Define
		{
	        [Newtonsoft.Json.JsonProperty("idx")]
			public int idx { get; set; }
	        [Newtonsoft.Json.JsonProperty("resource")]
			public string resource { get; set; }
	        [Newtonsoft.Json.JsonProperty("HP")]
			public int HP { get; set; }
	        [Newtonsoft.Json.JsonProperty("type")]
			public int type { get; set; }
	        [Newtonsoft.Json.JsonProperty("heart")]
			public int heart { get; set; }
	        [Newtonsoft.Json.JsonProperty("score")]
			public int score { get; set; }
	        [Newtonsoft.Json.JsonProperty("bonus_time")]
			public int bonus_time { get; set; }
	        [Newtonsoft.Json.JsonProperty("lose_time")]
			public int lose_time { get; set; }
		}

        [Newtonsoft.Json.JsonProperty("data")]
		public List<Define> data;
		public List<Define> GetData() { return data; }
	}	
	[JJFramework.Runtime.Attribute.Table("Prob")]
	public class Prob
	{
		public class Define
		{
	        [Newtonsoft.Json.JsonProperty("idx")]
			public int idx { get; set; }
	        [Newtonsoft.Json.JsonProperty("group")]
			public int group { get; set; }
	        [Newtonsoft.Json.JsonProperty("char_idx")]
			public int char_idx { get; set; }
	        [Newtonsoft.Json.JsonProperty("prob")]
			public int prob { get; set; }
		}

        [Newtonsoft.Json.JsonProperty("data")]
		public List<Define> data;
		public List<Define> GetData() { return data; }
	}	
}
