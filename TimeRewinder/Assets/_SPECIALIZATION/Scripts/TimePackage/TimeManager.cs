//#define TEMPLATE_SAVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace CVS_Time
{
	public class TimeManager : Singleton<TimeManager>
	{
		[SerializeField] private int _savedSeconds;
		
		[SerializeField ] private KeyCode _rewindActivation;
		[SerializeField] private Scrollbar _scrollbar; 
		
		public Action<int> OnSaveData;
		public Action<int> OnRewindData;
		public Action OnSaveStart;
		public Action OnRewindStart;

		private bool _isSaving = true;
		
		private int _maximumSaveSize;
		private int _maximumIndex;
		private int _minimumIndex;
		private int _currentIndex;

		private ITimerSaver[] _savers;

		public override void Awake()
		{
			base.Awake();

			_maximumSaveSize = (int)(_savedSeconds * (1 / Time.fixedUnscaledDeltaTime));
			OnSaveStart += Resume;
		}

		private void Start()
		{
			Assembly my_assembly = Assembly.GetExecutingAssembly();

			List<ITimerSaver> savers = new ();

			foreach (Type type in my_assembly.GetTypes())
			{
				TimeSavedAttribute time_save_attributed = type.GetCustomAttribute<TimeSavedAttribute>();
				if (time_save_attributed is null)
					continue;

				FieldInfo[] field_infos = type.GetDeclaredFields().Where(field => field.GetCustomAttribute<TimeSavedAttribute>() is not null).ToArray();
				PropertyInfo[] property_infos = type.GetDeclaredProperties().Where(property => property.GetCustomAttribute<TimeSavedAttribute>() is not null).ToArray();

				foreach (Object classInstance in FindObjectsOfType(type))
				{
					CreateFieldSaves(field_infos, classInstance, savers);

					CreatePropertySaves(property_infos, classInstance, savers);
				}
			}

			_savers = savers.ToArray();
		}
		
	#if !TEMPLATE_SAVER
		private void CreatePropertySaves(IEnumerable<PropertyInfo> property_infos, Object class_instance, List<ITimerSaver> savers)
		{
			foreach (PropertyInfo property_info in property_infos)
			{
				if (property_info.GetGetMethod(true) is null) continue;

				TimeSaverProperty property_save = new (_maximumSaveSize, property_info, class_instance);
				OnSaveData += property_save.Save;
				
				if (property_info.GetSetMethod(true) is not null)
					OnRewindData += property_save.Rewind;
				
				savers.Add(property_save);
			}
		}

		private void CreateFieldSaves(IEnumerable<FieldInfo> field_infos, Object class_instance, List<ITimerSaver> savers)
		{
			foreach (FieldInfo field_info in field_infos)
			{
				TimeSaverField field_save = new (_maximumSaveSize, field_info, class_instance);
				
				OnSaveData += field_save.Save;
				OnRewindData += field_save.Rewind;
				savers.Add(field_save);
			}
		}
	#else
		private void CreatePropertySaves(IEnumerable<PropertyInfo> property_infos, Object class_instance, List<ITimerSaver> savers)
		{
			foreach (PropertyInfo property_info in property_infos)
			{
				if (property_info.GetGetMethod(true) is null) continue;

				Type saver_type = typeof(TimeSaverProperty<>).MakeGenericType(property_info.PropertyType);
				ITimerSaver property_save = (ITimerSaver)Activator.CreateInstance(saver_type);
				property_save.Init(_maximumSaveSize, property_info, class_instance);

				OnSaveData += property_save.Save;
				if (property_info.GetSetMethod(true) is not null)
					OnRewindData += property_save.Rewind;
				savers.Add(property_save);
			}
		}

		private void CreateFieldSaves(IEnumerable<FieldInfo> field_infos, Object class_instance, List<ITimerSaver> savers)
		{
			foreach (FieldInfo field_info in field_infos)
			{
				Type saver_type = typeof(TimeSaverField<>).MakeGenericType(field_info.FieldType);
				ITimerSaver field_save = (ITimerSaver)Activator.CreateInstance(saver_type);
				field_save.Init(_maximumSaveSize, field_info, class_instance);
				
				OnSaveData += field_save.Save;
				OnRewindData += field_save.Rewind;
				savers.Add(field_save);
			}
		}
	#endif
		
		private void Update() 
		{
			if (Input.GetKeyDown(KeyCode.I))
				WriteSaveFile();

			float totalUsed = (_maximumIndex - _minimumIndex) / (float)_maximumSaveSize;
			
			float scrollbar_value = (_currentIndex - _minimumIndex) / (float)(_maximumIndex - _minimumIndex - 1) * totalUsed;
			scrollbar_value = Mathf.Clamp(scrollbar_value, 0.001f, 0.9999f);
			_scrollbar.value = scrollbar_value;

			if (!Input.GetKeyDown(_rewindActivation)) return;

			_isSaving = !_isSaving;

			if (_isSaving)
				OnSaveStart?.Invoke();
			else
				OnRewindStart?.Invoke();
		}

		private void WriteSaveFile()
		{
			Debug.Log($"Save is currently {_savers.Sum(timer_saver => timer_saver.SaveToString()) / 1024} kb");
		}

		private void Resume()
		{
			_maximumIndex = _currentIndex;
		}
		
		private void RewindFrame()
		{
			int maximum_save_size = _currentIndex % _maximumSaveSize;
			
			OnRewindData?.Invoke(maximum_save_size);
		}

		private void FixedUpdate()
		{
			if (_isSaving)
			{
				SaveFrame();
				return;
			}
			
			bool has_frame_changed = false;
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				_currentIndex--;
				has_frame_changed = true;
			}

			if (Input.GetKey(KeyCode.RightArrow))
			{
				_currentIndex++;
				has_frame_changed = true;
			}

			if (!has_frame_changed) return;

			_currentIndex = Math.Clamp(_currentIndex, _minimumIndex, _maximumIndex - 1);
			RewindFrame();
		}

		private void SaveFrame()
		{
			OnSaveData?.Invoke(_currentIndex % _maximumSaveSize);
			
			_maximumIndex++;

			if (_maximumIndex >= _maximumSaveSize)
				_minimumIndex++;

			_currentIndex = _maximumIndex;
		}
	}
}
