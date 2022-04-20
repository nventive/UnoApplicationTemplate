#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate.View.Controls
{
	public class SnapPointInfo
	{
		public SnapPointInfo(float nearSnapPoint, float centerSnapPoint, float farSnapPoint)
		{
			this.NearSnapPoint = nearSnapPoint;
			this.CenterSnapPoint = centerSnapPoint;
			this.FarSnapPoint = farSnapPoint;
		}

		public float NearSnapPoint { get; }
		public float CenterSnapPoint { get; }
		public float FarSnapPoint { get; }
	}
}
#endif
