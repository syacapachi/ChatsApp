using System;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
/// 
public abstract class ChatManagerBase
{
    /// <summary>
    /// (string message,string username , string timestamp)
    /// �f�[�^�x�[�X���X�V�����ƁA���b�Z�[�W��Ԃ��C�x���g�n���h��
    /// </summary>
    public abstract event Action<string,string,string> OnMessageReceived;
    public abstract void SendMessage(string message);
    public abstract void SendPicture(string pictureURL);
    /// <summary>
    /// �f�[�^�x�[�X�ɕύX�������OnMessageReceived�Ƀ��b�Z�[�W��n���悤�ɂȂ�֐��B
    /// </summary>
    public abstract void StartListenMessages(string roomId);
    /// <summary>
    /// ���̃��\�b�h���Ăяo���Ȃ����胁�b�Z�[�W����M�������Ă��܂��܂��B
    /// </summary>
    public abstract void StopLister();
    public static ChatManagerBase Instance { get; protected set; }
}
