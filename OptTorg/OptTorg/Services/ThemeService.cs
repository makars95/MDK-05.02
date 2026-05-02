using System;
using System.Windows;

namespace OptTorg.Services
{
    public static class ThemeService
    {
        public enum AppTheme
        {
            Dark,
            Light
        }

        private static AppTheme _currentTheme = AppTheme.Dark;

        public static AppTheme CurrentTheme
        {
            get => _currentTheme;
            private set => _currentTheme = value;
        }

        public static void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;

            var resources = Application.Current.Resources;

            if (theme == AppTheme.Dark)
            {
                resources["BgDark"] = resources["DarkBgDark"];
                resources["BgMedium"] = resources["DarkBgMedium"];
                resources["BgCard"] = resources["DarkBgCard"];
                resources["BgInput"] = resources["DarkBgInput"];
                resources["PrimaryBlue"] = resources["DarkPrimaryBlue"];
                resources["PrimaryHover"] = resources["DarkPrimaryHover"];
                resources["SuccessGreen"] = resources["DarkSuccessGreen"];
                resources["ErrorRed"] = resources["DarkErrorRed"];
                resources["TextPrimary"] = resources["DarkTextPrimary"];
                resources["TextSecondary"] = resources["DarkTextSecondary"];
                resources["BorderColor"] = resources["DarkBorderColor"];
                resources["WarningOrange"] = resources["DarkWarningOrange"];
                resources["SeparatorColor"] = resources["DarkSeparatorColor"];
                resources["InputPlaceholder"] = resources["DarkInputPlaceholder"];
                resources["SecondaryBlue"] = resources["DarkSecondaryBlue"];
                resources["AccentBlue"] = resources["DarkAccentBlue"];
                resources["BgGradient"] = resources["DarkBgGradient"];
            }
            else
            {
                resources["BgDark"] = resources["LightBgDark"];
                resources["BgMedium"] = resources["LightBgMedium"];
                resources["BgCard"] = resources["LightBgCard"];
                resources["BgInput"] = resources["LightBgInput"];
                resources["PrimaryBlue"] = resources["LightPrimaryBlue"];
                resources["PrimaryHover"] = resources["LightPrimaryHover"];
                resources["SuccessGreen"] = resources["LightSuccessGreen"];
                resources["ErrorRed"] = resources["LightErrorRed"];
                resources["TextPrimary"] = resources["LightTextPrimary"];
                resources["TextSecondary"] = resources["LightTextSecondary"];
                resources["BorderColor"] = resources["LightBorderColor"];
                resources["WarningOrange"] = resources["LightWarningOrange"];
                resources["SeparatorColor"] = resources["LightSeparatorColor"];
                resources["InputPlaceholder"] = resources["LightInputPlaceholder"];
                resources["SecondaryBlue"] = resources["LightSecondaryBlue"];
                resources["AccentBlue"] = resources["LightAccentBlue"];
                resources["BgGradient"] = resources["LightBgGradient"];
            }

            // Сохраняем выбор
            try
            {
                Properties.Settings.Default.Theme = theme.ToString();
                Properties.Settings.Default.Save();
            }
            catch
            {
                // Игнорируем ошибки сохранения
            }
        }

        public static void ToggleTheme()
        {
            SetTheme(CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark);
        }
    }
}