/*
 * generated 2021-03-27 15:34:00.282286
 */
using AutoGenerated.DB;
using System.Linq;
using System.Collections.Generic;
using JJFramework.Runtime.Resource;

namespace Scripts.Core.DB
{
	public class MasterDB
	{ 
		public Dictionary<int, CommonDefine.Define> CommonDefine { get; private set; }
		public Dictionary<int, Stage.Define> Stage { get; private set; }
		public Dictionary<int, Char.Define> Char { get; private set; }
		public Dictionary<int, Prob.Define> Prob { get; private set; }
		
		public void Init()
		{
			this.CommonDefine = DataLoader.Load<AutoGenerated.DB.CommonDefine>().GetData().ToDictionary(data => data.idx);
			this.Stage = DataLoader.Load<AutoGenerated.DB.Stage>().GetData().ToDictionary(data => data.phase);
			this.Char = DataLoader.Load<AutoGenerated.DB.Char>().GetData().ToDictionary(data => data.idx);
			this.Prob = DataLoader.Load<AutoGenerated.DB.Prob>().GetData().ToDictionary(data => data.idx);
		}

		public void Cleanup()
		{
			this.CommonDefine.Clear();
			this.CommonDefine = null;
			this.Stage.Clear();
			this.Stage = null;
			this.Char.Clear();
			this.Char = null;
			this.Prob.Clear();
			this.Prob = null;
		}
	}
}