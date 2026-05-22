namespace c335_fil_rouge_CamilleRais
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // pour activer le capteur pour que l'on puisse secouer le telephone et transmettre l'info
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}