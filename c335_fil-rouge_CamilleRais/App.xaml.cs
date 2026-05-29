/******************************************************************************
** PROGRAMME  App.xaml.cs                                                    **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Point d'entrée de l'application.                                          **
** Crée la fenêtre principale avec AppShell.                                 **
**                                                                           **
******************************************************************************/

namespace c335_fil_rouge_CamilleRais
{
    public partial class App : Application
    {
        /// <summary>
        /// constructeur de l'app
        /// </summary>
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