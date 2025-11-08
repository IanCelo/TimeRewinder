using System.Reflection;


namespace CVS_Time
{
	public interface ITimerSaver
	{
		public void Init(int arraySize, MemberInfo member_info, object class_instance);
		
		public void Save(int index);

		public void Rewind(int index);

		public int SaveToString();
	}
}
