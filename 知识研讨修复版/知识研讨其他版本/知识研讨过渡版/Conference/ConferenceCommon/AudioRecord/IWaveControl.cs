using System;

namespace ConferenceCommon.AudioRecord
{
	public delegate void ErrorEventHandle(Exception e,string error);
	/// <summary>
	/// IWaveControl ��ժҪ˵����
	/// </summary>
	public interface IWaveControl : IWaveRecordInfo
	{
		void Stop();
		void Start();

        void StreamDispose();

		event ErrorEventHandle ErrorEvent;
	}
}
