using System.ComponentModel;

namespace System.Configuration.Install
{
	internal class InstallerParentConverter : ReferenceConverter
	{
		public InstallerParentConverter(Type type)
			: base(type)
		{
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			var standardValues = base.GetStandardValues(context);
			var instance = context.Instance;
			var i = 0;
			var num = 0;
			var array = new object[standardValues.Count - 1];
			for (; i < standardValues.Count; i++)
			{
				if (standardValues[i] != instance)
				{
					array[num] = standardValues[i];
					num++;
				}
			}
			return new StandardValuesCollection(array);
		}
	}
}
