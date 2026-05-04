namespace Bloxstrap.UI.ViewModels.Installer
{
    public class WelcomeViewModel : NotifyPropertyChangedViewModel
    {
        // formatting is done here instead of in xaml, it's a bit easier
        public string MainText => String.Format(
            Strings.Installer_Welcome_MainText,
            "[github.com/cloudstrap-dev/Cloudstrap](https://github.com/cloudstrap-dev/Cloudstrap)"
        );

        public bool CanContinue { get; set; } = false;
    }
}
