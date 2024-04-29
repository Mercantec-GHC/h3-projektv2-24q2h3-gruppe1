namespace BlazorApp.Components.Pages
{
    public partial class Settings
    {
        private bool IsAutoChecked = true;
        private bool IsManualChecked = false;

        private void Toggle(char switchName)
        {
            if (!IsManualChecked && !IsAutoChecked)
            {
                IsAutoChecked = true;
            }
            else
            {
                IsAutoChecked = !IsAutoChecked;
                IsManualChecked = !IsManualChecked;
            }
        }
    }
}
