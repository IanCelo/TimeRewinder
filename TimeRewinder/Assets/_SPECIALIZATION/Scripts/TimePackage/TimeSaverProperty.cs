//#define TEMPLATE_SAVER

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;


namespace CVS_Time
{
	
#if TEMPLATE_SAVER
	public class TimeSaverProperty<T> : ITimerSaver
#else
	public class TimeSaverProperty : ITimerSaver
#endif
	{
		private object _classInstance;
		private object[] _savedData;

		private PropertyInfo _property;
		
		public TimeSaverProperty(int array_size, MemberInfo member_info, object class_instance)
		{
			_savedData = new object[array_size];
			_property = (PropertyInfo)member_info;
			_classInstance = class_instance;
		}
		
		public void Init(int array_size, MemberInfo member_info, object class_instance)
		{
			_savedData = new object[array_size];
			_property = (PropertyInfo)member_info;
			_classInstance = class_instance;
		}

		public void Save(int index)
		{
			object value = _property.GetValue(_classInstance);

			_savedData[index] = value;
		}
		
		public void Rewind(int index)
		{
			_property.SetValue(_classInstance, _savedData[index]);
		}

		public int SaveToString()
		{
			int out_size = 0;

			foreach (object variable in _savedData)
			{
				if (variable is null) return out_size;

				Type structure = variable.GetType();
				out_size += Marshal.SizeOf(structure);
			}

			return out_size;
		}
	}

}
