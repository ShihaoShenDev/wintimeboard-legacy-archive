namespace WinTimeBoard.Models
{
    /// <summary>
    /// 主题模式枚举
    /// </summary>
    public enum ThemeMode
    {
        /// <summary>
        /// 跟随系统
        /// </summary>
        FollowSystem = 0,
        
        /// <summary>
        /// 浅色模式
        /// </summary>
        Light = 1,
        
        /// <summary>
        /// 深色模式
        /// </summary>
        Dark = 2
    }

    /// <summary>
    /// 背景材质枚举
    /// </summary>
    public enum BackgroundMaterial
    {
        /// <summary>
        /// 无背景
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Mica 效果
        /// </summary>
        Mica = 1,
        
        /// <summary>
        /// MicaAlt 效果
        /// </summary>
        MicaAlt = 2,
        
        /// <summary>
        /// Acrylic 效果
        /// </summary>
        Acrylic = 3
    }
}