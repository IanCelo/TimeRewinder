//#define TEMPLATE_SAVER

using System;
using System.Reflection;
using System.Runtime.InteropServices;


namespace CVS_Time
{

#if TEMPLATE_SAVER
	public class TimeSaverField<T> : ITimerSaver
#else
	public class TimeSaverField : ITimerSaver
#endif
	{
		private object _classInstance;
		private object[] _savedData;
		private object _firstData;

		private FieldInfo _field;
		
		public TimeSaverField(int array_size, MemberInfo member_info, object class_instance)
		{
			_savedData = new object[array_size];
			_field = (FieldInfo)member_info;
			_classInstance = class_instance;
		}
		
		public void Init(int array_size, MemberInfo member_info, object class_instance)
		{
			_savedData = new object[array_size];
			_field = (FieldInfo)member_info;
			_classInstance = class_instance;
		}

		public void Save(int index)
		{
			object value = _field.GetValue(_classInstance);
			_savedData[index] = value;
		}
		
		public void Rewind(int index)
		{
			_field.SetValue(_classInstance, (object)_savedData[index]);
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
