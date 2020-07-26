using Exiled.API.Interfaces;
using System.ComponentModel;

namespace CustomLoadouts
{
    public class Config : IConfig
    {
        [Description("Плагин включен или нет.")]
        public bool IsEnabled { get; set; } = false;
        [Description("Использовать глобальный конфиг.")]
        public bool Cl_global { get; set; } = true;
        [Description("Выдать предметы в зависимости от уровня игрока?")]
        public bool Cl_levelsystem { get; set; } = false;
    }
}
