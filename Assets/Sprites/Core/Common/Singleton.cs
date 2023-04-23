/// <summary>
/// 单例
/// 关于资源的类不允许使用单例
/// </summary>
public class Singleton<T> where T : class, new()
{
    /// <summary>  Static Fields </summary>
    protected static T m_Instance;

    /// <summary> Static Properties </summary>
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new T();
            }
            return m_Instance;
        }
    }

    public virtual void InitDataM() { }
    public virtual void DestroyM() { }

    /// <summary> Static Methods </summary>
    public static T GetInstance()
    {
        return Instance;
    }

}
